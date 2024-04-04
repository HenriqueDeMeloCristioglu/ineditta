using Ineditta.Repository.Models;

using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Ineditta.Repository.EventosCalendario.Views.EventosCalendariosVencimentosMandatosPatronais
{
    public class EventoCalendarioVencimentoMandatoPatronalVw
    {
        public DateOnly Data { get; set; }
        public string? Origem { get; set; }
        public DateOnly? ValidadeInicial { get; set; }
        public DateOnly? ValidadeFinal { get; set; }
        public string? Dirigente { get; set; }
        public string? Funcao { get; set; }
        public string? AtividadesEconomicas { get; set; }
        public string? SiglasSindicatosPatronais { get; set; }
        public string? SindicatoLaboralId { get; set; }
        public string? SindicatoPatronalId { get; set; }
        public int? SindId { get; set; }
    }
}