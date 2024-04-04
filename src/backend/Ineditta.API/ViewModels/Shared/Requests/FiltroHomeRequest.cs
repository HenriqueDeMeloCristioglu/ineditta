using Ineditta.BuildingBlocks.Core.Web.API.Datatables;

#pragma warning disable CA1716 // Identifiers should not match keywords
namespace Ineditta.API.ViewModels.Shared.Requests
{
    public class FiltroHomeRequest
    {
        public IEnumerable<int>? UnidadesIds { get; set; }
        public IEnumerable<int>? MatrizesIds { get; set; }
        public IEnumerable<int>? GruposEconomicosIds { get; set; }
        public IEnumerable<int>? CnaesIds { get; set; }
        public int? NomeDocNegId { get; set; }
        public string? FaseDocNeg { get; set; }
        public string? AnoNeg { get; set; }
    }

    public class FiltroHomeDatatableRequest : DataTableRequest
    {
        public IEnumerable<int>? UnidadesIds { get; set; }
        public IEnumerable<int>? MatrizesIds { get; set; }
        public IEnumerable<int>? GruposEconomicosIds { get; set; }
        public IEnumerable<int>? CnaesIds { get; set; }
    }
}
#pragma warning restore CA1716 // Identifiers should not match keywords