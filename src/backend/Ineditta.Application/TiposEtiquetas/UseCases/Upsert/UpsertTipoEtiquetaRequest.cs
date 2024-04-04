using CSharpFunctionalExtensions;

using MediatR;

namespace Ineditta.Application.TiposEtiquetas.UseCases.Upsert
{
    public class UpsertTipoEtiquetaRequest : IRequest<Result>
    {
        public int Id { get; set; }
        public string Nome { get; set; } = null!;
    }
}
