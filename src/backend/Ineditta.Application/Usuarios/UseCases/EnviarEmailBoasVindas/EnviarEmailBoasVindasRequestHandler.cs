using CSharpFunctionalExtensions;

using Ineditta.Application.Usuarios.Services;
using Ineditta.BuildingBlocks.Core.Database;
using Ineditta.BuildingBlocks.Core.Domain.Handlers;
using Ineditta.BuildingBlocks.Core.Domain.Models;

using MediatR;

namespace Ineditta.Application.Usuarios.UseCases.EnviarEmailBoasVindas
{
    public class EnviarEmailBoasVindasRequestHandler : BaseCommandHandler, IRequestHandler<EnviarEmailBoasVindasRequest, IResult<Unit, Error>>
    {
        private readonly IUsuarioEmailService _usuarioEmailService;

        public EnviarEmailBoasVindasRequestHandler(IUnitOfWork unitOfWork, IUsuarioEmailService usuarioEmailService) : base(unitOfWork)
        {
            _usuarioEmailService = usuarioEmailService;
        }

        public async Task<IResult<Unit, Error>> Handle(EnviarEmailBoasVindasRequest request, CancellationToken cancellationToken)
        {
            var result = await _usuarioEmailService.EnviarBoasVindasAsync(request.Email, cancellationToken);

            return result.IsSuccess ? Result.Success<Unit, Error>(Unit.Value) : Result.Failure<Unit, Error>(result.Error);
        }
    }
}
