using Ineditta.Application.Comentarios.Entities;
using Ineditta.Application.Comentarios.Repositories;
using Ineditta.Repository.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Ineditta.Repository.Comentarios
{
    public class ComentarioRepository : IComentarioRepository
    {
        private readonly InedittaDbContext _context;

        public ComentarioRepository(InedittaDbContext context)
        {
            _context = context;
        }

        public async ValueTask AtualizarAsync(Comentario comentario)
        {
            _context.Comentarios.Update(comentario);

            await Task.CompletedTask;
        }

        public async ValueTask IncluirAsync(Comentario comentario)
        {
            _context.Comentarios.Add(comentario);

            await Task.CompletedTask;
        }

        public async ValueTask<Comentario?> ObterPorIdAsync(long id)
        {
            return await _context.Comentarios.SingleOrDefaultAsync(c => c.Id == id);
        }
    }
}
