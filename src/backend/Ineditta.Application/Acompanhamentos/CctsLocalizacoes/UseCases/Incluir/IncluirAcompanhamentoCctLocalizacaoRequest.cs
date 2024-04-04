using CSharpFunctionalExtensions;

using MediatR;

namespace Ineditta.Application.Acompanhamentos.CctsLocalizacoes.UseCases.Upsert
{
    public class IncluirAcompanhamentoCctLocalizacaoRequest : IRequest<Result>
    {
        public long AcompanhamentoCctId { get; set; }
        public int LocalizacaoId { get; set; }
    }
}
