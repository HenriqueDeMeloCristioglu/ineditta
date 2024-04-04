using CSharpFunctionalExtensions;
using MediatR;

namespace Ineditta.Application.IndicadoresEconomicos.UseCases.Upsert
{
    public class UpsertIndicadorEconomicoRequest : IRequest<Result>
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public int Id { get; set; }
        public string Origem { get; set; }
        public string Indicador { get; set; }
        public int IdUsuario { get; set; }
        public string Fonte { get; set; }
        public IEnumerable<PeriodoProjetado> PeriodosProjetados { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    }

    public class PeriodoProjetado
    {
        public string? Periodo { get; set; }
        public float Projetado { get; set; }
    }
}
