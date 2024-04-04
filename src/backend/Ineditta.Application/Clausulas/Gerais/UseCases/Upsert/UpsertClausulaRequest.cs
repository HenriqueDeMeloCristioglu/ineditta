using CSharpFunctionalExtensions;

using MediatR;

namespace Ineditta.Application.Clausulas.Gerais.UseCases.Upsert
{
    public class UpsertClausulaRequest : IRequest<Result>
    {
#pragma warning disable CA1805 // Do not initialize unnecessarily
        public int Id { get; set; } = 0;
#pragma warning restore CA1805 // Do not initialize unnecessarily
        public string? Texto { get; set; } = null!;
        public int EstruturaClausulaId { get; set; }
        public int DocumentoSindicalId { get; set; }
        public int? Numero { get; set; }
        public int? AssuntoId { get; set; }
        public int? SinonimoId { get; set; }
        public string? TextoResumido { get; set; }
        public bool? ConstaNoDocumento { get; set; }
        public IEnumerable<InformacaoAdicionalItemRequest>? InformacoesAdicionais { get; set; }
    }

    public class InformacaoAdicionalItemRequest
    {
        public int Id { get; set; }
        public string SequenciaLinha { get; set; } = null!;
        public int SequenciaItem { get; set; }
        public int GrupoId { get; set; }
        public int EstruturaId { get; set; }
        public int DocumentoId { get; set; }
        public int Codigo { get; set; }
        public int Nome { get; set; }
        public string? Texto { get; set; }
        public string? Combo { get; set; }
        public string? Hora { get; set; }
        public decimal? Percentual { get; set; }
        public string? Data { get; set; }
        public string? Descricao { get; set; }
        public decimal? Numerico { get; set; }
    }
}
