using CSharpFunctionalExtensions;

using MediatR;

namespace Ineditta.Application.InformacoesAdicionais.Cliente.UseCases
{
    public class UpsertInformacaoAdicionalClienteRequest : IRequest<Result>
    {
        public int DocumentoSindicalId { get; set; }
        public IEnumerable<InformacaoAdicionalRequest>? InformacoesAdicionais { get; set; }
        public IEnumerable<ObservacaoAdicionalRequest>? ObservacoesAdicionais { get; set; }
        public string? Orientacao { get; set; }
        public string? OutrasInformacoes { get; set; }
    }

    public class InformacaoAdicionalRequest
    {
        public int Id { get; set; }
        public required string Valor { get; set; }
    }

    public class ObservacaoAdicionalRequest
    {
        public int Id { get; set; }
        public required string Valor { get; set; }
        public int Tipo { get; set; }
    }
}
