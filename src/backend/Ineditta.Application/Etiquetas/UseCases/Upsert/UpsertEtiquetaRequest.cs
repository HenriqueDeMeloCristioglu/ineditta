using CSharpFunctionalExtensions;

using Ineditta.Application.Etiquetas.Entities;

using MediatR;

namespace Ineditta.Application.Etiquetas.UseCases.Upsert
{
    public class UpsertEtiquetaRequest : IRequest<Result>
    {
        public int Id { get; set; }
        public string Nome { get; set; } = null!;
        public long TipoEtiquetaId { get; set; }
    }
}
