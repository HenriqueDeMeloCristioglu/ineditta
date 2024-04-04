using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Ineditta.Application.GruposEconomicos.Entities;

namespace Ineditta.Application.GruposEconomicos.Repositories
{
    public interface IGrupoEconomicoRepository
    {
        ValueTask<GrupoEconomico?> ObterPorIdAsync(long id);
    }
}
