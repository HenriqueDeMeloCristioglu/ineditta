using Ineditta.BuildingBlocks.Core.Web.API.Datatables;

namespace Ineditta.API.ViewModels.IAs.DocumentosSindicais.Requests
{
    public class IADocumentoSindicalDataTableRequest : DataTableRequest
    {
        public IEnumerable<int>? GruposEconomicosIds { get; set; }
        public IEnumerable<int>? EmpresasIds { get; set; }
        public IEnumerable<int>? AtividadesEconomicasIds { get; set; }
        public int? GrupoOperacaoId { get; set; }
        public int? DocumentoId { get; set; }
        public int? NomeDocumentoId { get; set; }
        public int? StatusDocumentoId { get; set; }
        public int? StatusClausulasId { get; set; }
        public DateOnly? DataSlaInicio { get; set; }
        public DateOnly? DataSlaFim { get; set; }
    }
}
