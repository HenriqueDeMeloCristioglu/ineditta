using CSharpFunctionalExtensions;

using Ineditta.Application.CalendarioSindicais.Eventos.Services;
using Ineditta.BuildingBlocks.Core.Database;
using Ineditta.BuildingBlocks.Core.Domain.Handlers;
using Ineditta.BuildingBlocks.Core.Domain.Models;

using MediatR;

namespace Ineditta.Application.CalendarioSindicais.Eventos.UseCases.Gerar
{
    public sealed class GerarCalendarioSindicalLoteRequestHandler : BaseCommandHandler, IRequestHandler<GerarCalendarioSindicalLoteRequest, Result<Unit, Error>>
    {
        private readonly ICalendarioSindicalGeradorService _calendarioSindicalGeradorService;
        public GerarCalendarioSindicalLoteRequestHandler(IUnitOfWork unitOfWork, ICalendarioSindicalGeradorService calendarioSindicalGeradorService) : base(unitOfWork)
        {
            _calendarioSindicalGeradorService = calendarioSindicalGeradorService;
        }

        public async Task<Result<Unit, Error>> Handle(GerarCalendarioSindicalLoteRequest request, CancellationToken cancellationToken)
        {
            var result = await _calendarioSindicalGeradorService.GerarAsync(cancellationToken);

            if (result.IsFailure)
            {
                return Result.Failure<Unit, Error>(result.Error);
            }

            await CommitAsync(cancellationToken);

            return Result.Success<Unit, Error>(Unit.Value);
        }
    }
}
