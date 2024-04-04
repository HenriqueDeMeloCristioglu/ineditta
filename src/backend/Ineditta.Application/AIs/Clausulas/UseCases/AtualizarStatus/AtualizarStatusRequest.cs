using CSharpFunctionalExtensions;

using Ineditta.Application.AIs.Clausulas.Entities;

using MediatR;

namespace Ineditta.Application.AIs.Clausulas.UseCases.AtualizarStatus
{
    public class AtualizarStatusRequest : IRequest<Result>
    {
        public int Id { get; set; }
        public IAClausulaStatus Status { get; set; }
    }
}
