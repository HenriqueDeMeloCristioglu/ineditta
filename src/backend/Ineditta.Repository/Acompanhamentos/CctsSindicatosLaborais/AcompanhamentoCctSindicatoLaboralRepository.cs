using Ineditta.Application.Acompanhamentos.CctsSindicatosLaborais.Entities;
using Ineditta.Application.Acompanhamentos.CctsSindicatosLaborais.Repositories;
using Ineditta.Repository.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Ineditta.Repository.Acompanhamentos.CctsSindicatosLaborais
{
    public class AcompanhamentoCctSindicatoLaboralRepository : IAcompanhamentoCctSindicatoLaboralRepository
    {
        private readonly InedittaDbContext _context;

        public AcompanhamentoCctSindicatoLaboralRepository(InedittaDbContext context)
        {
            _context = context;
        }

        public async ValueTask DeletarAsync(AcompanhamentoCctSinditoLaboral acompanhamentoCctSinditoLaboral)
        {
            _context.AcompanhamentoCctSinditoLaboral.Remove(acompanhamentoCctSinditoLaboral);

            await Task.CompletedTask;
        }

        public async ValueTask IncluirAsync(AcompanhamentoCctSinditoLaboral acompanhamentoCctSinditoLaboral)
        {
            _context.AcompanhamentoCctSinditoLaboral.Add(acompanhamentoCctSinditoLaboral);

            await Task.CompletedTask;
        }

        public async ValueTask<IEnumerable<AcompanhamentoCctSinditoLaboral>?> ObterPorAcompanhamentoIdAsync(long acompanhamentoId)
        {
            return await _context.AcompanhamentoCctSinditoLaboral
                .Where(c => c.AcompanhamentoCctId == acompanhamentoId)
                .ToListAsync();
        }
    }
}
