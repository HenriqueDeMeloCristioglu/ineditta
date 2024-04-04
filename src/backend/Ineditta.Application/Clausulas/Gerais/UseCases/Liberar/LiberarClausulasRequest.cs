using CSharpFunctionalExtensions;

using MediatR;

namespace Ineditta.Application.Clausulas.Gerais.UseCases.Liberar
{
    public class LiberarClausulasRequest : IRequest<Result>
    {
        public int DocumentoId { get; set; }
    }
}
