using CSharpFunctionalExtensions;

using Ineditta.Application.SharedKernel.Auth;
using Ineditta.Application.Usuarios.Repositories;
using Ineditta.BuildingBlocks.Core.Database;
using Ineditta.BuildingBlocks.Core.Domain.Handlers;

using MediatR;

namespace Ineditta.Application.Usuarios.UseCases.AtualizarPermissoes
{
    public class AtualizarPermissaoRequestHandler : BaseCommandHandler, IRequestHandler<AtualizarPermissaoRequest, Result>
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IAuthService _authService;

        public AtualizarPermissaoRequestHandler(IUnitOfWork unitOfWork, IUsuarioRepository usuarioRepository, IAuthService authService) : base(unitOfWork)
        {
            _usuarioRepository = usuarioRepository;
            _authService = authService;
        }

        public async Task<Result> Handle(AtualizarPermissaoRequest request, CancellationToken cancellationToken)
        {
            var usuario = await _usuarioRepository.ObterPorIdAsync(request.Id);

            if (usuario is null)
            {
                return Result.Failure("Usuário não foi encontrado");
            }

            var result = await _authService.AtualizarPermissoesAsync(usuario);

            return result.IsSuccess ? Result.Success() : Result.Failure(result.Error.Message);
        }
    }
}
