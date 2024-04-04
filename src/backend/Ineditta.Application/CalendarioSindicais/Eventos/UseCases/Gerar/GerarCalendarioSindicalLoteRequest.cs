using CSharpFunctionalExtensions;

using Ineditta.BuildingBlocks.Core.Domain.Models;

using MediatR;

namespace Ineditta.Application.CalendarioSindicais.Eventos.UseCases.Gerar
{
    public class GerarCalendarioSindicalLoteRequest : IRequest<Result<Unit, Error>>
    {
        public GerarCalendarioSindicalLoteRequest()
        {
            DataExecucao = DateTimeOffset.Now;
        }

        public DateTimeOffset DataExecucao { get; set; }
    }
}