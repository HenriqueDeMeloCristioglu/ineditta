using CSharpFunctionalExtensions;

using Ineditta.BuildingBlocks.Core.Domain.ValueObjects;

using MediatR;

namespace Ineditta.Application.ClientesUnidades.UseCases
{
    public class UpsertClienteUnidadeRequest: IRequest<Result>
    {
        public int Id { get; set; }
        public required string Codigo { get; set; }
        public required string Nome { get; set; }
        public required string Cnpj { get; set; }

        public string? Endereco { get; set; }
        public string? Regiao { get; set; }
        public string? Bairro { get; set; }
        public required string Cep { get; set; }
        public DateOnly? DataAusencia { get; set; }
        public string? CodigoSindicatoCliente { get; set; }
        public string? CodigoSindicatoPatronal { get; set; }
        public int EmpresaId { get; set; }
        public int TipoNegocioId { get; set; }
        public int LocalizacaoId { get; set; }
        public int? CnaeFilial { get; set; }

        public IEnumerable<CnaeUnidadeRequest>? CnaesUnidade { get; set; }
    }

    public class CnaeUnidadeRequest
    {
        public int Id { get; set; }
    }
}
