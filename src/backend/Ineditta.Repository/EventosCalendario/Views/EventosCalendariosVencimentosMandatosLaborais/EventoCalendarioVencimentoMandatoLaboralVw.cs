using Ineditta.Repository.Models;

using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Ineditta.Repository.EventosCalendario.Views.EventosCalendariosVencimentosMandatosLaborais
{
    public class EventoCalendarioVencimentoMandatoLaboralVw
    {
        public DateOnly Data { get; set; }
        public string? Origem { get; set; }
        public DateOnly? ValidadeInicial { get; set; }
        public DateOnly? ValidadeFinal { get; set; }
        public string? Dirigente { get; set; }
        public string? Funcao { get; set; }
        public string? AtividadesEconomicas { get; set; }
        public string? SiglasSindicatosLaborais { get; set; }
        public int? SindId { get; set; }
    }
}