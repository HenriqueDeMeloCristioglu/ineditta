namespace Ineditta.API.ViewModels.Clausulas.Requests
{
    public class ComparacaoClausulaRequest
    {
        public int? SindeId { get; set; }
        public int? SindpId { get; set; }
        public int? TipoDocIdReferencia { get; set; }
        public int? TipoDocIdAnterior { get; set; }
        public DateOnly? ValidadeInicialReferencia { get; set; }
        public DateOnly? ValidadeFinalReferencia { get; set; }
        public DateOnly? ValidadeInicialAnterior { get; set; }
        public DateOnly? ValidadeFinalAnterior { get; set; }
        public IEnumerable<int>? GrupoClausula { get; set; }
        public IEnumerable<int>? EstruturaClausula { get; set; }
        public bool ExibirDiferencas { get; set; }
        public int DocumentoReferenciaId { get; set; }
        public int DocumentoAnteriorId { get; set; }
    }
}
