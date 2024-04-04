using Ineditta.Application.Acompanhamentos.CctsFases.Repositories;
using Ineditta.Application.CctsFases.Entities;
using Ineditta.Repository.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Ineditta.Repository.Acompanhamentos.CctsFases
{
    public class FaseCctRepository : IFaseCctRepository
    {
        private readonly InedittaDbContext _context;

        public FaseCctRepository(InedittaDbContext context)
        {
            _context = context;
        }

        public async ValueTask<FasesCct?> ObterPorIdAsync(long id)
        {
            return await _context.FasesCct.FirstOrDefaultAsync(c => c.Id == id);
        }
    }
}
