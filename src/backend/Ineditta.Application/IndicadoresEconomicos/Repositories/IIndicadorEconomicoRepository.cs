using Ineditta.Application.IndicadoresEconomicos.Entities;

namespace Ineditta.Application.IndicadoresEconomicos.Repositories
{
    public interface IIndicadorEconomicoRepository
    {
        ValueTask IncluirAsync(IndicadorEconomico indicadorEconomico);
        ValueTask<IndicadorEconomico> ObterAsync(int id);
        ValueTask<bool> ExisteAsync(int id);
        ValueTask AtualizarAsync(IndicadorEconomico indicadorEconomico);
    }
}
