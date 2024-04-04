using Ineditta.Application.IndicadoresEconomicos.Entities;

namespace Ineditta.Application.IndicadoresEconomicos.Repositories
{
    public interface IIndicadorEconomicoRealRepository
    {
        ValueTask IncluirAsync(IndicadorEconomicoReal indicadorEconomico);
        ValueTask<IndicadorEconomicoReal> ObterAsync(int id);
        ValueTask<bool> ExisteAsync(int id);
        ValueTask AtualizarAsync(IndicadorEconomicoReal indicadorEconomico);
    }
}
