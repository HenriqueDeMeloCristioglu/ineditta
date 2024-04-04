using CSharpFunctionalExtensions;

using Ineditta.Application.Clausulas.Gerais.Dtos;
using Ineditta.Application.Clausulas.Gerais.Templates;
using Ineditta.Application.SharedKernel.GestaoDeChamados;
using Ineditta.BuildingBlocks.Core.Domain.Models;
using Ineditta.Integration.Email.Dtos;
using Ineditta.Integration.Email.Protocols;
using Ineditta.BuildingBlocks.Core.Renderers.Html;

using Microsoft.Extensions.Options;
using Ineditta.Application.Documentos.Sindicais.Factories;

namespace Ineditta.Application.Clausulas.Gerais.Services.ClausulasEmailService
{
    public class ClausulasEmailService : IClausulasEmailService
    {
        private readonly IEmailSender _emailSender;
        private readonly IHtmlRenderer _htmlRenderer;
        private readonly GerarLinkArquivoFactory _gerarLinkArquivoFactory;
        private readonly GestaoDeChamadoConfiguration _gestaoDeChamadoConfigurations;

        public ClausulasEmailService(IEmailSender emailSender, IHtmlRenderer htmlRenderer, GerarLinkArquivoFactory gerarLinkArquivoFactory, IOptions<GestaoDeChamadoConfiguration> gestaoDeChamadoConfiguration)
        {
            _emailSender = emailSender;
            _htmlRenderer = htmlRenderer;
            _gerarLinkArquivoFactory = gerarLinkArquivoFactory;
            _gestaoDeChamadoConfigurations = gestaoDeChamadoConfiguration?.Value ?? throw new ArgumentNullException(nameof(gestaoDeChamadoConfiguration));
        }
        public async ValueTask<Result<bool, Error>> EnviarEmailClausulasAprovadasAsync(DocumentoClausulasLiberadasEmailDto infoEmail, long documentoId, IEnumerable<string> emails, CancellationToken cancellationToken)
        {
            infoEmail.Url = _gerarLinkArquivoFactory.Criar(documentoId);
            infoEmail.GestaoDeChamadosUrl = _gestaoDeChamadoConfigurations.Url;
            infoEmail.CodigosSindicatosCliente = infoEmail.Estabelecimentos is not null ? string.Join(", ", infoEmail.Estabelecimentos.Select(e => e.CodigoSindicatoCliente).Distinct().ToList()) : "";

            var html = await _htmlRenderer.RenderAsync(ClausulasTemplateId.ClausulasLiberadas, DocumentoClausulasLiberadasEmail.Html, infoEmail);

            if (html.IsFailure)
            {
                return Result.Failure<bool, Error>(Errors.Html.TemplateRender());
            }

            var result = await _emailSender.SendAsync(new SendEmailRequestDto(emails, infoEmail.Assunto, html.Value), cancellationToken);

            if (result.IsFailure)
            {
                return Result.Failure<bool, Error>(result.Error);
            }

            return Result.Success<bool, Error>(true);
        }
    }
}
