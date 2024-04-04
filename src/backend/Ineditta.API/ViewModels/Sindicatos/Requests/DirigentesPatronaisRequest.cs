using Ineditta.BuildingBlocks.Core.Web.API.Datatables;

namespace Ineditta.API.ViewModels.Sindicatos
{
    public class DirigentesPatronaisRequest : DataTableRequest
    {
        public int? GrupoEconomicoId { get; set; }
        public IEnumerable<int>? MatrizesIds { get; set; }
        public IEnumerable<int>? ClientesUnidadesIds { get; set; }
        public IEnumerable<int>? LocalizacoesIds { get; set; }
        public IEnumerable<string>? Regioes { get; set; }
        public IEnumerable<string>? Ufs { get; set; }
        public IEnumerable<int>? SindLaboraisIds { get; set; }
        public IEnumerable<int>? SindPatronaisIds { get; set; }
        public IEnumerable<string>? DataBase { get; set; }
        public string? Uf { get; set; }
        public IEnumerable<int>? CnaesIds { get; set; }
        public bool Filtrar { get; set; }
    }
}
