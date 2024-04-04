using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Ineditta.Application.ModulosClientes.Entities;
using Ineditta.Application.ModulosClientes.Repositories;
using Ineditta.Application.TiposDocumentos.Entities;
using Ineditta.Repository.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Ineditta.Repository.ModulosClientes
{
    public class ModuloClienteRepository : IModuloClienteRepository
    {
        private readonly InedittaDbContext _context;
        public ModuloClienteRepository(InedittaDbContext context)
        {
            _context = context;
        }
        public async ValueTask AtualizarAsync(ModuloCliente moduloCliente, CancellationToken cancellationToken = default)
        {
            _context.ModulosClientes.Update(moduloCliente);
            await Task.CompletedTask;
        }

        public async ValueTask IncluirAsync(ModuloCliente moduloCliente, CancellationToken cancellationToken = default)
        {
            _context.ModulosClientes.Add(moduloCliente);
            await Task.CompletedTask;
        }

        public async ValueTask<IEnumerable<ModuloCliente>?> ObterVigentesPorIdClienteMatriz(int clienteMatrizId)
        {
            return await _context.ModulosClientes.Where(mc => mc.ClienteMatrizId == clienteMatrizId && mc.DataFim == null).ToListAsync();
        }

        public async ValueTask<IEnumerable<ModuloCliente>?> ObterPorListaIds(IEnumerable<int> modulosClientesIds)
        {
            return await _context.ModulosClientes.Where(mc => modulosClientesIds.Contains(mc.Id)).ToListAsync();
        }
    }
}

