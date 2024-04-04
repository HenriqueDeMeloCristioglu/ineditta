using CSharpFunctionalExtensions;

using Ineditta.Application.SharedKernel.Auth;
using Ineditta.Application.SharedKernel.Frontend;
using Ineditta.Application.Usuarios.Dtos;
using Ineditta.Application.Usuarios.Repositories;
using Ineditta.Application.Usuarios.Templates;
using Ineditta.BuildingBlocks.Core.Domain.Models;
using Ineditta.BuildingBlocks.Core.Renderers.Html;
using Ineditta.Integration.Email.Dtos;
using Ineditta.Integration.Email.Protocols;

using Microsoft.Extensions.Options;

namespace Ineditta.Application.Usuarios.Services
{
    public class UsuarioEmailService : IUsuarioEmailService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IAuthService _authService;
        private readonly IHtmlRenderer _htmlRenderer;
        private readonly IEmailSender _emailSender;
        private readonly FrontendConfiguration _frontendConfiguration;

        public UsuarioEmailService(IUsuarioRepository usuarioRepository, IHtmlRenderer htmlRenderer, IEmailSender emailSender, IOptions<FrontendConfiguration> frontendConfiguration, IAuthService authService)
        {
            _usuarioRepository = usuarioRepository;
            _htmlRenderer = htmlRenderer;
            _emailSender = emailSender;
            _frontendConfiguration = frontendConfiguration?.Value ?? throw new ArgumentNullException(nameof(frontendConfiguration));
            _authService = authService;
        }

        public async ValueTask<Result<bool, Error>> EnviarBoasVindasAsync(string email, CancellationToken cancellationToken = default)
        {
            var usuario = await _usuarioRepository.ObterPorEmailAsync(email);

            if (usuario is null)
            {
                return Result.Failure<bool, Error>(Errors.General.Business("Usuário inválido"));
            }

            var usuarioAuth = await _authService.ObterPorEmailAsync(email);

            var emailBoaVindaDto = new EmailBoaVindaDto(_frontendConfiguration.Url, usuario.Email, usuario.Nome, usuarioAuth.Value.Username ?? string.Empty);
            
            var html = await _htmlRenderer.RenderAsync(UsuarioTemplateId.EmailBoasVindas, EmailBoasVindasTemplate.Html, emailBoaVindaDto);

            if (html.IsFailure)
            {
                return Result.Failure<bool, Error>(Errors.Html.TemplateRender());
            }

            var result = await _emailSender.SendAsync(new SendEmailRequestDto(new List<string> { usuario.Email }, "Acesso Sistema Ineditta!", html.Value), cancellationToken);

            return result.IsFailure ? Result.Failure<bool, Error>(result.Error) : Result.Success<bool, Error>(true);
        }
    }
}
