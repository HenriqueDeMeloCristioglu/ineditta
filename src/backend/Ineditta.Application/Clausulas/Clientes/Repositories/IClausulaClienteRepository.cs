using Ineditta.Application.Clausulas.Clientes.Entities;

namespace Ineditta.Application.Clausulas.Clientes.Repositories
{
    public interface IClausulaClienteRepository
    {
        ValueTask IncluirAsync(ClausulaCliente clausulaCliente);
        ValueTask AtualizarAsync(ClausulaCliente clausulaCliente);
        ValueTask<ClausulaCliente?> ObterPorId(int id);
    }
}
