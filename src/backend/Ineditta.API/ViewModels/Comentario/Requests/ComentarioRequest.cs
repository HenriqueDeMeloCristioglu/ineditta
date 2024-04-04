using Ineditta.BuildingBlocks.Core.Web.API.Datatables;

namespace Ineditta.API.ViewModels.Comentario.Requests
{
    public class ComentarioRequest : DataTableRequest
    {
        public bool? PorUsuario { get; set; }
    }
}
