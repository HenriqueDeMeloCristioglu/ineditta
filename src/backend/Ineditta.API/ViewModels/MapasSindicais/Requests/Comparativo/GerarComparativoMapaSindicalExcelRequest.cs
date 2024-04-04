namespace Ineditta.API.ViewModels.MapasSindicais.Requests.Comparativo
{
    public class GerarComparativoMapaSindicalExcelRequest
    {
        public IEnumerable<int>? DocumentoComparacaoIds { get; set; }
        public int DocumentoReferenciaId { get; set; }
    }
}
