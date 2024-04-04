using Ineditta.API.ViewModels.Shared.Requests;

namespace Ineditta.API.ViewModels.DocumentosSindicais.Requests
{
    public class NegociacaoAcumuladaRequest : FiltroHomeRequest
    {
        public int AnoInicial { get; set; }
        public int AnoFinal { get; set; }
    }
}
