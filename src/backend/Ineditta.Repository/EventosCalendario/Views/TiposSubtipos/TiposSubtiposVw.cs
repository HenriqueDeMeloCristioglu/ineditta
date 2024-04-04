using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ineditta.Repository.EventosCalendario.Views.TiposSubtipos
{
    public class TiposSubtiposVw
    {
        public int Id { get; set; }
        public string Nome { get; set; } = null!;
        public int? TipoAssociado { get; set; }
    }
}
