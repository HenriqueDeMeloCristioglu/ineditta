using CSharpFunctionalExtensions;

using Ineditta.Application.AIs.DocumentosSindicais.Entities;

using MediatR;

namespace Ineditta.Application.AIs.DocumentosSindicais.UseCases.Upsert
{
    public class UpsertDocumentoSindicalIARequest : IRequest<Result>
    {
#pragma warning disable CA1805 // Do not initialize unnecessarily
        public int Id { get; set; } = 0;
#pragma warning restore CA1805 // Do not initialize unnecessarily
        public int DocumentoReferenciaId { get; set; }
        public IADocumentoStatus Status { get; set; }
        public string? MotivoErro { get; set; }
    }
}
