using System.Diagnostics;
using System.Linq.Expressions;

using CSharpFunctionalExtensions;

using Ineditta.Application.CalendarioSindicais.Eventos.Dtos;
using Ineditta.Application.CalendarioSindicais.Eventos.Entities;
using Ineditta.Application.CalendarioSindicais.Eventos.Repositories;
using Ineditta.Application.CalendarioSindicais.Eventos.Services;
using Ineditta.Application.NotificacaoEventos.UseCases.Upsert;
using Ineditta.Application.SharedKernel.FeaturesFlags;
using Ineditta.Application.TiposEventosCalendarioSindical.Entities;
using Ineditta.BuildingBlocks.Core.Domain.Models;

using MediatR;

using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;

using Newtonsoft.Json;

namespace Ineditta.Repository.CalendariosSindicais
{
    public class NotificacaoCalendarioSindicalService : INotificacaoCalendarioSindicalService
    {
        private readonly ILogger<ICalendarioSindicalGeradorService> _logger;
        private readonly IEventoRepository _eventoRepository;
        private readonly IEventoVencimentoDocumentoRepository _eventoVencimentoDocumentoRepository;
        private readonly IEventoVencimentoMandatoPatronalRepository _eventoVencimentoMandatoPatronalRepository;
        private readonly IEventoVencimentoMandatoLaboralRepository _eventoVencimentoMandatoLaboralRepository;
        private readonly IEventoTrintidioRepository _eventoTrintidioRepository;
        private readonly IEventoEmail _eventoEmail;
        private readonly IMediator _mediator;
        private readonly IFeatureManager _featureManager;
        public NotificacaoCalendarioSindicalService(ILogger<ICalendarioSindicalGeradorService> logger, IEventoRepository eventoRepository, IEventoEmail eventoEmail, IMediator mediator, IFeatureManager featureManager, IEventoVencimentoDocumentoRepository eventoVencimentoDocumentoRepository, IEventoVencimentoMandatoPatronalRepository eventoVencimentoMandatoPatronalRepository, IEventoVencimentoMandatoLaboralRepository eventoVencimentoMandatoLaboralRepository, IEventoTrintidioRepository eventoTrintidioRepository)
        {
            _logger = logger;
            _eventoRepository = eventoRepository;
            _eventoEmail = eventoEmail;
            _mediator = mediator;
            _featureManager = featureManager;
            _eventoVencimentoDocumentoRepository = eventoVencimentoDocumentoRepository;
            _eventoVencimentoMandatoPatronalRepository = eventoVencimentoMandatoPatronalRepository;
            _eventoVencimentoMandatoLaboralRepository = eventoVencimentoMandatoLaboralRepository;
            _eventoTrintidioRepository = eventoTrintidioRepository;
        }

        public async ValueTask<Result<bool, Error>> EnviarNotificacoesAsync(CancellationToken cancellationToken)
        {
#pragma warning disable IDE0059 // Atribuição desnecessária de um valor
#pragma warning disable CS0168 // A variável foi declarada, mas nunca foi usada
            try
            {
                if (!await _featureManager.IsEnabledAsync(nameof(FeatureFlag.NotificaCalendarioSindical)))
                {
                    return Result.Success<bool, Error>(true);
                }
                
                await NotificarVencimentoDocumentosAsync(cancellationToken);
                await NotificarVencimentoMandatosPatronaisAsync(cancellationToken);
                await NotificarVencimentoMandatosLaboralAsync(cancellationToken);
                await NotificarEventosClausulasAsync(cancellationToken);
                await NotificarAgendaEventosAsync(cancellationToken);
                await NotificarEventosTrintidio(cancellationToken);
                await NotificarAssembleisPatronais(cancellationToken);
                await NotificarReunioesSindicais(cancellationToken);

                _logger.LogInformation("Notificações de eventos enviadas com sucesso");
                return Result.Success<bool, Error>(true);
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Ocorreu um erro durante o envio das notificações");
                return Result.Failure<bool, Error>(Errors.General.Business("Ocorreu uma exceção ao tentar enviar as notificações"));
            }
#pragma warning restore CS0168 // A variável foi declarada, mas nunca foi usada
#pragma warning restore IDE0059 // Atribuição desnecessária de um valor
        }

