using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Ineditta.Application.ClientesUnidadesSindicatosPatronais.Entities;

namespace Ineditta.Application.ClientesUnidadesSindicatosPatronais.Repositories
{
    public interface IClienteUnidadeSindicatoPatronalRepository
    {
        ValueTask IncluirAsync(ClienteUnidadeSindicatoPatronal clienteUnidadeSindicatoPatronal);
        ValueTask RemoverPorIdAsync(long clienteUnidadeSindicatoPatronalId);
        ValueTask<bool> ExisteAlgumPorSindicatoPatronalAsync(int sindicatoPatronalId);
        ValueTask<bool> ExistePorIdAsync(long clienteUnidadeSindicatoPatronalId);
        ValueTask<IEnumerable<ClienteUnidadeSindicatoPatronal>?> ObterTodosPorSindicatoId(int sindicatoPatronalId);
    }
}
