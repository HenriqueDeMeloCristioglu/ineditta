using CSharpFunctionalExtensions;

using MediatR;

namespace Ineditta.Application.Clausulas.Gerais.UseCases.Aprovar
{
    public class AprovarClausulaGeralRequest : IRequest<Result>
    {
        public int Id { get; set; }
    }
}
