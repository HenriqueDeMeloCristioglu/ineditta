using Ineditta.Application.Acompanhamentos.CctsSindicatosPatronais.Entities;
using Ineditta.Application.Acompanhamentos.CctsSindicatosPatronais.Repositories;
using Ineditta.Repository.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Ineditta.Repository.Acompanhamentos.CctsSindicatosPatronais
{
    public class AcompanhamentoCctSindicatoPatronalRepository : IAcompanhamentoCctSindicatoPatronalRepository
    {
        private readonly InedittaDbContext _context;

        public AcompanhamentoCctSindicatoPatronalRepository(InedittaDbContext context)
        {
            _context = context;
        }

        public async ValueTask DeletarAsync(AcompanhamentoCctSinditoPatronal acompanhamentoCctSinditoPatronal)
        {
            _context.AcompanhamentoCctSinditoPatronal.Remove(acompanhamentoCctSinditoPatronal);

            await Task.CompletedTask;
        }

        public async ValueTask IncluirAsync(AcompanhamentoCctSinditoPatronal acompanhamentoCctSinditoPatronal)
        {
            _context.AcompanhamentoCctSinditoPatronal.Add(acompanhamentoCctSinditoPatronal);

            await Task.CompletedTask;
        }

        public async ValueTask<IEnumerable<AcompanhamentoCctSinditoPatronal>?> ObterPorAcompanhamentoIdAsync(long acompanhamentoId)
        {
            return await _context.AcompanhamentoCctSinditoPatronal
                .Where(c => c.AcompanhamentoCctId == acompanhamentoId)
                .ToListAsync();
        }
    }
}
