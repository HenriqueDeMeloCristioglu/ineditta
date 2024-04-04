using Ineditta.Application.Documentos.SindicatosPatronais;
using Ineditta.Application.Documentos.SindicatosPatronais.Repositories;
using Ineditta.Repository.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Ineditta.Repository.Documentos.SindicatosPatronais
{
    public class DocumentoSindicatoPatronalRepository : IDocumentoSindicatoPatronalRepository
    {
        private readonly InedittaDbContext _context;

        public DocumentoSindicatoPatronalRepository(InedittaDbContext context)
        {
            _context = context;
        }

        public async ValueTask DeletarPorDocumentoIdAsync(int documentoId)
        {
            var documentosSindicatosPatronais = await _context.DocumentosSindicatosPatronais.Where(dsp => dsp.DocumentoSindicalId == documentoId).ToListAsync();
            _context.DocumentosSindicatosPatronais.RemoveRange(documentosSindicatosPatronais);
            await Task.CompletedTask;
        }

        public async ValueTask InserirAsync(DocumentoSindicatoPatronal documentoSindicatoPatronal)
        {
            _context.DocumentosSindicatosPatronais.Add(documentoSindicatoPatronal);
            await Task.CompletedTask;
        }
    }
}
