using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ineditta.Repository.ClientesUnidades.Views
{
    public class EstabelecimentosVw
    {
        public int Id { get; set; }
        public string? Nome { get; set; }
        public string? Filial { get; set; }
        public string? Grupo { get; set; }
        public string? Cnpj { get; set; }
        public string? NomeGrupoEconomico { get; set;}
        public int? ClienteGrupoIdGrupoEconomico { get; set; }
    }
}
