using CSharpFunctionalExtensions;

using Ineditta.Application.AIs.Clausulas.Entities;

using MediatR;

namespace Ineditta.Application.AIs.Clausulas.UseCases.Upsert
{
    public class UpsertIAClausulaRequest : IRequest<Result>
    {
        public int? Id { get; set; }
        public string Texto { get; set; } = null!;
        public int DocumentoSindicalId { get; set; }
        public int EstruturaClausulaId { get; set; }
        public int Numero { get; set; }
        public int SinonimoId { get; set; }
        public IAClausulaStatus? Status { get; set; }
    }
}
