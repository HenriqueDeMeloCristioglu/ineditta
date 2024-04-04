using Ineditta.Application.Jornada.Entities;
using Ineditta.Application.Jornada.Repositories;
using Ineditta.Repository.Contexts;

namespace Ineditta.Repository.Jornadas
{
    public class JornadaRepository : IJornadaRepository
    {
        private readonly InedittaDbContext _context;

        public JornadaRepository(InedittaDbContext context)
        {
            _context = context;
        }

        public async ValueTask IncluirAsync(Jornada jornada)
        {
            _context.Jornada.Add(jornada);

            await Task.CompletedTask;
        }
    }
}
