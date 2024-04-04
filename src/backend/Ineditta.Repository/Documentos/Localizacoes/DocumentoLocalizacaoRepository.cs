using Ineditta.Application.Documentos.Localizacoes.Entities;
using Ineditta.Application.Documentos.Localizacoes.Repositories;
using Ineditta.Repository.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Ineditta.Repository.Documentos.Localizacoes
{
    public class DocumentoLocalizacaoRepository : IDocumentoLocalizacaoRepository
    {
        private readonly InedittaDbContext _context;

        public DocumentoLocalizacaoRepository(InedittaDbContext context)
        {
            _context = context;
        }

        public async ValueTask AtualizarAsync(DocumentoLocalizacao documentoAbrangencia)
        {
            _context.DocumentosLocalizacoes.Update(documentoAbrangencia);
            await Task.CompletedTask;
        }

        public async ValueTask InserirAsync(DocumentoLocalizacao documentoAbrangencia)
        {
            _context.DocumentosLocalizacoes.Add(documentoAbrangencia);
            await Task.CompletedTask;
        }

        public async ValueTask RemoverTudoPorDocumentoIdAsync(int documentoId)
        {
            var documentosLocalizacoes = await _context.DocumentosLocalizacoes
                .Where(dl => dl.DocumentoId == documentoId)
                .ToListAsync();

            _context.DocumentosLocalizacoes.RemoveRange(documentosLocalizacoes);

            await Task.CompletedTask;
        }
    }
}
