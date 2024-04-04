using Ineditta.Application.BasesTerritoriaisPatronais.Entities;

namespace Ineditta.Application.BasesTerritoriaisPatronais.Repositories
{
    public interface IBaseTerritorialRepository
    {
        ValueTask IncluirAsync(BaseTerritorialPatronal baseTerritorialPatronal);
        ValueTask AtualizarAsync(BaseTerritorialPatronal baseTerritorialPatronal);
        ValueTask<BaseTerritorialPatronal?> ObterPorIdAsync(int id);
        ValueTask<IEnumerable<BaseTerritorialPatronal>?> ObterPorSindicatoPatronalIdAsync(int id);
        ValueTask<IEnumerable<BaseTerritorialPatronal>?> ObterVigentesPorSindicatoPatronalIdAsync(int id);
    }
}
