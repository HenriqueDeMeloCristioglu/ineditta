using Ineditta.Application.ClientesMatriz.Entities;

namespace Ineditta.Application.ClientesMatriz.Repositories
{
    public interface IClienteMatrizRepository
    {
        ValueTask IncluirAsync(ClienteMatriz clienteMatriz, CancellationToken cancellationToken = default);
        ValueTask AtualizarAsync(ClienteMatriz clienteMatriz, CancellationToken cancellationToken = default);
        ValueTask<int?> ObterMenorDataCortePagamento(IEnumerable<int> matrizesIds, CancellationToken cancellationToken = default);
        ValueTask<ClienteMatriz?> ObterPorId(int id, CancellationToken cancellationToken = default);
    }
}
