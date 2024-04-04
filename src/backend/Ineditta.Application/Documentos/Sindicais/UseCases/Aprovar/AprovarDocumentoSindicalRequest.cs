using CSharpFunctionalExtensions;

using MediatR;

namespace Ineditta.Application.Documentos.Sindicais.UseCases.Aprovar
{
    public class AprovarDocumentoSindicalRequest : IRequest<Result>
    {
        public long Id { get; set; }
    }
}
