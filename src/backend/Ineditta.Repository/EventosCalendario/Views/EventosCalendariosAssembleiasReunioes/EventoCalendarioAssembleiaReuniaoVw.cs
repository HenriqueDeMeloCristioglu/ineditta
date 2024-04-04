using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ineditta.Repository.EventosCalendario.Views.EventosCalendariosAssembleiasReunioes
{
    public class EventoCalendarioAssembleiaReuniaoVw
    {
        public DateTime Data { get; set; }
        public string? TipoEvento { get; set; }
        public string? Origem { get; set; }
        public int? TipoDocId { get; set; }
        public string? TipoDocNome { get; set; }
        public IEnumerable<string>? AtividadesEconomicas { get; set; }
        public string? DataBaseNegociacao { get; set; }
        public string? Fase { get; set; }
        public int? ChaveReferenciaId { get; set; }
        public string? DescricoesSubclasse { get; set; }
        public int? TipoEventoId { get; set; }
        public IEnumerable<SindicatoEvento>? SindicatosLaborais { get; set; }
        public IEnumerable<SindicatoEvento>? SindicatosPatronais { get; set; }
    }

    public class SindicatoEvento
    {
        public int Id { get; set; }
        public string Sigla { get; set; }
    }
}
