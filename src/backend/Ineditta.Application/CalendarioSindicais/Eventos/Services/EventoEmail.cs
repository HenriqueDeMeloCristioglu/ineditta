using CSharpFunctionalExtensions;
using Ineditta.BuildingBlocks.Core.Domain.Models;
using Ineditta.BuildingBlocks.Core.Renderers.Html;
using Ineditta.Application.CalendarioSindicais.Eventos.Dtos;
using Ineditta.Application.CalendarioSindicais.Eventos.Templates;
using Ineditta.BuildingBlocks.Core.Extensions;
using System.Globalization;
using Ineditta.Integration.Email.Protocols;
using Ineditta.Integration.Email.Dtos;

namespace Ineditta.Application.CalendarioSindicais.Eventos.Services
{
    public class EventoEmail : IEventoEmail
    {
        private readonly IHtmlRenderer _htmlRenderer;
        private readonly IEmailSender _emailSender;

        public EventoEmail(IHtmlRenderer htmlRenderer, IEmailSender emailSender)
        {
            _htmlRenderer = htmlRenderer;
            _emailSender = emailSender;
        }

        public async ValueTask<Result<bool, Error>> EnviarNotificacaoVencimentoDocumentoAsync(List<string> emails, VencimentoDocumentoViewModel evento, CancellationToken cancellationToken = default)
        {
            var atividadesEconomicas = evento.Cnaes is not null ? string.Join("; ", evento.Cnaes.Select(c => c.Subclasse).ToList()) : "";
            var abrangencias = evento.Abrangencias is not null ? string.Join(", ", evento.Abrangencias.Select(a => a.Municipio + "/" + a.Uf).ToList()) : "";
            var sindicatosLaborais = evento.SindicatosLaborais is not null ? string.Join(", ", evento.SindicatosLaborais.Select(s => s.Sigla).ToList()) : "";
            var sindicatosPatronais = evento.SindicatosPatronais is not null ? string.Join(", ", evento.SindicatosPatronais.Select(s => s.Sigla).ToList()) : "";

            string assuntoEmail = "Notificação calendário - Vencimento " + 
                                evento.SiglaDocumento + " " + 
                                (evento.SindicatosLaborais is not null && evento.SindicatosLaborais.Any() ? evento.SindicatosLaborais.Select(s => s.Sigla).ToList()[0] : "") + " " + 
                                evento.DataVencimento!.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

            var infoEmail = new EmailVencimentoDocumentoDto
            {
                Abrangencia = abrangencias,
                AtividadesEconomicas = atividadesEconomicas,
                SindicatoLaboral = sindicatosLaborais,
                SindicatoPatronal = sindicatosPatronais,
                DataVencimento = evento.DataVencimento,
                NomeDocumento = evento.NomeDocumento
            };

            var result = await DispararEmails(EventosTemplateId.VencimentoDocumentos, VencimentoDocumentoEmail.Html, infoEmail, emails, assuntoEmail, cancellationToken);

            return result;
        }

        public async ValueTask<Result<bool, Error>> EnviarNotificacaoVencimentoMandatoPatronalAsync(List<string> emails, VencimentoMandatoPatronalViewModel vencimentoMandatoPatronal, CancellationToken cancellationToken = default)
        {
           var abrangencias = vencimentoMandatoPatronal.Abrangencias is not null ? string.Join(", ", vencimentoMandatoPatronal.Abrangencias.Select(a => a.Municipio + "/" + a.Uf).ToList()) : "";

           var assuntoEmail = "Notificação calendário: Vencimentos de Mandatos Sindicais " + vencimentoMandatoPatronal.SiglaSindicato;

           var infoEmail = new EmailVencimentoMandatoPatronalDto
            {
                DataVencimento = vencimentoMandatoPatronal.DataVencimento,
                SindicatoPatronal = vencimentoMandatoPatronal.SindicatoPatronal,
                Abrangencia = abrangencias,
                Dirigente = vencimentoMandatoPatronal.Dirigentes,
            };

            var result = await DispararEmails(EventosTemplateId.VencimentoMandatoPatronal, VencimentoMandatoPatronalEmail.Html, infoEmail, emails, assuntoEmail, cancellationToken);

            return result;
        }

        public async ValueTask<Result<bool, Error>> EnviarNotificacaoVencimentoMandatoLaboralAsync(List<string> emails, VencimentoMandatoLaboralViewModel vencimentoMandatoLaboral, CancellationToken cancellationToken = default)
        {
            var abrangencias = vencimentoMandatoLaboral.Abrangencias is not null ? string.Join(", ", vencimentoMandatoLaboral.Abrangencias.Select(a => a.Municipio + "/" + a.Uf).ToList()) : "";

            var assuntoEmail = "Notificação calendário: Vencimentos de Mandatos Sindicais " + vencimentoMandatoLaboral.SiglaSindicato;

            var infoEmail = new EmailVencimentoMandatoLaboralDto
            {
                DataVencimento = vencimentoMandatoLaboral.DataVencimento,
                SindicatoLaboral = vencimentoMandatoLaboral.SindicatoLaboral,
                Abrangencia = abrangencias,
                Dirigente = vencimentoMandatoLaboral.Dirigentes,
            };

            var result = await DispararEmails(EventosTemplateId.VencimentoMandatoLaboral, VencimentoMandatoLaboralEmail.Html, infoEmail, emails, assuntoEmail, cancellationToken);

            return result;
        }

