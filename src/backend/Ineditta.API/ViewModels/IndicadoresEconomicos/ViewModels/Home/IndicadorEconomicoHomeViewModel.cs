namespace Ineditta.API.ViewModels.IndicadoresEconomicos.ViewModels.Home
{
    public class IndicadorEconomicoHomeViewModel
    {
        public int Tipo { get; set; }
        public string Indicador { get; set; } = null!;
        public string? Fonte { get; set; }

        public IEnumerable<IndicadorEconomicoHomeItemViewModel>? Indices { get; set; }

        public static IEnumerable<IndicadorEconomicoHomeViewModel> Converter(IEnumerable<IndicadorEconomicoHomeTipoViewModel> indicadoresEconomicosTipos)
        {
            return !(indicadoresEconomicosTipos?.Any() ?? false)
                ? Enumerable.Empty<IndicadorEconomicoHomeViewModel>()
                : (from iet in indicadoresEconomicosTipos
                   group iet by new { iet.Tipo, Indicador = iet.Indicador!.ToUpperInvariant() } into g
                   select new IndicadorEconomicoHomeViewModel
                   {
                       Tipo = g.Key.Tipo,
                       Indicador = g.Key.Indicador,
                       Fonte = g.FirstOrDefault(item => !string.IsNullOrEmpty(item.Fonte))?.Fonte,
                       Indices = g.Select(item => new IndicadorEconomicoHomeItemViewModel
                       {
                           Periodo = item.Periodo,
                           Indice = item.Indice
                       })
                   });
        }
    }

    public class IndicadorEconomicoHomeItemViewModel
    {
        public DateOnly Periodo { get; set; }
        public decimal Indice { get; set; }
    }
}
