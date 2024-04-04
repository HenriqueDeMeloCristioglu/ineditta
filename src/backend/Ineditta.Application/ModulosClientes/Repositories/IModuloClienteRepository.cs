using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Ineditta.Application.ClientesMatriz.Entities;
using Ineditta.Application.ModulosClientes.Entities;
using Ineditta.Application.TiposDocumentos.Entities;

namespace Ineditta.Application.ModulosClientes.Repositories
{
    public interface IModuloClienteRepository
    {
        ValueTask AtualizarAsync(ModuloCliente moduloCliente, CancellationToken cancellationToken = default);
        ValueTask IncluirAsync(ModuloCliente moduloCliente, CancellationToken cancellationToken = default);
        ValueTask<IEnumerable<ModuloCliente>?> ObterPorListaIds(IEnumerable<int> modulosClientesIds);
        ValueTask<IEnumerable<ModuloCliente>?> ObterVigentesPorIdClienteMatriz(int clienteMatrizId);
    }
}
