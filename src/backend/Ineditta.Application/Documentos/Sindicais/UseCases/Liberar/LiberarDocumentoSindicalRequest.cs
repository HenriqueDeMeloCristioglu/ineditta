using CSharpFunctionalExtensions;

using MediatR;

namespace Ineditta.Application.Documentos.Sindicais.UseCases.Liberar
{
    public class LiberarDocumentoSindicalRequest : IRequest<Result>
    {
        public long Id { get; set; }
    }
}
