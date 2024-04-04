using CSharpFunctionalExtensions;

using MediatR;

namespace Ineditta.Application.Acompanhamentos.Ccts.UseCases.AdicionarScript
{
    public class AdicionarScriptRequest : IRequest<Result>
    {
        public IEnumerable<long> AcompanhamentosCctsIds { get; set; } = null!;
        public long FaseId { get; set; }
        public IEnumerable<string> Respostas { get; set; } = null!;
        public long StatusId { get; set; }
    }
}
