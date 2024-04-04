using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Ineditta.Application.Localizacoes.Entities;
using Ineditta.Application.Localizacoes.Repositories;
using Ineditta.Repository.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Ineditta.Repository.Localizacoes
{
    public class LocalizacaoRepository : ILocalizacaoRepository
    {
        private readonly InedittaDbContext _context;
        public LocalizacaoRepository(InedittaDbContext context)
        {
            _context = context;
        }

        public async ValueTask<Localizacao?> ObterPorIdAsync(int localizacaoId)
        {
            return await _context.Localizacoes.SingleOrDefaultAsync(l => l.Id == localizacaoId);
        }

        public async ValueTask<IEnumerable<Localizacao>?> ObterPorListaIdAsync(IEnumerable<int> ids)
        {
            return await _context.Localizacoes.Where(l => ids.Any(id => l.Id == id)).ToListAsync();
        }
    }
}
