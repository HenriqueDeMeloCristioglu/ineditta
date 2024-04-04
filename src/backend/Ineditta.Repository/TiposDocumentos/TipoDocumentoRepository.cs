using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Ineditta.Application.Documentos.Sindicais.Dtos;
using Ineditta.Application.TiposDocumentos.Entities;
using Ineditta.Application.TiposDocumentos.Repositories;
using Ineditta.Application.TiposDocumentos.UseCases.Upsert;
using Ineditta.Repository.Contexts;
using Ineditta.Repository.Models;

using Microsoft.EntityFrameworkCore;

using Org.BouncyCastle.Asn1.Ocsp;

namespace Ineditta.Repository.TiposDocumentos
{
    public class TipoDocumentoRepository : ITipoDocumentoRepository
    {
        private readonly InedittaDbContext _context;
        public TipoDocumentoRepository(InedittaDbContext context)
        {
            _context = context;
        }

        public async ValueTask AtualizarAsync(TipoDocumento tipoDocumento)
        {
            _context.TipoDocs.Update(tipoDocumento);
            await Task.CompletedTask;
        }

        public async ValueTask IncluirAsync(TipoDocumento tipoDocumento)
        {
            _context.TipoDocs.Add(tipoDocumento);
            await Task.CompletedTask;
        }

        public async ValueTask<TipoDocumento?> ObterPorIdAsync(int tipoDocumentoId)
        {
            return await _context.TipoDocs.SingleOrDefaultAsync(td => td.Id == tipoDocumentoId);
        }

        public async ValueTask<IEnumerable<TipoDocumento>?> ObterPorListaIds(IEnumerable<int> tiposIds)
        {
            return await _context.TipoDocs.Where(td => tiposIds.Contains(td.Id)).ToListAsync();
        }
    }
}
