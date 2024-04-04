using Ineditta.API.ViewModels.Shared.Requests;

namespace Ineditta.API.ViewModels.IndicadoresEconomicos.Requests.Home
{
    public class IndicadorEconomicoHomeRequest : FiltroHomeRequest
    {
        public int? NumeroMeses { get; set; }

    }
}
