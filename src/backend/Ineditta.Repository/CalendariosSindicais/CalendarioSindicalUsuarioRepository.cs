using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Ineditta.Application.CalendarioSindicais.Usuarios.Entities;
using Ineditta.Application.CalendarioSindicais.Usuarios.Repositories;
using Ineditta.Repository.Contexts;

namespace Ineditta.Repository.CalendariosSindicais
{
    public class CalendarioSindicalUsuarioRepository : ICalendarioSindicalUsuarioRepository 
    {
    
        private readonly InedittaDbContext _context;

        public CalendarioSindicalUsuarioRepository(InedittaDbContext context)
        {
            _context = context;
        }

        public async ValueTask AtualizarAsync(CalendarioSindicalUsuario evento)
        {
            _context.Entry(evento).Property("DataInclusao").CurrentValue = DateTime.Now;

            if (evento.Id <= 0) return;

            _context.CalendariosSindicaisUsuario.Update(evento);

            await Task.CompletedTask;
        }

        public async ValueTask IncluirAsync(CalendarioSindicalUsuario evento)
        {
            _context.Entry(evento).Property("DataInclusao").CurrentValue = DateTime.Now;

            _context.CalendariosSindicaisUsuario.Add(evento);

            await Task.CompletedTask;
        }

        public async ValueTask<CalendarioSindicalUsuario?> ObterPorIdAsync(long id)
        {
            return await _context.CalendariosSindicaisUsuario.FindAsync(id);
        }
    }
}
