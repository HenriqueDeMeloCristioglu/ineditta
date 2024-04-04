using CSharpFunctionalExtensions;

using Ineditta.BuildingBlocks.Core.Domain.Models;

using MediatR;

namespace Ineditta.Application.CalendarioSindicais.Eventos.UseCases.Notificar
{
    public class NotificarCalendarioSindicalRequest : IRequest<Result<Unit, Error>>
    {
        public NotificarCalendarioSindicalRequest()
        {
            DataExecucao = DateTimeOffset.Now;
        }

        public DateTimeOffset DataExecucao { get; set; }
    }
}
