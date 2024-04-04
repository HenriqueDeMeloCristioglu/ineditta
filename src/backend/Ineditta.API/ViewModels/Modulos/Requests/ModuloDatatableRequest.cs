using Ineditta.BuildingBlocks.Core.Web.API.Datatables;

namespace Ineditta.API.ViewModels.Modulos.Requests
{
    public class ModuloDatatableRequest : DataTableRequest
    {
        public string? Nome { get; set; }
        public string? Tipo { get; set; }
        public bool Criar { get; set; }
        public bool Consultar { get; set; }
        public bool Comentar { get; set; }
        public bool Alterar { get; set; }
        public bool Exlcluir { get; set; }
        public bool Aprovar { get; set; }
        public string? Uri { get; set; }
    }
}
