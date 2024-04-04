using Ineditta.API.ViewModels.SindicatosLaborais.ViewModels;
using Ineditta.API.ViewModels.SindicatosPatronais.ViewModels;

namespace Ineditta.API.ViewModels.Sindicatos.ViewModels
{
    public class SindicatosViewModel
    {
        public IEnumerable<SindicatoLaboralViewModel> SindicatosLaborais { get; set; } = new List<SindicatoLaboralViewModel>();
        public IEnumerable<SindicatoPatronalViewModel> SindicatosPatronais { get; set; } = new List<SindicatoPatronalViewModel>();
        public MandatosSindicaisViewModel? MandatosSindicais { get; set; }
    }
}
