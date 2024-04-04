using Ineditta.BuildingBlocks.Core.Web.API.Datatables;

namespace Ineditta.API.ViewModels.MapasSindicais.Requests.GerarExcel
{
    public class GerarExelRequest : DataTableRequest
    {
        public IEnumerable<int>? GrupoEconomicoIds { get; set; }
        public IEnumerable<int>? EmpresaIds { get; set; }
        public IEnumerable<int>? UnidadeIds { get; set; }
        public IEnumerable<int>? TipoDocIds { get; set; }
        public IEnumerable<int>? CnaeIds { get; set; }
        public IEnumerable<int>? LocalidadeIds { get; set; }
        public IEnumerable<string>? Ufs { get; set; }
        public IEnumerable<int>? SindLaboralIds { get; set; }
        public IEnumerable<int>? SindPatronalIds { get; set; }
        public IEnumerable<string>? DataBases { get; set; }
        public IEnumerable<int>? GrupoClausulaIds { get; set; }
        public IEnumerable<int>? EstruturaClausulaIds { get; set; }
        public DateTimeOffset? VigenciaInicial { get; set; }
        public DateTimeOffset? VigenciaFinal { get; set; }
        public DateTimeOffset? ProcessamentoInedittaInicial { get; set; }
        public DateTimeOffset? ProcessamentoInedittaFinal { get; set; }
        public IEnumerable<int>? DocumentoIds { get; set; }
        public string? PalavraChave { get; set; }
    }
}
