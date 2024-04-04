using CSharpFunctionalExtensions;

using MediatR;

namespace Ineditta.Application.AIs.Clausulas.UseCases.Delete
{
    public class DeleteIAClausulaRequest : IRequest<Result>
    {
        public int Id { get; set; }
    }
}
