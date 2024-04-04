namespace Ineditta.API.ViewModels.Clausulas.Requests
{
    public class VigenciaSelectRequest
    {
        public int SindeId { get; set; }
        public int SindpId { get; set; }
        public int NomeDocumentoId { get; set; }
        public DateOnly? ValidadeInicialVigenciaReferencia { get; set; }
    }
}
