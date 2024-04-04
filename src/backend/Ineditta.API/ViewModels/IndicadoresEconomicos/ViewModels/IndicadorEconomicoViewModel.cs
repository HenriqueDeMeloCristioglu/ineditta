namespace Ineditta.API.ViewModels.IndicadoresEconomicos.ViewModels
{
    public class IndicadorEconomicoViewModel
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public int Id { get; set; }
        public string Origem { get; set; }
        public string Indicador { get; set; }
        public int IdUsuario { get; set; }
        public string? Fonte { get; set; }
        public string Data { get; set; }
        public float? DadoProjetado { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    }
}
