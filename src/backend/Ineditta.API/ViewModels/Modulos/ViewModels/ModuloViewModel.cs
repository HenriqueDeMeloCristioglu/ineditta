using Ineditta.Application.Modulos.Entities;
using Ineditta.BuildingBlocks.Core.Extensions;
using Ineditta.BuildingBlocks.Core.Web.API.Datatables.Attributes;

namespace Ineditta.API.ViewModels.Modulos.ViewModels
{
    public class ModuloViewModel
    {
        public int Id { get; set; }
        public TipoModulo TipoId { get; set; }
        public string? Tipo => TipoId.GetDescription();
        public string? Nome { get; set; }
        [NotSearchableDataTable]
        public bool Criar { get; set; }
        [NotSearchableDataTable]
        public bool Consultar { get; set; }
        [NotSearchableDataTable]
        public bool Comentar { get; set; }
        [NotSearchableDataTable]
        public bool Alterar { get; set; }
        [NotSearchableDataTable]
        public bool Excluir { get; set; }
        [NotSearchableDataTable]
        public bool Aprovar { get; set; }
        [NotSearchableDataTable]
        public string? Uri { get; set; }
    }
}
