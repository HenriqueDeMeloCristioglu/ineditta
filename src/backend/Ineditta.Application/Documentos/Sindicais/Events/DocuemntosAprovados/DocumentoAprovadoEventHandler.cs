using CSharpFunctionalExtensions;

using Ineditta.Application.ClientesUnidades.Repositories;
using Ineditta.Application.Cnaes.Repositories;
using Ineditta.Application.Documentos.Sindicais.Builders;
using Ineditta.Application.Documentos.Sindicais.Dtos;
using Ineditta.Application.Documentos.Sindicais.Entities;
using Ineditta.Application.Documentos.Sindicais.Repositories;
using Ineditta.Application.Documentos.Sindicais.Services;
using Ineditta.Application.Localizacoes.Repositories;
using Ineditta.Application.Sindicatos.Laborais.Repositories;
using Ineditta.Application.Sindicatos.Patronais.Repositories;
using Ineditta.Application.TiposDocumentos.Repositories;
using Ineditta.Application.Usuarios.Repositories;
using Ineditta.BuildingBlocks.Core.Bus;

namespace Ineditta.Application.Documentos.Sindicais.Events.DocuemntosAprovados
{
#pragma warning disable CA1711 // Identifiers should not have incorrect suffix
    public class DocumentoAprovadoEventHandler : IRequestHandler<DocumentoAprovadoEvent>
#pragma warning restore CA1711 // Identifiers should not have incorrect suffix
    {
        private readonly IDocumentoSindicalEmailService _documentoSindicalEmailService;
        private readonly IDocumentoSindicalRepository _documentoSindicalRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IClienteUnidadeRepository _clienteUnidadeRepository;
        private readonly ICnaeRepository _cnaeRepository;
        private readonly ISindicatoLaboralRepository _sindicatoLaboralRepository;
        private readonly ISindicatoPatronalRepository _sindicatoPatronalRepository;
        private readonly ITipoDocumentoRepository _tipoDocumentoRepository;
        private readonly ILocalizacaoRepository _localizacaoRepository;

        public DocumentoAprovadoEventHandler(IDocumentoSindicalEmailService documentoSindicalEmailService, IDocumentoSindicalRepository documentoSindicalRepository, IUsuarioRepository usuarioRepository, IClienteUnidadeRepository clienteUnidadeRepository, ICnaeRepository cnaeRepository, ISindicatoLaboralRepository sindicatoLaboralRepository, ISindicatoPatronalRepository sindicatoPatronalRepository, ITipoDocumentoRepository tipoDocumentoRepository, ILocalizacaoRepository localizacaoRepository)
        {
            _documentoSindicalEmailService = documentoSindicalEmailService;
            _documentoSindicalRepository = documentoSindicalRepository;
            _usuarioRepository = usuarioRepository;
            _clienteUnidadeRepository = clienteUnidadeRepository;
            _cnaeRepository = cnaeRepository;
            _sindicatoLaboralRepository = sindicatoLaboralRepository;
            _sindicatoPatronalRepository = sindicatoPatronalRepository;
            _tipoDocumentoRepository = tipoDocumentoRepository;
            _localizacaoRepository = localizacaoRepository;
        }

        public async ValueTask<Result> Handle(DocumentoAprovadoEvent message, CancellationToken cancellationToken = default)
        {
            const string estruturaClausulaDataBaseId = "197";

            var documento = await _documentoSindicalRepository.ObterPorIdAsync(message.DocumentoId);
            if (documento == null) return Result.Failure("O documento de id " + message.DocumentoId + " não foi encontrado");
            if (!documento.DataAprovacao.HasValue) return Result.Failure("O documento não está aprovado.");

            var usuario = await _usuarioRepository.ObterPorIdAsync(message.UsuarioId);
            if (usuario == null)
            {
                return Result.Failure("Nenhum usuário a ser notificado");
            }

            var documentoSisapEmailBuilder = new DocumentoSisapInfoEmailBuilder(_localizacaoRepository);

            documentoSisapEmailBuilder.AdicionarAbrangencia(documento);

            var unidadesUsuarioDocumento = await _clienteUnidadeRepository.ObterClientesUnidadePorDocumentoPorUsuario(documento.Id, usuario.Id);
            var unidadesUsuarioDocumentoIds = unidadesUsuarioDocumento is null ? new List<int>() : unidadesUsuarioDocumento.Select(u => u.Id);

            var atividadesEconomicasUnidades = await _cnaeRepository.ObterPorUnidadesIds(unidadesUsuarioDocumentoIds);
            documentoSisapEmailBuilder.AdicionarAtividadesEconomicas(atividadesEconomicasUnidades, documento);

            var sindicatosLaboraisUsuario = await _sindicatoLaboralRepository.ObterPorClientesUnidadeIdsAsync(unidadesUsuarioDocumentoIds);
            documentoSisapEmailBuilder.AdicionarSindicatosLaborais(sindicatosLaboraisUsuario, documento);

            var sindicatosPatronaisUsuario = await _sindicatoPatronalRepository.ObterPorClienteUnidadesIdsAsync(unidadesUsuarioDocumentoIds);
            documentoSisapEmailBuilder.AdicionarSindicatosPatronais(sindicatosPatronaisUsuario, documento);

            var tipoDocumento = await _tipoDocumentoRepository.ObterPorIdAsync(documento.TipoDocumentoId);

            await documentoSisapEmailBuilder.AdicionarUnidadesCliente(unidadesUsuarioDocumento, true);

            documentoSisapEmailBuilder.AdicionarVigencias(documento);
            documentoSisapEmailBuilder.AdicionarNomeDocumento(tipoDocumento!);

            if (Enum.IsDefined(typeof(DocumentosComRegrasEspeciaisParaDataSla), tipoDocumento!.Id))
            {
                if (documento.Referencias is not null && documento.Referencias.Any(d => d == estruturaClausulaDataBaseId))
                {
                    documentoSisapEmailBuilder.AdicionarSla(documento);
                }
            }
            else
            {
                documentoSisapEmailBuilder.AdicionarSla(documento);
            }

            documentoSisapEmailBuilder.AdicionarAssunto(tipoDocumento!, documento, sindicatosLaboraisUsuario, sindicatosPatronaisUsuario);

            var emailInfo = documentoSisapEmailBuilder.Build<DocumentoAprovadoEmailDto>();

            var emails = new List<string>
            {
                usuario.Email.Valor
            };

            var resultEnvioEmail = await _documentoSindicalEmailService.EnviarNotificacaoDocumentoAprovadoAsync(emailInfo, documento.Id, emails, cancellationToken);

            if (resultEnvioEmail.IsFailure)
            {
                return Result.Failure("Não foi possível realizar o envio de email");
            }

            return Result.Success("Emails enviados com sucesso");
        }
    }
}
