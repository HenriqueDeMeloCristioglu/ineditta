using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Ineditta.Application.UsuariosTiposEventosCalendarioSindical.Entities;
using Ineditta.Application.UsuariosTiposEventosCalendarioSindical.Repositories;
using Ineditta.Repository.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Ineditta.Repository.UsuariosTiposEventosCalendarioSindical
{
    public class UsuarioTipoEventoCalendarioSindicalRepository : IUsuarioTipoEventoCalendarioSindicalRepository
    {
        private readonly InedittaDbContext _context;
        public UsuarioTipoEventoCalendarioSindicalRepository(InedittaDbContext context)
        {
            _context = context;
        }

        public async ValueTask AtualizarAsync(UsuarioTipoEventoCalendarioSindical usuarioTipoEventoCalendarioSindical)
        {
            _context.UsuariosTiposEventosCalendarioSindical.Update(usuarioTipoEventoCalendarioSindical);
            await Task.CompletedTask;
        }

        public async ValueTask IncluirAsync(UsuarioTipoEventoCalendarioSindical usuarioTipoEventoCalendarioSindical)
        {
            _context.UsuariosTiposEventosCalendarioSindical.Add(usuarioTipoEventoCalendarioSindical);
            await Task.CompletedTask;
        }

        public async ValueTask<IEnumerable<UsuarioTipoEventoCalendarioSindical>> ObterTodosPorUsuarioIdAsync(long usuarioId)
        {
            return await _context.UsuariosTiposEventosCalendarioSindical.Where(u => u.UsuarioId == usuarioId).ToListAsync();
        }
    }
}
