using CSharpFunctionalExtensions;

using MediatR;

namespace Ineditta.Application.Documentos.Sindicais.UseCases.IniciarScrap
{
    public class IniciarScrapDocumentoSindicalRequest : IRequest<Result>
    {
        public int DocumentoId { get; set; }
    }
}
