using Ineditta.Application.Clausulas.Gerais.UseCases.Upsert;

namespace Ineditta.API.ViewModels.ClausulasGerais.Requests
{
    public class IncluirClausulasGeraisRequest
    {
        public string Texto { get; set; } = null!;
        public int EstruturaId { get; set; }
        public int DocumentoId { get; set; }
        public int Numero { get; set; }
        public int? AssuntoId { get; set; }
        public int? SinonimoId { get; set; }
        public IEnumerable<InformacaoAdicionalItemRequest>? InformacoesAdicionais { get; set; }
    }
}
