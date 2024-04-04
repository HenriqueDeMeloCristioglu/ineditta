using Ineditta.BuildingBlocks.Core.Web.API.Datatables;

namespace Ineditta.API.ViewModels.Usuariosadm.Requests
{
    public class UsuariosPorGrupoEconomicoRequest : DataTableRequest
    {
        public IEnumerable<int>? EstabelecimentosIds { get; set; }
        public bool FiltrarUsuariosAceitamNotificacaoEmail { get; set; }
        public IEnumerable<int>? SindicatosLaboraisIds { get; set; }
        public IEnumerable<int>? SindicatosPatronaisIds { get; set; }
    }
}
