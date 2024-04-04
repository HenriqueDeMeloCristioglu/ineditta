using Ineditta.BuildingBlocks.Core.Web.API.Datatables;

using Microsoft.AspNetCore.Mvc;

namespace Ineditta.API.ViewModels.Localizacoes.Requests
{
    public class LocalizacaoRequest : DataTableRequest
    {
        public bool PorUsuario { get; set; }
        public bool PorGrupoDoUsuario { get; set; }
        public IEnumerable<int>? GruposEconomicosIds { get; set; }
        public IEnumerable<int>? MatrizesIds { get; set; }
        public IEnumerable<int>? ClientesUnidadesIds { get; set; }
        public int? GrupoEconomicoId { get; set; }
        public string? TipoLocalidade { get; set; }
        public bool LocalizacoesPorAcompanhamentos { get; set; }
    }
}
