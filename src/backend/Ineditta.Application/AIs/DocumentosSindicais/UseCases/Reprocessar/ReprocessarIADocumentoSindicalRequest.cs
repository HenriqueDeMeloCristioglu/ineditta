using CSharpFunctionalExtensions;
using MediatR;

namespace Ineditta.Application.AIs.DocumentosSindicais.UseCases.Reprocessar
{
    public class ReprocessarIADocumentoSindicalRequest : IRequest<Result>
    {
        public int DocumentoId { get; set; }
    }
}
