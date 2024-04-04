using CSharpFunctionalExtensions;
using CSharpFunctionalExtensions.ValueTasks;

using Ineditta.Application.ClientesUnidades.Entities;
using Ineditta.Application.ClientesUnidades.Repositories;
using Ineditta.Application.Documentos.Sindicais.Dtos;
using Ineditta.Application.Documentos.Sindicais.Factories;
using Ineditta.Application.Documentos.Sindicais.Repositories;
using Ineditta.Application.Documentos.Sindicais.Templates;
using Ineditta.Application.SharedKernel.GestaoDeChamados;
using Ineditta.Application.TemplatesEmails.Entities;
using Ineditta.Application.TemplatesEmails.Repositories;
using Ineditta.Application.Usuarios.Entities;
using Ineditta.Application.Usuarios.Factories;
using Ineditta.Application.Usuarios.Repositories;
using Ineditta.BuildingBlocks.Core.Domain.Models;
using Ineditta.BuildingBlocks.Core.Renderers.Html;
using Ineditta.Integration.Email.Dtos;
using Ineditta.Integration.Email.Protocols;

using Microsoft.Extensions.Options;

using SixLabors.ImageSharp;

namespace Ineditta.Application.Documentos.Sindicais.Services
{
    public class DocumentoSindicalEmailService : IDocumentoSindicalEmailService
    {
        private readonly IHtmlRenderer _htmlRenderer;
        private readonly IEmailSender _emailSender;
        private readonly IDocumentoInfoEmailRepository _documentoInfoEmailRepository;
        private readonly GerarLinkArquivoFactory _gerarLinkArquivoFactory;
        private readonly IClienteUnidadeRepository _clienteUnidadeRepository;
        private readonly GestaoDeChamadoConfiguration _gestaoDeChamadoConfigurations;
        private readonly ITemplateEmailRepository _templateEmailRepository;
        private readonly IUsuarioRepository _usuarioRepository;

        public DocumentoSindicalEmailService(IHtmlRenderer htmlRenderer, IEmailSender emailSender, IDocumentoInfoEmailRepository documentoInfoEmailRepository, GerarLinkArquivoFactory gerarLinkArquivoFactory, IClienteUnidadeRepository clienteUnidadeRepository, IOptions<GestaoDeChamadoConfiguration> gestaoDeChamadoConfiguration, ITemplateEmailRepository templateEmailRepository, GestaoDeChamadoConfiguration gestaoDeChamadoConfigurations, IUsuarioRepository usuarioRepository)
        {
            _documentoInfoEmailRepository = documentoInfoEmailRepository;
            _htmlRenderer = htmlRenderer;
            _emailSender = emailSender;
            _gerarLinkArquivoFactory = gerarLinkArquivoFactory;
            _clienteUnidadeRepository = clienteUnidadeRepository;
            _gestaoDeChamadoConfigurations = gestaoDeChamadoConfiguration?.Value ?? throw new ArgumentNullException(nameof(gestaoDeChamadoConfiguration));
            _templateEmailRepository = templateEmailRepository;
            _gestaoDeChamadoConfigurations = gestaoDeChamadoConfigurations;
            _usuarioRepository = usuarioRepository;
        }

        public async ValueTask<Result<bool, Error>> EnviarEmail(IEnumerable<string> emails, string template, string assunto, CancellationToken cancellationToken = default)
        {
            var result = await _emailSender.SendAsync(new SendEmailRequestDto(emails, assunto, template), cancellationToken);
            return result.IsFailure ? Result.Failure<bool, Error>(result.Error) : Result.Success<bool, Error>(true);
        }

