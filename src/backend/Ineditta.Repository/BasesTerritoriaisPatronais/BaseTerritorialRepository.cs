using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Ineditta.Application.BasesTerritoriaisPatronais.Entities;
using Ineditta.Application.BasesTerritoriaisPatronais.Repositories;
using Ineditta.Repository.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Ineditta.Repository.BasesTerritoriaisPatronais
{
    public class BaseTerritorialRepository : IBaseTerritorialRepository
    {
        private readonly InedittaDbContext _context;
        public BaseTerritorialRepository(InedittaDbContext context)
        {
            _context = context;
        }

        public async ValueTask AtualizarAsync(BaseTerritorialPatronal baseTerritorialPatronal)
        {
            _context.BaseTerritorialsindpatros.Update(baseTerritorialPatronal);
            await Task.CompletedTask;
        }

        public async ValueTask IncluirAsync(BaseTerritorialPatronal baseTerritorialPatronal)
        {
            _context.BaseTerritorialsindpatros.Add(baseTerritorialPatronal);
            await Task.CompletedTask;
        }

        public async ValueTask<BaseTerritorialPatronal?> ObterPorIdAsync(int id)
        {
            return await _context.BaseTerritorialsindpatros.FirstOrDefaultAsync(btp => btp.Id == id);
        }

        public async ValueTask<IEnumerable<BaseTerritorialPatronal>?> ObterPorSindicatoPatronalIdAsync(int id)
        {
            return await _context.BaseTerritorialsindpatros.Where(btp => btp.SindicatoId == id).ToListAsync();
        }

        public async ValueTask<IEnumerable<BaseTerritorialPatronal>?> ObterVigentesPorSindicatoPatronalIdAsync(int id)
        {
            return await _context.BaseTerritorialsindpatros.Where(btp => btp.SindicatoId == id && (btp.DataFinal == null || btp.DataFinal <= DateOnly.FromDateTime(DateTime.MinValue))).ToListAsync();
        }
    }
}
