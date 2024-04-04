using CSharpFunctionalExtensions;
using MediatR;

namespace Ineditta.Application.AIs.DocumentosSindicais.UseCases.Aprovar
{
    public class AprovarIADocumentoSindicalRequest : IRequest<Result>
    {
        public int DocumentoId { get; set; }
    }
}
