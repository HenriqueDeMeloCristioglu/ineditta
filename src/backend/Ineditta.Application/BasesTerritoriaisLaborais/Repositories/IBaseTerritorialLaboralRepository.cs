using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Ineditta.Application.BasesTerritoriaisLaborais.Entities;
using Ineditta.Application.BasesTerritoriaisPatronais.Entities;

namespace Ineditta.Application.BasesTerritoriaisLaborais.Repositories
{
    public interface IBaseTerritorialLaboralRepository
    {
        ValueTask IncluirAsync(BaseTerritorialLaboral baseTerritorialLaboral);
        ValueTask AtualizarAsync(BaseTerritorialLaboral baseTerritorialLaboral);
        ValueTask<BaseTerritorialLaboral?> ObterPorIdAsync(int id);
        ValueTask<IEnumerable<BaseTerritorialLaboral>?> ObterPorSindicatoLaboralIdAsync(int id);
        ValueTask<IEnumerable<BaseTerritorialLaboral>?> ObterVigentesPorSindicatoLaboralIdAsync(int id);
    }
}
