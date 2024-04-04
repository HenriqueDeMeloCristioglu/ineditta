using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ineditta.Repository.Usuarios.Views
{
    public class UsuariosAdmsVw
    {
        public long Id { get; set; }
        public string? Nome { get; set; }
        public string? Email { get; set; }
        public string? Cargo { get; set; }
        public string? Telefone { get; set; }
        public string? Ramal { get; set; }
        public string? Departamento { get; set; }
        public int? IdSuperior { get; set; }
        public DateTime? DataCriacao { get; set; }
        public string? NomeUserCriador { get; set; }
        public int? IdGrupoEconomico { get; set; }
        public int[]? IdsFmge { get; set; }
        public string? Nivel { get; set; }
    }
}
