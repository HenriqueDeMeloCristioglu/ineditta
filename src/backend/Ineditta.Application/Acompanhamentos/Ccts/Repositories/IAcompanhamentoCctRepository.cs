using CSharpFunctionalExtensions;

using Ineditta.Application.Acompanhamentos.Ccts.Entities;

namespace Ineditta.Application.Acompanhamentos.Ccts.Repositories
{
    public interface IAcompanhamentoCctRepository
    {
        ValueTask<Result> IncluirEstabelecimentos();
        ValueTask IncluirAsync(AcompanhamentoCct acompanhamentoCct);
        ValueTask AtualizarAsync(AcompanhamentoCct acompanhamentoCct);
        ValueTask<AcompanhamentoCct?> ObterPorIdAsync(long id);
        ValueTask<IEnumerable<AcompanhamentoCct>?> ObterPorIdsAsync(IEnumerable<long> ids);
    }
}
