using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Ineditta.Application.Modulos.Entities;
using Ineditta.Application.ModulosClientes.Entities;

namespace Ineditta.Application.Modulos.Repositories
{
    public interface IModuloRepository
    {
        ValueTask AtualizarAsync(Modulo modulo, CancellationToken cancellationToken = default);
        ValueTask IncluirAsync(Modulo modulo, CancellationToken cancellationToken = default);
        ValueTask<IEnumerable<Modulo>?> ObterPorListaIds(IEnumerable<int> modulosIds);
    }
}
