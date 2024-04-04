using Ineditta.Application.GruposEconomicos.Entities;
using Ineditta.Application.GruposEconomicos.Repositories;
using Ineditta.Repository.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Ineditta.Repository.GruposEconomicos
{
    public class GrupoEconomicoRepository : IGrupoEconomicoRepository
    {
        private readonly InedittaDbContext _context;

        public GrupoEconomicoRepository(InedittaDbContext context)
        {
            _context = context;            
        }

        public async ValueTask<GrupoEconomico?> ObterPorIdAsync(long id)
        {
            return await _context.GrupoEconomico.FirstOrDefaultAsync(cg => cg.Id == id);
        }
    }
}