        public async ValueTask<Result<bool, Error>> EnviarNotificacaoEventoClausulaAsync(List<string> emails, EventoClausulaViewModel eventoClausula, CancellationToken cancellationToken = default)
        {
            var atividadesEconomicas = eventoClausula.Cnaes is not null ? string.Join("; ", eventoClausula.Cnaes.Select(c => c.Subclasse).ToList()) : "";
            var abrangencias = eventoClausula.Abrangencias is not null ? string.Join(", ", eventoClausula.Abrangencias.Select(a => a.Municipio + "/" + a.Uf).ToList()) : "";
            var sindicatosLaborais = eventoClausula.SindicatosLaborais is not null ? string.Join(", ", eventoClausula.SindicatosLaborais.Select(s => s.Sigla).ToList()) : "";
            var sindicatosPatronais = eventoClausula.SindicatosPatronais is not null ? string.Join(", ", eventoClausula.SindicatosPatronais.Select(s => s.Sigla).ToList()) : "";

            string assuntoEmail = "Notificação calendário - Vencimento obrigação " +
                                eventoClausula.NomeCampo + " " +
                                eventoClausula.SiglaDocumento + " " +
                                (eventoClausula.SindicatosLaborais is not null && eventoClausula.SindicatosLaborais.Any() ? eventoClausula.SindicatosLaborais.Select(s => s.Sigla).ToList()[0] : "") + " " +
                                eventoClausula.DataVencimento!.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

            var infoEmail = new EmailEventoClausulaDto
            {
                Abrangencias = abrangencias,
                AtividadesEconomicas = atividadesEconomicas,
                SindicatosLaborais = sindicatosLaborais,
                SindicatosPatronais = sindicatosPatronais,
                DataVencimento = eventoClausula.DataVencimento,
                NomeCampo = eventoClausula.NomeCampo,
                NomeDocumento = eventoClausula.NomeDocumento,
                NomeInfoAdicional = eventoClausula.NomeInfoAdicional,
            };

            var result = await DispararEmails(EventosTemplateId.EventoClausula, EventoClausulaEmail.Html, infoEmail, emails, assuntoEmail, cancellationToken);

            return result;
        }

        public async ValueTask<Result<bool, Error>> EnviarNotificacaoAgendaEventosAsync(List<string> emails, AgendaEventosViewModel eventoAgenda, CancellationToken cancellationToken = default)
        {
            var assuntoEmail = "Notificação de calendário - Agenda de eventos " + eventoAgenda.Titulo + " " + eventoAgenda.DataVencimento;
            var infoEmail = new EmailAgendaEventosDto
            {
                DataVencimento = eventoAgenda.DataVencimento,
                Local = eventoAgenda.Local,
                Recorrencia = eventoAgenda.Recorrencia!.Value.GetDescription(),
                Titulo = eventoAgenda.Titulo,
                ValidadeRecorrencia = eventoAgenda.ValidadeRecorrencia,
                Comentarios = eventoAgenda.Comentarios,
                Visivel = eventoAgenda.Visivel ? "Sim" : "Não"
            };

            var result = await DispararEmails(EventosTemplateId.AgendaEventos, AgendaEventosEmail.Html, infoEmail, emails, assuntoEmail, cancellationToken);

            return result;
        }

        public async ValueTask<Result<bool, Error>> EnviarNotificacaoEventoTrintidioAsync(List<string> emails, EventoTrintidioViewModel evento, CancellationToken cancellationToken = default)
        {
            var atividadesEconomicas = evento.Cnaes is not null ? string.Join("; ", evento.Cnaes.Select(c => c.Subclasse).ToList()) : "";
            var abrangencias = evento.Abrangencias is not null ? string.Join(", ", evento.Abrangencias.Select(a => a.Municipio + "/" + a.Uf).ToList()) : "";
            var sindicatosLaborais = evento.SindicatosLaborais is not null ? string.Join(", ", evento.SindicatosLaborais.Select(s => s.Sigla).ToList()) : "";
            var sindicatosPatronais = evento.SindicatosPatronais is not null ? string.Join(", ", evento.SindicatosPatronais.Select(s => s.Sigla).ToList()) : "";

            string assuntoEmail = "Notificação calendário - Vencimento " +
                                evento.SiglaDocumento + " " +
                                (evento.SindicatosLaborais is not null && evento.SindicatosLaborais.Any() ? evento.SindicatosLaborais.Select(s => s.Sigla).ToList()[0] : "") + " " +
                                evento.VigenciaInicial!.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) + " - " +
                                evento.VigenciaFinal!.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

