using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace Ineditta.Application.CalendarioSindicais.Eventos.Dtos
{
    public class ReuniaoSindicalViewModel : AssembleiaReuniaoViewModelBase
    {
        public string? TipoEvento { get; set; }
    }
}
