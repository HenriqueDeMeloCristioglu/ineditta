using CSharpFunctionalExtensions;

using MediatR;

namespace Ineditta.Application.Clausulas.Gerais.UseCases.Resumir
{
    public class ResumirClausulaRequest : IRequest<Result>
    {
        public int DocumentoId { get; set; }
    }
}
