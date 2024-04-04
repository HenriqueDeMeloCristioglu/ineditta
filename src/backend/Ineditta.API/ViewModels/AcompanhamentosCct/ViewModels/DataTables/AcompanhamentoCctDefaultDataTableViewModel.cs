using Ineditta.BuildingBlocks.Core.Web.API.Datatables.Attributes;

namespace Ineditta.API.ViewModels.AcompanhamentosCct.ViewModels.DataTables
{
    public class AcompanhamentoCctDefaultDataTableViewModel
    {
        public long Id { get; set; }
        public DateOnly DataInicial { get; set; }
        public DateOnly? DataFinal { get; set; }
        [NotSearchableDataTable]
        public DateTime? DataAlteracao { get; set; }
        public DateOnly? UltimaAtualizacao => DataAlteracao != null ? DateOnly.FromDateTime((DateTime)DataAlteracao) : default;
        public string? Status { get; set; }
        public string NomeUsuario { get; set; } = string.Empty;
        public string? Fase { get; set; }
        public required string NomeDocumento { get; set; }
        public DateOnly? ProxLigacao { get; set; }
        public string? DataBase { get; set; }
        public required string SiglaSindPatronal { get; set; }
        public string? UfSindPatronal { get; set; }
        public required string SiglaSindEmpregado { get; set; }
        public string? UfSindEmpregado { get; set; }
        public required string DescricaoSubClasse { get; set; }
        public string? Etiquetas { get; set; }
    }
}
