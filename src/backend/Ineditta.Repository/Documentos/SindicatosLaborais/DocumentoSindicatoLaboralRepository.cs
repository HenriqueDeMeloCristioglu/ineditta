using Ineditta.Application.Documentos.SindicatosLaborais;
using Ineditta.Application.Documentos.SindicatosLaborais.Repositories;
using Ineditta.Repository.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Ineditta.Repository.Documentos.SindicatosLaborais
{
    public class DocumentoSindicatoLaboralRepository : IDocumentoSindicatoLaboralRepository
    {
        private readonly InedittaDbContext _context;

        public DocumentoSindicatoLaboralRepository(InedittaDbContext context)
        {
            _context = context;
        }

        public async ValueTask DeletarPorDocumentoIdAsync(int documentoId)
        {
            var documentoSindicatosLaborais = await _context.DocumentosSindicatosLaborais.Where(dsl => dsl.DocumentoSindicalId == documentoId).ToListAsync();
            _context.DocumentosSindicatosLaborais.RemoveRange(documentoSindicatosLaborais);
            await Task.CompletedTask;
        }

        public async ValueTask InserirAsync(DocumentoSindicatoLaboral documentoSindicatoLaboral)
        {
            _context.DocumentosSindicatosLaborais.Add(documentoSindicatoLaboral);
            await Task.CompletedTask;
        }
    }
}
