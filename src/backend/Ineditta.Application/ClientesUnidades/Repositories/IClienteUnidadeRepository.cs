using Ineditta.Application.ClientesUnidades.Entities;
using Ineditta.Application.GruposEconomicos.Entities;

namespace Ineditta.Application.ClientesUnidades.Repositories
{
    public interface IClienteUnidadeRepository
    {
        ValueTask IncluirAsync(ClienteUnidade clienteUnidade);
        ValueTask AtualizarAsync(ClienteUnidade clienteUnidade);
        ValueTask<ClienteUnidade?> ObterPorIdAsync(int id);
        ValueTask<IEnumerable<ClienteUnidade>?> ObterPorListaIds(IEnumerable<int> ids);
        ValueTask<bool> ExistePorIdAsync(int clienteUnidadeId);
        ValueTask<IEnumerable<ClienteUnidade>?> ObterClientesUnidadePorDocumentoPorUsuario(long documentoId, long usuarioId);
    }
}
