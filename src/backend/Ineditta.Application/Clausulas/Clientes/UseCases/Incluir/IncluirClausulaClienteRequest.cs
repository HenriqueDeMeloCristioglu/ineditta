using CSharpFunctionalExtensions;

using MediatR;

namespace Ineditta.Application.Clausulas.Clientes.UseCases.Upsert
{
    public class IncluirClausulaClienteRequest : IRequest<Result>
    {
        public int ClausulaId { get; set; }
        public string Texto { get; set; } = null!;
    }
}
