using Ineditta.Application.Etiquetas.Entities;
using Ineditta.Application.Etiquetas.Respositories;
using Ineditta.Repository.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Ineditta.Repository.Etiquetas
{
    public class EtiquetaRepository : IEtiquetaRepository
    {
        private readonly InedittaDbContext _context;

        public EtiquetaRepository(InedittaDbContext context)
        {
            _context = context;
        }

        public async ValueTask AtualizarAsync(Etiqueta etiqueta)
        {
            _context.Etiquetas.Update(etiqueta);

            await Task.CompletedTask;
        }

        public async ValueTask IncluirAsync(Etiqueta etiqueta)
        {
            _context.Etiquetas.Add(etiqueta);

            await Task.CompletedTask;
        }

        public async ValueTask<Etiqueta?> ObterPorIdAsync(long id)
        {
            return await _context.Etiquetas.Where(e => e.Id == id).SingleOrDefaultAsync();
        }
    }
}