            var infoEmail = new EmailEventoTrintidioDto
            {
                Abrangencia = abrangencias,
                AtividadesEconomicas = atividadesEconomicas,
                SindicatoLaboral = sindicatosLaborais,
                SindicatoPatronal = sindicatosPatronais,
                VigenciaInicio = evento.VigenciaInicial,
                VigenciaFinal = evento.VigenciaFinal,
                NomeDocumento = evento.NomeDocumento
            };

            var result = await DispararEmails(EventosTemplateId.EventoTrintidio, EventoTrintidioEmail.Html, infoEmail, emails, assuntoEmail, cancellationToken);

            return result;
        }

        public async ValueTask<Result<bool, Error>> EnviarNotificacaoReuniaoSindicalAsync(List<string> emails, AssembleiaReuniaoViewModelBase evento, CancellationToken cancellationToken = default)
        {
            const int indice_tipo_reuniao = 15;

            var respostasScript = evento.RespostasScript != null ? evento.RespostasScript.ToList() : null;

            var assuntoEmail = "Notificação de calendário - Reunião entre entidades sindicais " + evento.DataHora;

            var infoEmail = new EmailReuniaoSindicalDto
            {
                Abrangencia = evento.Abrangencia,
                AtividadesEconomicas = evento.AtividadesEconomicas,
                ClientReport = evento.Observacoes,
                DataBase = evento.DataBase,
                DataHora = evento.DataHora,
                FaseNegociacao = evento.FaseNegociacao,
                NomeDocumento = evento.NomeDocumento,
                SindicatosLaborais = evento.SindicatosLaborais,
                SindicatosPatronais = evento.SindicatosPatronais,
                TipoReuniaoSindical = respostasScript != null ? respostasScript[indice_tipo_reuniao] : "",
            };

            var result = await DispararEmails(EventosTemplateId.ReuniaoSindical, ReuniaoSindicalEmail.Html, infoEmail, emails, assuntoEmail, cancellationToken);

            return result;
        }

        public async ValueTask<Result<bool, Error>> EnviarNotificacaoAssembleiaPatronalAsync(List<string> emails, AssembleiaReuniaoViewModelBase evento, CancellationToken cancellationToken = default)
        {
            const int indice_tipo_assembleia = 10;
            const int indice_endereco_assembleia = 12;
            const int indice_comentario_assembleia = 13;

            var respostasScript = evento.RespostasScript != null ? evento.RespostasScript.ToList() : null;

            var assuntoEmail = "Notificação de calendário - Assembleia patronal com as empresas " + evento.DataHora;

            var infoEmail = new EmailAssembleiaPatronalDto
            {
                Abrangencia = evento.Abrangencia,
                AtividadesEconomicas = evento.AtividadesEconomicas,
                ClientReport = evento.Observacoes,
                ComentarioAssembleiaPatronal = respostasScript != null ? respostasScript[indice_comentario_assembleia] : "",
                DataBase = evento.DataBase,
                DataHora = evento.DataHora,
                EnderecoAssembleiaPatronal = respostasScript != null ? respostasScript[indice_endereco_assembleia] : "",
                FaseNegociacao = evento.FaseNegociacao,
                NomeDocumento = evento.NomeDocumento,
                SindicatosLaborais = evento.SindicatosLaborais,
                SindicatosPatronais = evento.SindicatosPatronais,
                TipoAssembleiaPatronal = respostasScript != null ? respostasScript[indice_tipo_assembleia] : "",
            };

            var result = await DispararEmails(EventosTemplateId.AssembleiaPatronal, AssembleiaPatronalEmail.Html, infoEmail, emails, assuntoEmail, cancellationToken);

            return result;
        }

        private async ValueTask<Result<bool,Error>> DispararEmails(string templateKey, string templateContent, object emailModel, IEnumerable<string> listaEmails, string tituloEmail, CancellationToken cancellationToken)
        {
            var html = await _htmlRenderer.RenderAsync(templateKey, templateContent, emailModel);

            if (html.IsFailure)
            {
                return Result.Failure<bool, Error>(Errors.Html.TemplateRender());
            }

            foreach (var email in listaEmails)
            {
                IEnumerable<string> listaUnitariaEmail = new List<string>() { email };
                var result = await _emailSender.SendAsync(new SendEmailRequestDto(listaUnitariaEmail, tituloEmail, html.Value), cancellationToken);
                if (result.IsFailure)
                {
                    return Result.Failure<bool, Error>(result.Error);
                }
            }

            return Result.Success<bool, Error>(true);
        }
    }
}
