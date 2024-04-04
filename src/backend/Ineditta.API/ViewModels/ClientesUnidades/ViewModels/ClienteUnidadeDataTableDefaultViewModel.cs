using Ineditta.BuildingBlocks.Core.Web.API.Datatables.Attributes;

namespace Ineditta.API.ViewModels.ClientesUnidades.ViewModels
{
    public class ClienteUnidadeDataTableDefaultViewModel
    {
        public int Id { get; set; }
        public string? Nome { get; set; }
        public string? Cnpj { get; set; }
        public DateOnly? DataInativacao { get; set; }
        public DateTime? DataAtivacao { get; set; }
        public string? NomeEstabelecimento { get; set; }
        public string? Municipio { get; set; }
        public string? Uf { get; set; }
        public string? NomeGrupoEconomico { get; set; }
        public string? TipoFilial { get; set; }
        [NotSearchableDataTable]
        public int IdGrupoEconomico { get; set; }
        [NotSearchableDataTable]
        public int IdMatriz { get; set; }
    }
}
