using Ineditta.Application.Sinonimos.Entities;

namespace Ineditta.Application.Sinonimos.Repositories
{
    public interface ISinonimoRepository
    {
        ValueTask<Sinonimo?> ObterPorEstruturaClausulaIdENomeAsync(int estruturaClausulaId, string nome);
        ValueTask IncluirAsync(Sinonimo sinonimo);
        ValueTask AtualizarAsync(Sinonimo sinonimo);
        ValueTask<Sinonimo?> ObterPorIdAsync(long id);
        ValueTask<IEnumerable<Sinonimo>?> ObterPorIdEstruturaClausulaAsync(long id, long estruturaClausulaId);
        ValueTask<Sinonimo?> ObterPorNomeExatoAsync(string nome);
        ValueTask<Sinonimo?> ObterPorNomeAproximadoAsync(string nome);
    }
}