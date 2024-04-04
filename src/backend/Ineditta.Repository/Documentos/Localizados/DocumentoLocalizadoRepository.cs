using Ineditta.Application.Documentos.Localizados.Repositories;
using Ineditta.Application.DocumentosLocalizados.Entities;
using Ineditta.Repository.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Ineditta.Repository.Documentos.Localizados
{
    public class DocumentoLocalizadoRepository : IDocumentoLocalizadoRepository
    {
        private readonly InedittaDbContext _context;

        public DocumentoLocalizadoRepository(InedittaDbContext dbContext)
        {
            _context = dbContext;
        }

        public async ValueTask AtualizarAsync(DocumentoLocalizado documentoLocalizado)
        {
            _context.DocumentosLocalizados.Update(documentoLocalizado);
            await Task.CompletedTask;
        }

        public async ValueTask DeletarAsync(DocumentoLocalizado documentoLocalizado)
        {
            _context.DocumentosLocalizados.Remove(documentoLocalizado);
            await Task.CompletedTask;
        }

        public async ValueTask IncluirAsync(DocumentoLocalizado documentoLocalizado)
        {
            _context.DocumentosLocalizados.Add(documentoLocalizado);
            await Task.CompletedTask;
        }

        public async ValueTask<DocumentoLocalizado?> ObterPorIdAsync(long id)
        {
            return await _context.DocumentosLocalizados.FirstOrDefaultAsync(d => d.Id == id);
        }
    }
}
