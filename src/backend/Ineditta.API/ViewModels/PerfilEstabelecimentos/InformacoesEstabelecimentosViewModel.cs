using Ineditta.Application.Sindicatos.Laborais.Entities;
using Ineditta.Application.Sindicatos.Patronais.Entities;

namespace Ineditta.API.ViewModels.PerfilEstabelecimentos
{
    public class InformacoesEstabelecimentosViewModel
    {
        public string? CodigoEstabelecimento { get; set; }
        public string? NomeEstabelecimento { get; set; }
        public string? CnpjEstabelecimento { get; set; }
        public string? CodigoSindicatoLaboral { get; set; }
        public IEnumerable<SindicatoLaboral>? SindicatosLaborais { get; set; }
        public IEnumerable<SindicatoPatronal>? SindicatosPatronais { get; set; }
    }
}
