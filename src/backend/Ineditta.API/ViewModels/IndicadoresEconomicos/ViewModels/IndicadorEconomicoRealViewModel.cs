namespace Ineditta.API.ViewModels.IndicadoresEconomicos.ViewModels
{
    public class IndicadorEconomicoRealViewModel
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public int Id { get; set; }
        public string Indicador { get; set; }
        public float DadoReal { get; set; }
        public DateOnly? PeriodoData { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    }
}
