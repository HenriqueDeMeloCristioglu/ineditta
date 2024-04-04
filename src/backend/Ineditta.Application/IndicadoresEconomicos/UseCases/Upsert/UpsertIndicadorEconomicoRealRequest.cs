using CSharpFunctionalExtensions;
using MediatR;

namespace Ineditta.Application.IndicadoresEconomicos.UseCases.Upsert
{
    public class UpsertIndicadorEconomicoRealRequest : IRequest<Result>
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public int Id { get; set; }
        public string Indicador { get; set; }
        public IEnumerable<PeriodoReal> PeriodosReais { get; set; }
    }

    public class PeriodoReal
    {
        public DateOnly Periodo { get; set; }
        public float Real { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    }
}
