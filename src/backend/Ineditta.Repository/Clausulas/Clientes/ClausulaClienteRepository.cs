using Ineditta.Application.Clausulas.Clientes.Entities;
using Ineditta.Application.Clausulas.Clientes.Repositories;
using Ineditta.Repository.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Ineditta.Repository.Clausulas.Clientes
{
    public class ClausulaClienteRepository : IClausulaClienteRepository
    {
        private readonly InedittaDbContext _context;

        public ClausulaClienteRepository(InedittaDbContext context)
        {
            _context = context;
        }

        public async ValueTask AtualizarAsync(ClausulaCliente clausulaCliente)
        {
            _context.ClausulaCliente.Update(clausulaCliente);

            await Task.CompletedTask;
        }

        public async ValueTask IncluirAsync(ClausulaCliente clausulaCliente)
        {
            _context.ClausulaCliente.Add(clausulaCliente);

            await Task.CompletedTask;
        }

        public async ValueTask<ClausulaCliente?> ObterPorId(int id)
        {
            return await _context.ClausulaCliente.SingleOrDefaultAsync(c => c.Id == id);
        }
    }
}
