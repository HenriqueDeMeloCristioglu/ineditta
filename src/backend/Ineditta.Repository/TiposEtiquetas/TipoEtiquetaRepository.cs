using Ineditta.Application.TiposEtiquetas.Entities;
using Ineditta.Application.TiposEtiquetas.Respositories;
using Ineditta.Repository.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Ineditta.Repository.TiposEtiquetas
{
    public class TipoEtiquetaRepository : ITipoEtiquetaRepository
    {
        private readonly InedittaDbContext _context;

        public TipoEtiquetaRepository(InedittaDbContext context)
        {
            _context = context;
        }

        public async ValueTask AtualizarAsync(TipoEtiqueta tipoEtiqueta)
        {
            _context.Update(tipoEtiqueta);

            await Task.CompletedTask;
        }

        public async ValueTask InlcuirAsync(TipoEtiqueta tipoEtiqueta)
        {
            _context.Add(tipoEtiqueta);

            await Task.CompletedTask;
        }

        public async ValueTask<TipoEtiqueta?> ObterPorIdAsync(int id)
        {
            var result = await _context.TiposEtiquetas.Where(t => t.Id == id).SingleOrDefaultAsync();

            if (result == null)
            {
                return null;
            }

            return result;
        }
    }
}
