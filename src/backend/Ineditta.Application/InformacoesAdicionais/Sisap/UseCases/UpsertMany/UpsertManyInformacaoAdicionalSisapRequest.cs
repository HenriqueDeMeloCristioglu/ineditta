using CSharpFunctionalExtensions;

using MediatR;

namespace Ineditta.Application.InformacoesAdicionais.Sisap.UseCases.UpsertMany
{
    public class UpsertManyInformacaoAdicionalSisapRequest : IRequest<Result>
    {
#pragma warning disable CA1805 // Do not initialize unnecessarily
        public int ClausulaId { get; set; } = 0;
#pragma warning restore CA1805 // Do not initialize unnecessarily
        public IEnumerable<InformacaoAdicionalSisapRequestItem> InformacoesAdicionais { get; set; } = null!;
    }

    public class InformacaoAdicionalSisapRequestItem
    {
        public int Id { get; set; }
        public int DocumentoSindicalId { get; set; }
        public int ClausulaGeralId { get; set; }
        public int EstruturaClausulaId { get; set; }
        public int NomeInformacaoEstruturaClausulaId { get; set; }
        public int TipoinformacaoadicionalId { get; set; }
        public int InforamcacaoAdicionalGrupoId { get; set; }
        public int SequenciaItem { get; set; }
        public int SequenciaLinha { get; set; }
        public string? Texto { get; set; }
        public decimal? Numerico { get; set; }
        public string? Descricao { get; set; }
        public string? Data { get; set; }
        public decimal? Percentual { get; set; }
        public string? Hora { get; set; }
        public string? Combo { get; set; }
    }
}
