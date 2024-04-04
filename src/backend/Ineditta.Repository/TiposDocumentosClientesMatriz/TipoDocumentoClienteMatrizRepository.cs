using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Ineditta.Application.TiposDocumentosClientesMatriz.Entities;
using Ineditta.Application.TiposDocumentosClientesMatriz.repositories;
using Ineditta.Repository.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Ineditta.Repository.TiposDocumentosClientesMatriz
{
    public class TipoDocumentoClienteMatrizRepository : ITipoDocumentoClienteMatrizRepository
    {
        private readonly InedittaDbContext _context;
        public TipoDocumentoClienteMatrizRepository(InedittaDbContext context)
        {
            _context = context;
        }
        public async ValueTask AtualizarAsync(TipoDocumentoClienteMatriz tipoDocumentoClienteMatriz, CancellationToken cancellationToken = default)
        {
            _context.TiposDocumentosClientesMatriz.Update(tipoDocumentoClienteMatriz);
            await Task.CompletedTask;
        }

        public async ValueTask IncluirAsync(TipoDocumentoClienteMatriz tipoDocumentoClienteMatriz, CancellationToken cancellationToken = default)
        {
            _context.TiposDocumentosClientesMatriz.Add(tipoDocumentoClienteMatriz);
            await Task.CompletedTask;
        }

        public async ValueTask LimparTodosPorMatrizIdAsync(int matrizId)
        {
            var listaParaDeletar = await _context.TiposDocumentosClientesMatriz.Where(tdcm => tdcm.ClienteMatrizId == matrizId).ToListAsync();
            _context.TiposDocumentosClientesMatriz.RemoveRange(listaParaDeletar);
            await Task.CompletedTask;
        }
    }
}
