using Ineditta.BuildingBlocks.Core.Web.API.Datatables;

namespace Ineditta.API.ViewModels.Sindicatos
{
    public class InfoDirigentesRequest : DataTableRequest
    {
        public int? SindId { get; set; }
        public string? TipoSind { get; set; }
    }
}
