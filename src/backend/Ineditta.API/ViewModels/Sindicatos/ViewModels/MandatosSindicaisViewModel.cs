using Ineditta.Repository.Models;

namespace Ineditta.API.ViewModels.Sindicatos.ViewModels
{
    public class MandatosSindicaisViewModel
    {
        public IEnumerable<DateOnly>? TerminoMandatosEmps { get; set; }
        public IEnumerable<DateOnly>? TerminoMandatosPatrs { get; set; }
        public int MandatosVigentes { get; set; }
        public int MandatosVencidos { get; set; }
        public int NegVencidas { get; set; }
        public int NegVigentes { get; set; }
        public IEnumerable<NegAbertoEstadoViewModel>? NegAbertoPorEstado { get; set; }
        public IEnumerable<string?>? EstadosComentarios { get; set; }
        public IEnumerable<QuantidadeCentraisSindicaisViewModel>? QtdCentraisSindicais { get; set; }
    }
}
