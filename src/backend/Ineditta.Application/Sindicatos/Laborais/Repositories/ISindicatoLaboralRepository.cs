using Ineditta.Application.Sindicatos.Laborais.Entities;
using Ineditta.BuildingBlocks.Core.Domain.ValueObjects;

namespace Ineditta.Application.Sindicatos.Laborais.Repositories
{
    public interface ISindicatoLaboralRepository
    {
        ValueTask IncluirAsync(SindicatoLaboral sindicato);
        ValueTask AtualizarAsync(SindicatoLaboral sindicato);
        ValueTask<SindicatoLaboral?> ObterPorIdAsync(int id);
        ValueTask<IEnumerable<SindicatoLaboral>?> ObterPorListaIdsAsync(IEnumerable<int> ids);
        ValueTask<SindicatoLaboral?> ObterPorCNPJAsync(CNPJ cnpj);
        ValueTask<bool> ExisteAsync(CNPJ cnpj, int ignorarId = 0);
        ValueTask<IEnumerable<SindicatoLaboral>?> ObterPorClientesUnidadeIdsAsync(IEnumerable<int> clientesUnidadeIds);
    }
}
