namespace Ineditta.API.ViewModels.IndicadoresEconomicos.ViewModels.Home
{
    public class IndicadorEconomicoHomeTipoViewModel
    {
        public int Tipo { get; set; }
        public string Indicador { get; set; } = null!;
        public DateOnly Periodo { get; set; }
        public decimal Indice { get; set; }
        public string? Fonte { get; set; }
    }
}
