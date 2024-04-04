using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Ineditta.Repository.EventosCalendario.Views.EventosCalendarios
{
    public class EventoCalendarioVw
    {
        public DateOnly Data { get; set; }
        public string? TipoEvento { get; set; }
        public string? Origem { get; set; }
        public int? TipoDocId { get; set; }
        public string? TipoDocNome { get; set; }
        public DateOnly? ValidadeInicial { get; set; }
        public DateOnly? ValidadeFinal { get; set; }
        public string? AtividadesEconomicas { get; set; }
        public string? SiglasSindicatosLaborais { get; set; }
        public string? SiglasSindicatosPatronais { get; set; }
    }
}