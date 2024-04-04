using CSharpFunctionalExtensions;

using Ineditta.Application.SharedKernel.Auth;
using Ineditta.Application.Usuarios.Repositories;
using Ineditta.BuildingBlocks.Core.Database;
using Ineditta.BuildingBlocks.Core.Domain.Handlers;

using MediatR;

namespace Ineditta.Application.Usuarios.UseCases.AtualizacaoCredenciais
{
    public class EnviarEmailAtualizacaoCredenciaisRequestHandler : BaseCommandHandler, IRequestHandler<EnviarEmailAtualizacaoCredenciaisRequest, Result>
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IAuthService _authService;

        public EnviarEmailAtualizacaoCredenciaisRequestHandler(IUnitOfWork unitOfWork, IUsuarioRepository usuarioRepository, IAuthService authService) : base(unitOfWork)
        {
            _usuarioRepository = usuarioRepository;
            _authService = authService;
        }

        public async Task<Result> Handle(EnviarEmailAtualizacaoCredenciaisRequest request, CancellationToken cancellationToken)
        {
            var usuario = await _usuarioRepository.ObterPorIdAsync(request.Id);

            if (usuario is null)
            {
                return Result.Failure("Usuário não foi encontrado");
            }

            var result = await _authService.EnviarEmailAtualizacaoCredenciaisAsync(usuario);

            return result.IsFailure ? Result.Failure(result.Error.Message) : Result.Success();
        }
    }
}
