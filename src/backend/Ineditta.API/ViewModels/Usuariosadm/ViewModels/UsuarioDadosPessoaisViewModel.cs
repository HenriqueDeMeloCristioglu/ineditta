using Ineditta.Application.Usuarios.Entities;

namespace Ineditta.API.ViewModels.Usuariosadm.ViewModels
{
    public class UsuarioDadosPessoaisViewModel
    {
        public long Id { get; set; }
        public Nivel Nivel { get; set; }
        public int? GrupoEconomicoId { get; set; }
        public string? LogoGrupoEconomico { get; set; }
    }
}
