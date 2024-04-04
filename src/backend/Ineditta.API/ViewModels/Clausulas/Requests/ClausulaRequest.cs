using Ineditta.BuildingBlocks.Core.Web.API.Datatables;

namespace Ineditta.API.ViewModels.Clausulas.Requests
{
    public class ClausulaRequest : DataTableRequest
    {
        public IEnumerable<int>? GrupoEconomicoIds { get; set; }
        public IEnumerable<int>? EmpresaIds { get; set; }
        public IEnumerable<int>? UnidadeIds { get; set; }
        public IEnumerable<int>? TipoDocIds { get; set; }
        public IEnumerable<int>? CnaeIds { get; set; }
        public IEnumerable<int>? MunicipiosIds { get; set; }
        public IEnumerable<string>? Ufs { get; set; }
        public IEnumerable<int>? SindLaboralIds { get; set; }
        public IEnumerable<int>? SindPatronalIds { get; set; }
        public string? TipoDataBase { get; set; }
        public string? DataBase { get; set; }
        public IEnumerable<int>? GrupoClausulaIds { get; set; }
        public IEnumerable<int>? EstruturaClausulaIds { get; set; }
        public DateTimeOffset? VigenciaInicial { get; set; }
        public DateTimeOffset? VigenciaFinal { get; set; }
        public IEnumerable<int>? DocumentoIds { get; set; }
        public string? PalavraChave { get; set; }
        public int? IdDoc { get; set; }
        public DateOnly? DataProcessamentoInicial { get; set; }
        public DateOnly? DataProcessamentoFinal { get; set; }
        public bool ClausulaGrupoEconomico { get; set; }
        public bool DataProcessamentoDocumento { get; set; }
        public bool NaoConstaNoDocumento { get; set; }
        public bool Resumivel { get; set; }
    }
}
