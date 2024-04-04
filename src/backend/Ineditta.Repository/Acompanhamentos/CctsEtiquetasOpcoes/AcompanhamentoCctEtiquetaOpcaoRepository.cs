using Ineditta.Application.Acompanhamentos.CctsEtiquetasOpcoes.Entities;
using Ineditta.Application.Acompanhamentos.CctsEtiquetasOpcoes.Repositories;
using Ineditta.Repository.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Ineditta.Repository.Acompanhamentos.CctsEtiquetasOpcoes
{
    public class AcompanhamentoCctEtiquetaOpcaoRepository : IAcompanhamentoCctEtiquetaOpcaoRepository
    {
        private readonly InedittaDbContext _context;

        public AcompanhamentoCctEtiquetaOpcaoRepository(InedittaDbContext context)
        {
            _context = context;
        }

        public async ValueTask<AcompanhamentoCctEtiquetaOpcao?> ObterPorId(long id)
        {
            return await _context.AcompanhamentoCctEtiquetaOpcao.SingleOrDefaultAsync(e => e.Id == id);
        }
    }
}
