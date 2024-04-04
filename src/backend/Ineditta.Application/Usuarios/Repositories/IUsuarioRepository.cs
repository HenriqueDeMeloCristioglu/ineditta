using Ineditta.Application.Usuarios.Entities;

namespace Ineditta.Application.Usuarios.Repositories
{
    public interface IUsuarioRepository
    {
        ValueTask IncluirAsync(Usuario usuario);
        ValueTask<Usuario?> ObterPorIdAsync(long id);
        ValueTask<IEnumerable<Usuario>?> ObterPorListaIdsAsync(IEnumerable<long> ids);
        ValueTask<Usuario?> ObterPorEmailAsync(string email);
        ValueTask AtualizarAsync(Usuario usuario);
        ValueTask<IEnumerable<Usuario>?> ObterPorUnidadesGrupoIds(IEnumerable<int> unidadesIds, IEnumerable<int> gruposEconomicosIds);
        ValueTask<IEnumerable<Usuario>?> ObterPorDocumentoId(long documentoId);
        ValueTask<IEnumerable<Usuario>?> ObterPorDocumentoId(long documentoId, IEnumerable<int>? usuariosIds);
    }
}
