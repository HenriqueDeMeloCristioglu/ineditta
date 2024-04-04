using System.Text.Json.Serialization;

using Newtonsoft.Json;

namespace Ineditta.Application.CalendarioSindicais.Eventos.Dtos
{
    public class AssembleiaPatronalViewModel : AssembleiaReuniaoViewModelBase
    {
        public string? TipoEvento { get; set; }
    }
}
