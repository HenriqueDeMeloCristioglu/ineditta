using Ineditta.Application.Documentos.Sindicais.Dtos;

namespace Ineditta.Repository.EventosCalendario.Views.EventosCalendariosVencimentosDocumentos
{
    public class EventoCalendarioVencimentoDocumentoVw
    {
        public DateOnly Data { get; set; }
        public string? TipoEvento { get; set; }
        public string? Origem { get; set; }
        public int? TipoDocId { get; set; }
        public string? TipoDocNome { get; set; }
        public DateOnly? ValidadeInicial { get; set; }
        public DateOnly? ValidadeFinal { get; set; }
        public string? AtividadesEconomicas { get; set; }
        public IEnumerable<SindicatoLaboral>? SindicatosLaborais { get; set; }
        public IEnumerable<SindicatoPatronal>? SindicatosPatronais { get; set; }
        public int? ChaveReferenciaId { get; set; }
    }
}
