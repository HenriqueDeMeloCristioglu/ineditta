using Ineditta.Application.Sindicatos.Patronais.Entities;
using Ineditta.BuildingBlocks.Core.Domain.ValueObjects;

namespace Ineditta.Application.Sindicatos.Patronais.Repositories
{
    public interface ISindicatoPatronalRepository
    {
        ValueTask IncluirAsync(SindicatoPatronal sindicato);
        ValueTask AtualizarAsync(SindicatoPatronal sindicato);
        ValueTask<SindicatoPatronal?> ObterPorIdAsync(int id);
        ValueTask<IEnumerable<SindicatoPatronal>?> ObterPorListaIdAsync(IEnumerable<int> ids);
        ValueTask<SindicatoPatronal?> ObterPorCNPJAsync(CNPJ cnpj);
        ValueTask<bool> ExisteAsync(CNPJ cnpj, int ignorarId = 0);
        ValueTask<bool> ExistePorIdAsync(int sindicatoPatronalId);
        ValueTask<IEnumerable<SindicatoPatronal>?> ObterPorClienteUnidadesIdsAsync(IEnumerable<int> clientesUnidadesIds);
    }
}