        public async ValueTask<Result<bool, Error>> EnviarEmailNotificacaoDocumentoCriadoAsync(string email, long documentoSindicalId, CancellationToken cancellationToken = default)
        {
            var usuario = await _usuarioRepository.ObterPorEmailAsync(email);
            if (usuario == null)
            {
                return Result.Failure<bool, Error>(Error.Create("Email sender","Usuario para notificar não encontrado"));
            }

            var infoDocumentoSindicalId = await _documentoInfoEmailRepository.ObterInfoDocumentoCriadoEmail(documentoSindicalId);
            if (infoDocumentoSindicalId is null) return Result.Failure<bool, Error>(Errors.Http.NotFound());

            var estabelecimentosEntities = usuario.Nivel == Nivel.Ineditta ? new List<ClienteUnidade>() :
                await _clienteUnidadeRepository.ObterClientesUnidadePorDocumentoPorUsuario(documentoSindicalId, usuario.Id);

            var abrangencias = infoDocumentoSindicalId.Abrangencia != null ? string.Join(", ", infoDocumentoSindicalId.Abrangencia.Select(a => a.Municipio + "/" + a.Uf).ToList()) : "";
            var sindicatosLaborais = infoDocumentoSindicalId.SindicatosLaborais != null ? string.Join("; ", infoDocumentoSindicalId.SindicatosLaborais.Select(s => s.Sigla + " / " + s.Denominacao).ToList()) : "";
            var sindicatosPatronais = infoDocumentoSindicalId.SindicatosPatronais != null ? string.Join("; ", infoDocumentoSindicalId.SindicatosPatronais.Select(s => s.Sigla + " / " + s.Denominacao).ToList()) : "";
            var atividadesEconomicas = infoDocumentoSindicalId.AtividadesEconomicas != null ? string.Join("; ", infoDocumentoSindicalId.AtividadesEconomicas.Select(c => c.Subclasse).ToList()) : "";

            var assuntoEmail = "Notificação de Documento: " + infoDocumentoSindicalId.NomeDocumento + " " +
                (infoDocumentoSindicalId.ValidadeInicial.HasValue && infoDocumentoSindicalId.ValidadeInicial > DateOnly.MinValue ? " " + infoDocumentoSindicalId.ValidadeInicial : "") +
                (infoDocumentoSindicalId.ValidadeFinal.HasValue && infoDocumentoSindicalId.ValidadeFinal > DateOnly.MinValue ? " " + infoDocumentoSindicalId.ValidadeFinal : "");

            var infoEmail = new DocumentoCriadoEmailDto
            {
                OrigemDocumento = infoDocumentoSindicalId.OrigemDocumento,
                NomeDocumento = infoDocumentoSindicalId.NomeDocumento,
                Assunto = infoDocumentoSindicalId.Assuntos,
                NumeroLegislacao = infoDocumentoSindicalId.NumeroLegislacao,
                FonteLegislacaoSite = infoDocumentoSindicalId.FonteLegislacaoSite,
                Restrito = infoDocumentoSindicalId.Restrito ? "Sim" : "Não",
                SindicatosLaborais = sindicatosLaborais,
                SindicatosPatronais = sindicatosPatronais,
                ValidadeInicial = infoDocumentoSindicalId.ValidadeInicial,
                ValidadeFinal = infoDocumentoSindicalId.ValidadeFinal,
                Comentarios = infoDocumentoSindicalId.Comentarios,
                Estabelecimentos = estabelecimentosEntities,
                DocumentoLink = _gerarLinkArquivoFactory.Criar(documentoSindicalId),
                Abrangencia = abrangencias,
                AtividadesEconomicas = atividadesEconomicas
            };

            var listaUnitariaEmail = new List<string> { email };
            var result = await DispararEmails(DocumentosSindicaisTemplateId.DocumentoCriado, DocumentoSindicalNotificarUpsertEmail.Html, infoEmail, listaUnitariaEmail, assuntoEmail);

            return result;
        }

        public async ValueTask<Result<bool, Error>> EnviarNotificacaoDocumentoAprovadoAsync(DocumentoAprovadoEmailDto infoEmail, long documentoId, IEnumerable<string> emails, CancellationToken cancellationToken)
        {
            infoEmail.Url = _gerarLinkArquivoFactory.Criar(documentoId);
            infoEmail.GestaoDeChamadosUrl = _gestaoDeChamadoConfigurations.Url;
            infoEmail.CodigosSindicatosCliente = infoEmail.Estabelecimentos is not null ? string.Join(", ", infoEmail.Estabelecimentos.Select(e => e.CodigoSindicatoCliente).Distinct().ToList()) : "";

            foreach (var email in emails)
            {
                var listaUnitariaEmail = new List<string>() { email };

                var template = await _templateEmailRepository.ObterPorUsuarioEmailAsync(email, TipoTemplateEmail.DocumentoAprovadoSisap);
                var templateString = template is null || string.IsNullOrEmpty(template!.Html) ? DocumentoAprovadoSisapEmail.Html : template.Html;
                var templateKey = template is null || string.IsNullOrEmpty(template!.Html) ? DocumentosSindicaisTemplateId.DocumentoAprovado : "custom-template-" + template.Id;

                var result = await DispararEmails(templateKey, templateString, infoEmail, listaUnitariaEmail, infoEmail.Assunto);
                if (result.IsFailure) return result;
            }

            return Result.Success<bool, Error>(true);
        }

        private async ValueTask<Result<bool, Error>> DispararEmails(string templateKey, string templateContent, object emailModel, IEnumerable<string> listaEmails, string tituloEmail)
        {
            var html = await _htmlRenderer.RenderAsync(templateKey, templateContent, emailModel);

            if (html.IsFailure)
            {
                return Result.Failure<bool, Error>(Errors.Html.TemplateRender());
            }

            foreach (var email in listaEmails)
            {
                IEnumerable<string> listaUnitariaEmail = new List<string>() { email };
                var cancellationTokenEmailSender = new CancellationToken();
                var result = await _emailSender.SendAsync(new SendEmailRequestDto(listaUnitariaEmail, tituloEmail, html.Value), cancellationTokenEmailSender);
                if (result.IsFailure)
                {
                    return Result.Failure<bool, Error>(result.Error);
                }
            }

            return Result.Success<bool, Error>(true);
        }
    }
}
