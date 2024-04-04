using Ineditta.Application.Cnaes.Entities;

namespace Ineditta.Application.Cnaes.Repositories
{
    public interface ICnaeRepository
    {
        ValueTask IncluirAsync(Cnae cnae);
        ValueTask AtualizarAsync(Cnae cnae);
        ValueTask<Cnae?> ObterPorIdAsync(int id);
        ValueTask<IEnumerable<Cnae>?> ObterPorListaIdAsync(IEnumerable<int> ids);
        ValueTask<IEnumerable<Cnae>?> ObterPorUnidadesIds(IEnumerable<int> unidadesIds);
    }
}
