using CSharpFunctionalExtensions;

using MediatR;

namespace Ineditta.Application.Documentos.Sindicais.UseCases.AtualizarDataSla
{
    public class AtualizarDataSlaRequest : IRequest<Result>
    {
        public long? DocSindId { get; set; }
        public DateOnly NovaDataSla { get; set; }
    }
}
