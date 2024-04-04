using Ineditta.BuildingBlocks.Core.Web.API.Datatables;

namespace Ineditta.API.ViewModels.EventosCalendario
{
    public class EventoCalendarioRequest: DataTableRequest
    {
        public string? TipoEvento { get; set; }
        public IEnumerable<int>? NomesDocumentosIds { get; set; }
        public IEnumerable<int>? AtividadesEconomicasIds { get; set; }
        public IEnumerable<int>? GruposEconomicosIds { get; set; }
        public IEnumerable<int>? MatrizesIds { get; set; }
        public IEnumerable<int>? EstabelecimentosIds { get; set; }
        public IEnumerable<int>? SindicatosLaboraisIds { get; set; }
        public IEnumerable<int>? SindicatosPatronaisIds { get; set; }
        public DateTime? DataInicial { get; set; }
        public DateTime? DataFinal { get; set; }
        public IEnumerable<int>? GruposAssuntoIds { get; set; }
        public IEnumerable<int>? AssuntosIds { get; set; }
        public IEnumerable<int>? Origem { get; set; }
        public IEnumerable<int>? MunicipiosIds { get; set; }
        public IEnumerable<string>? Ufs { get; set; }
    }
}