        private async ValueTask NotificarVencimentoDocumentosAsync(CancellationToken cancellationToken)
        {
            IEnumerable<VencimentoDocumentoViewModel>? vencimentosDocumentos;
            try
            {
                vencimentosDocumentos = await _eventoVencimentoDocumentoRepository.ObterNotificacoesVencimentoDocumento();
            }
            catch
            {
                _logger.LogInformation("Algo deu errado durante a obtenção dos vencimentos de documentos pelo repository");
                return; 
            }

            if (vencimentosDocumentos is not null && vencimentosDocumentos.Any())
            {
#pragma warning disable S3267
#pragma warning disable CS8619
                foreach (var evento in vencimentosDocumentos)
                {
#pragma warning disable CA2254
                    try
                    {
                        var listaEmail = await _eventoVencimentoDocumentoRepository.ObterListaEmailNotificacao(evento.DocId ?? 0);

                        if (evento is not null && listaEmail is not null && listaEmail.Any())
                        {
                            var result = await _eventoEmail.EnviarNotificacaoVencimentoDocumentoAsync(listaEmail.ToList(), evento, cancellationToken);
                            if (result.IsSuccess)
                            {
                                var upsertNotificacaoRequest = new UpsertNotificacaoRequest
                                {
                                    EventoId = evento.Id ?? 0,
                                    Notificados = listaEmail
                                };

                                var insertNotificacaoResult = await _mediator.Send(upsertNotificacaoRequest, cancellationToken);
                                if (insertNotificacaoResult.IsFailure) _logger.LogInformation("O registro do envio dos email para o vencimento de documento de id " + evento.Id + " falhou");
                            }
                            else
                            {
                                _logger.LogInformation("Result falhou na tentativa de notificar vencimento de documento de id: " + evento.Id + " falhou.");
                            }
                        }
                    }
                    catch
                    { 
                        _logger.LogInformation("Erro ao notificar o vencimento de documento de id: " + evento.Id);
                    }
#pragma warning restore CA2254
                }
            }
        }

        private async ValueTask NotificarVencimentoMandatosPatronaisAsync(CancellationToken cancellationToken)
        {
            try
            {
                var vencimentosMandatosPatronais = await _eventoVencimentoMandatoPatronalRepository.ObterNotificacoesVencimentoMandatoPatronal();

                if (vencimentosMandatosPatronais is not null && vencimentosMandatosPatronais.Any())
                {
                    foreach (var evento in vencimentosMandatosPatronais)
                    {
#pragma warning disable CA2254
                        try
                        {
                            evento.Dirigentes = evento.DirigentesJson is not null ? JsonConvert.DeserializeObject<IEnumerable<Dirigente>?>(evento.DirigentesJson) : null;
                            var listaEmail = await _eventoVencimentoMandatoPatronalRepository.ObterListaEmailNotificacao(evento.SindicatoPatronalId ?? 0);

                            if (evento is not null && listaEmail is not null && listaEmail.Any())
                            {
                                IEnumerable<string> listaEmailNaoNula = listaEmail.Where(email => email is not null);

                                var result = await _eventoEmail.EnviarNotificacaoVencimentoMandatoPatronalAsync(listaEmailNaoNula.ToList(), evento, cancellationToken);
                                if (result.IsSuccess)
                                {
                                    var upsertNotificacaoRequest = new UpsertNotificacaoRequest
                                    {
                                        EventoId = evento.Id ?? 0,
                                        Notificados = listaEmail
                                    };

                                    await _mediator.Send(upsertNotificacaoRequest, cancellationToken);
                                }
                            }
                        }
                        catch
                        {
                            _logger.LogInformation("Erro ao tentar notificar o evento de mandato patronal de id: " + evento.Id);
                        }
#pragma warning restore CA2254
                    }
                }
            }
            catch
            {
                _logger.LogInformation("Algo deu errado com a notificação dos eventos de mandato patronal");
            }
        }

