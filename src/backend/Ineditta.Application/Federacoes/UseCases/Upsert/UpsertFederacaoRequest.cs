using CSharpFunctionalExtensions;

using MediatR;

namespace Ineditta.Application.Federacoes.UseCases.Upsert
{
    public class UpsertFederacaoRequest : IRequest<Result>
    {
        public int Id { get; set; }
        public string? Sigla { get; set; }
        public string? CNPJ { get; set; }
        public string? AreaGeoeconomica { get; set; }
        public string? Telefone { get; set; }
        public string? Grupo { get; set; }
        public string? Grau { get; set; }
    }
}
