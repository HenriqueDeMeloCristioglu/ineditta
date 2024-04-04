using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Ineditta.Application.BasesTerritoriaisLaborais.Entities;
using Ineditta.Application.BasesTerritoriaisLaborais.Repositories;
using Ineditta.Application.BasesTerritoriaisPatronais.Entities;
using Ineditta.Repository.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Ineditta.Repository.BasesTerritoriaisLaborais
{
    public class BaseTerritorialLaboralRepository : IBaseTerritorialLaboralRepository
    {
        private readonly InedittaDbContext _context;
        public BaseTerritorialLaboralRepository(InedittaDbContext context)
        {
            _context = context;
        }

        public async ValueTask AtualizarAsync(BaseTerritorialLaboral baseTerritorialLaboral)
        {
            _context.BaseTerritorialsindemps.Update(baseTerritorialLaboral);
            await Task.CompletedTask;
        }

        public async ValueTask IncluirAsync(BaseTerritorialLaboral baseTerritorialLaboral)
        {
            _context.BaseTerritorialsindemps.Add(baseTerritorialLaboral);
            await Task.CompletedTask;
        }

        public async ValueTask<BaseTerritorialLaboral?> ObterPorIdAsync(int id)
        {
            return await _context.BaseTerritorialsindemps.FirstOrDefaultAsync(btp => btp.Id == id);
        }

        public async ValueTask<IEnumerable<BaseTerritorialLaboral>?> ObterPorSindicatoLaboralIdAsync(int id)
        {
            return await _context.BaseTerritorialsindemps.Where(btp => btp.SindicatoId == id).ToListAsync();
        }

        public async ValueTask<IEnumerable<BaseTerritorialLaboral>?> ObterVigentesPorSindicatoLaboralIdAsync(int id)
        {
            return await _context.BaseTerritorialsindemps.Where(btp => btp.SindicatoId == id && (btp.DataFinal == null || btp.DataFinal <= DateOnly.FromDateTime(DateTime.MinValue))).ToListAsync();
        }
    }
}
