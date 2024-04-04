using Ineditta.Application.DirigentesPatronais.Entities;
using Ineditta.Application.DirigentesPatronais.Repositories;
using Ineditta.Repository.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Ineditta.Repository.Dirigentes.Patronais
{
    public class DirigentePatronalRepository : IDirigentePatronalRepository
    {
        private readonly InedittaDbContext _context;

        public DirigentePatronalRepository(InedittaDbContext context)
        {
            _context = context;
        }

        public async ValueTask AtualizarAsync(DirigentePatronal dirigentePatronal)
        {
            _context.SindDirpatros.Update(dirigentePatronal);
            await Task.CompletedTask;
        }

        public async ValueTask IncluirAsync(DirigentePatronal dirigentePatronal)
        {
            _context.SindDirpatros.Add(dirigentePatronal);
            await Task.CompletedTask;
        }

        public async ValueTask<DirigentePatronal?> ObterPorIdAsync(long dirigenteId)
        {
            return await _context.SindDirpatros.SingleOrDefaultAsync(d => d.Id == dirigenteId);
        }
    }
}
