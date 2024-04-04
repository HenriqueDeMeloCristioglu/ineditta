using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Ineditta.Application.ClientesUnidadesSindicatosPatronais.Entities;
using Ineditta.Application.ClientesUnidadesSindicatosPatronais.Repositories;
using Ineditta.Repository.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Ineditta.Repository.ClientesUnidadesSindicatosPatronais
{
    public class ClienteUnidadeSindicatoPatronalRepository : IClienteUnidadeSindicatoPatronalRepository
    {
        private readonly InedittaDbContext _context;
        public ClienteUnidadeSindicatoPatronalRepository(InedittaDbContext context)
        {
            _context = context;
        }
        public async ValueTask<bool> ExisteAlgumPorSindicatoPatronalAsync(int sindicatoPatronalId)
        {
            return await _context.ClientesUnidadesSindicatosPatronais.AnyAsync(cusp => cusp.SindicatoPatronalId == sindicatoPatronalId);
        }

        public async ValueTask<bool> ExistePorIdAsync(long clienteUnidadeSindicatoPatronalId)
        {
            return await _context.ClientesUnidadesSindicatosPatronais.AnyAsync(cusp => cusp.Id == clienteUnidadeSindicatoPatronalId);
        }

        public async ValueTask IncluirAsync(ClienteUnidadeSindicatoPatronal clienteUnidadeSindicatoPatronal)
        {
            _context.ClientesUnidadesSindicatosPatronais.Add(clienteUnidadeSindicatoPatronal);
            await Task.CompletedTask;
        }

        public async ValueTask<IEnumerable<ClienteUnidadeSindicatoPatronal>?> ObterTodosPorSindicatoId(int sindicatoPatronalId)
        {
            return await _context.ClientesUnidadesSindicatosPatronais.Where(cusp => cusp.SindicatoPatronalId == sindicatoPatronalId).ToListAsync();
        }

        public async ValueTask RemoverPorIdAsync(long clienteUnidadeSindicatoPatronalId)
        {
            var clienteUnidadeSindicatoPatronalParaDeletar = await _context.ClientesUnidadesSindicatosPatronais.SingleAsync(cusp => cusp.Id == clienteUnidadeSindicatoPatronalId);
            _context.ClientesUnidadesSindicatosPatronais.Remove(clienteUnidadeSindicatoPatronalParaDeletar);
            await Task.CompletedTask;
        }
    }
}
