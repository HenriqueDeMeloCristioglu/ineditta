using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Ineditta.Application.ClientesMatriz.Entities;
using Ineditta.Application.ClientesMatriz.Repositories;
using Ineditta.Repository.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Ineditta.Repository.ClientesMatriz
{
    public class ClienteMatrizRepository : IClienteMatrizRepository
    {
        private readonly InedittaDbContext _context;

        public ClienteMatrizRepository(InedittaDbContext context)
        {
            _context = context;
        }

        public async ValueTask AtualizarAsync(ClienteMatriz clienteMatriz, CancellationToken cancellationToken = default)
        {
            _context.ClienteMatrizes.Update(clienteMatriz);
            await Task.CompletedTask;
        }

        public async ValueTask IncluirAsync(ClienteMatriz clienteMatriz, CancellationToken cancellationToken = default)
        {
            _context.ClienteMatrizes.Add(clienteMatriz);
            await Task.CompletedTask;
        }

        public async ValueTask<int?> ObterMenorDataCortePagamento(IEnumerable<int> matrizesIds, CancellationToken cancellationToken = default)
        {
            var datasCorte = await _context.ClienteMatrizes
                .Where(cm => matrizesIds.ToList().Contains(cm.Id))
                .Select(cm => cm.DataCorteForpag)
                .Distinct().ToListAsync(cancellationToken);

            if(!datasCorte.Any())
            {
                return 0;
            }
#pragma warning disable CA1305 // Especificar IFormatProvider
            return datasCorte.Min();
#pragma warning restore CA1305 // Especificar IFormatProvider
        }

        public async ValueTask<ClienteMatriz?> ObterPorId(int id, CancellationToken cancellationToken = default)
        {
            return await _context.ClienteMatrizes.SingleOrDefaultAsync(cm => cm.Id == id, cancellationToken);
        }
    }
}