        private async ValueTask NotificarVencimentoMandatosLaboralAsync(CancellationToken cancellationToken)
        {
            try
            {
                var vencimentosMandatosLaborais = await _eventoVencimentoMandatoLaboralRepository.ObterNotificacoesVencimentoMandatoLaboral();

                if (vencimentosMandatosLaborais is not null && vencimentosMandatosLaborais.Any())
                {
                    foreach (var evento in vencimentosMandatosLaborais)
                    {
#pragma warning disable CA2254
                        try
                        {
                            evento.Dirigentes = evento.DirigentesJson is not null ? JsonConvert.DeserializeObject<IEnumerable<Dirigente>?>(evento.DirigentesJson) : null;
                            var listaEmail = await _eventoVencimentoMandatoLaboralRepository.ObterListaEmailNotificacao(evento.SindicatoLaboralId ?? 0);

                            if (evento is not null && listaEmail is not null && listaEmail.Any())
                            {
                                IEnumerable<string> listaEmailNaoNula = listaEmail.Where(email => email is not null);

                                var result = await _eventoEmail.EnviarNotificacaoVencimentoMandatoLaboralAsync(listaEmailNaoNula.ToList(), evento, cancellationToken);
                                if (result.IsSuccess)
                                {
                                    var upsertNotificacaoRequest = new UpsertNotificacaoRequest
                                    {
                                        EventoId = evento.Id ?? 0,
                                        Notificados = listaEmail
                                    };

                                    await _mediator.Send(upsertNotificacaoRequest, cancellationToken);
                                }
                            }
                        }
                        catch
                        {
                            _logger.LogInformation("Erro ao tentar notificar o evento de mandato laboral de id: " + evento.Id);
                        }
#pragma warning restore CA2254
                    }
                }
            }
            catch
            {
                _logger.LogInformation("Algo deu errado com a notificação dos eventos de mandato laboral");
            }    
        }

        private async ValueTask NotificarEventosClausulasAsync(CancellationToken cancellationToken)
        {
            var eventosClausulas = await _eventoRepository.ObterNotificacoesEventosClausulas();
            
            if (eventosClausulas is not null && eventosClausulas.Any())
            {
                foreach(var eventoClausula in eventosClausulas)
                {
                    var listaEmail = await _eventoRepository.ObterListaEmailEventoPorDocumentoId(eventoClausula.DocId ?? 0, eventoClausula.TipoEventoId, eventoClausula.SubtipoEventoId);

                    if (eventoClausula is not null && listaEmail is not null && listaEmail.Any())
                    {
                        IEnumerable<string> listaEmailNaoNula = listaEmail.Where(email => email is not null);

                        var result = await _eventoEmail.EnviarNotificacaoEventoClausulaAsync(listaEmailNaoNula.ToList(), eventoClausula, cancellationToken);
                        if (result.IsSuccess)
                        {
                            var upsertNotificacaoRequest = new UpsertNotificacaoRequest
                            {
                                EventoId = eventoClausula.Id ?? 0,
                                Notificados = listaEmail
                            };

                            await _mediator.Send(upsertNotificacaoRequest, cancellationToken);
                        }
                    }
                }
            }
        }

        private async ValueTask NotificarAgendaEventosAsync(CancellationToken cancellationToken)
        {
            var agendasEventos = await _eventoRepository.ObterNotificacoesAgendaEventos();

            if (agendasEventos is not null && agendasEventos.Any())
            {
                foreach(var evento in  agendasEventos)
                {
                    if (evento is null) { continue; }
                    if (evento.UsuarioCriadorEmail is null)
                    {
                        _logger.LogError("O usuário criador de está sem email. Evento id: {Id}",evento.Id);
                        return;
                    }

                    if (!evento.UsuarioNotificarEmail) continue;

                    List<string> listaUnitariaEmail = new() { evento.UsuarioCriadorEmail };

                    var result = await _eventoEmail.EnviarNotificacaoAgendaEventosAsync(listaUnitariaEmail, evento, cancellationToken);
                    if (result.IsSuccess)
                    {
                        var upsertNotificacaoRequest = new UpsertNotificacaoRequest
                        {
                            EventoId = evento.Id ?? 0,
                            Notificados = listaUnitariaEmail
                        };

                        await _mediator.Send(upsertNotificacaoRequest, cancellationToken);
                    }
                }
            }
        }

