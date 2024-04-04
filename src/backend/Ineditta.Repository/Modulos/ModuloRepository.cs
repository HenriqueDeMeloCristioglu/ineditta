using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Ineditta.Application.Modulos.Entities;
using Ineditta.Application.Modulos.Repositories;
using Ineditta.Repository.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Ineditta.Repository.Modulos
{
    public class ModuloRepository : IModuloRepository
    {
        private readonly InedittaDbContext _context;
        public ModuloRepository(InedittaDbContext context)
        {
            _context = context;
        }
        public async ValueTask AtualizarAsync(Modulo modulo, CancellationToken cancellationToken = default)
        {
            _context.Modulos.Update(modulo);
            await Task.CompletedTask;
        }

        public async ValueTask IncluirAsync(Modulo modulo, CancellationToken cancellationToken = default)
        {
            _context.Modulos.Add(modulo);
            await Task.CompletedTask;
        }

        public async ValueTask<IEnumerable<Modulo>?> ObterPorListaIds(IEnumerable<int> modulosIds)
        {
            return await _context.Modulos.Where(mc => modulosIds.Contains(mc.Id)).ToListAsync();
        }
    }
}
