using Ineditta.Application.Documentos.Estabelecimentos.Entities;
using Ineditta.Application.Documentos.Estabelecimentos.Repositories;
using Ineditta.Repository.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Ineditta.Repository.Documentos.Estabelecimentos.Repositories
{
    public class DocumentoEstabelecimentoRepository : IDocumentoEstabelecimentoRepository
    {
        private readonly InedittaDbContext _dbContext;

        public DocumentoEstabelecimentoRepository(InedittaDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async ValueTask InserirAsync(DocumentoEstabelecimento documentoEstabelecimento)
        {
            _dbContext.DocumentosEstabelecimentos.Add(documentoEstabelecimento);
            await Task.CompletedTask;
        }

        public async ValueTask RemoverAsync(DocumentoEstabelecimento documentoEstabelecimento)
        {
            _dbContext.DocumentosEstabelecimentos.Remove(documentoEstabelecimento);
            await Task.CompletedTask;
        }

        public async ValueTask RemoverTudoPorDocumentoId(int documentoId)
        {
            var documentosEstabelecimentos = await _dbContext.DocumentosEstabelecimentos.Where(de => de.DocumentoId == documentoId).ToListAsync();
            _dbContext.DocumentosEstabelecimentos.RemoveRange(documentosEstabelecimentos);
            await Task.CompletedTask;
        }
    }
}