        private async ValueTask NotificarEventosTrintidio(CancellationToken cancellationToken)
        {
            try
            {
                var eventosTrintidio = await _eventoTrintidioRepository.ObterEventosTrintidioAsync();

                if (eventosTrintidio is not null && eventosTrintidio.Any())
                {
                    foreach (var evento in eventosTrintidio)
                    {
#pragma warning disable CA2254
                        try
                        {
                            if (evento is null) continue;
                            var listaEmail = await _eventoTrintidioRepository.ObterListaEmailPorDocumentoId(evento.DocId ?? 0);

                            if (evento is not null && listaEmail is not null && listaEmail.Any())
                            {
                                IEnumerable<string> listaEmailNaoNula = listaEmail.Where(email => email is not null);

                                var result = await _eventoEmail.EnviarNotificacaoEventoTrintidioAsync(listaEmailNaoNula.ToList(), evento, cancellationToken);
                                if (result.IsSuccess)
                                {
                                    var upsertNotificacaoRequest = new UpsertNotificacaoRequest
                                    {
                                        EventoId = evento.Id ?? 0,
                                        Notificados = listaEmail
                                    };

                                    await _mediator.Send(upsertNotificacaoRequest, cancellationToken);
                                }
                            }
                        }
                        catch
                        {
                            _logger.LogInformation("Erro ao tentar notificar o evento trintídio de id: " + evento?.Id);
                        }
#pragma warning restore CA2254
                    }
                }
            }
            catch
            {
                _logger.LogInformation("Algo deu errado com a notificação dos eventos trintídio");
            }  
        }

        private async ValueTask NotificarAssembleisPatronais(CancellationToken cancellationToken)
        {
            var assembleiasPatronais = await _eventoRepository.ObterAssembleiaPatronalReunioesSindicaisAsync(TipoEventoCalendarioSindical.AssembleiaPatronal.Id);

            if (assembleiasPatronais is not null && assembleiasPatronais.Any())
            {
                foreach (var assembleia in assembleiasPatronais)
                {
                    if (assembleia is null) continue;
                    var listaEmail = await _eventoRepository.ObterListaEmailAssembleiaPatronalReuniaoAsync(assembleia, TipoEventoCalendarioSindical.AssembleiaPatronal.Id);

                    if (assembleia is not null && listaEmail is not null && listaEmail.Any())
                    {
                        IEnumerable<string> listaEmailNaoNula = listaEmail.Where(email => email is not null);

                        var result = await _eventoEmail.EnviarNotificacaoAssembleiaPatronalAsync(listaEmailNaoNula.ToList(), assembleia, cancellationToken);
                        if (result.IsSuccess)
                        {
                            var upsertNotificacaoRequest = new UpsertNotificacaoRequest
                            {
                                EventoId = assembleia.Id ?? 0,
                                Notificados = listaEmail
                            };

                            await _mediator.Send(upsertNotificacaoRequest, cancellationToken);
                        }
                    }
                }
            }
        }

        private async ValueTask NotificarReunioesSindicais(CancellationToken cancellationToken)
        {
            var reunioesSindicais = await _eventoRepository.ObterAssembleiaPatronalReunioesSindicaisAsync(TipoEventoCalendarioSindical.ReuniaoEntrePartes.Id);

            if (reunioesSindicais is not null && reunioesSindicais.Any())
            {
                foreach (var reuniao in reunioesSindicais)
                {
                    if (reuniao is null) continue;
                    var listaEmail = await _eventoRepository.ObterListaEmailAssembleiaPatronalReuniaoAsync(reuniao, TipoEventoCalendarioSindical.ReuniaoEntrePartes.Id);

                    if (reuniao is not null && listaEmail is not null && listaEmail.Any())
                    {
                        IEnumerable<string> listaEmailNaoNula = listaEmail.Where(email => email is not null);

                        var result = await _eventoEmail.EnviarNotificacaoReuniaoSindicalAsync(listaEmailNaoNula.ToList(), reuniao, cancellationToken);
                        if (result.IsSuccess)
                        {
                            var upsertNotificacaoRequest = new UpsertNotificacaoRequest
                            {
                                EventoId = reuniao.Id ?? 0,
                                Notificados = listaEmail
                            };

                            await _mediator.Send(upsertNotificacaoRequest, cancellationToken);
                        }
                    }
                }
            }
        }
    }
}
