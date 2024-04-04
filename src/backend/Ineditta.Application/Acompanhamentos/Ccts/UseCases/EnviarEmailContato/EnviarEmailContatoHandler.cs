using CSharpFunctionalExtensions;

using Ineditta.Application.Acompanhamentos.Ccts.Services.AcompanhamentosCctsEmailsServices;
using Ineditta.Application.AcompanhamentosCcts.UseCases.EnviarEmail;
using Ineditta.BuildingBlocks.Core.Database;
using Ineditta.BuildingBlocks.Core.Domain.Handlers;
using Ineditta.BuildingBlocks.Core.Domain.Models;

using MediatR;

namespace Ineditta.Application.AcompanhamentosCcts.UseCases.EnviarEmailSindicato
{
    public class EnviarEmailContatoHandler : BaseCommandHandler, IRequestHandler<EnviarEmailContatoRequest, IResult<Unit, Error>>
    {
        private readonly IAcompanhamentoCctEmailService _acompanhamentoCctEmailService;
        public EnviarEmailContatoHandler(IUnitOfWork unitOfWork, IAcompanhamentoCctEmailService acompanhamentoCctEmailService) : base(unitOfWork)
        {
            _acompanhamentoCctEmailService = acompanhamentoCctEmailService;
        }

        public async Task<IResult<Unit, Error>> Handle(EnviarEmailContatoRequest request, CancellationToken cancellationToken)
        {
            var result = await _acompanhamentoCctEmailService.EnviarEmailContatoAsync(request.Emails, request.Template, request.Assunto, cancellationToken);

            return result.IsSuccess ? Result.Success<Unit, Error>(Unit.Value) : Result.Failure<Unit, Error>(result.Error);
        }
    }
}
