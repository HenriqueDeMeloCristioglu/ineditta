using Ineditta.Application.AIs.DocumentosSindicais.Entities;
using Ineditta.Application.AIs.DocumentosSindicais.Repositories;
using Ineditta.Repository.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Ineditta.Repository.IA.IADocumentosSindicais
{
    public class IADocumentoSindicalRepository : IIADocumentoSindicalRepository
    {
        private readonly InedittaDbContext _context;

        public IADocumentoSindicalRepository(InedittaDbContext context)
        {
            _context = context;
        }

        public async ValueTask AtualizarAsync(IADocumentoSindical iaDocumentoSindical)
        {
            _context.IADocumentoSindical.Update(iaDocumentoSindical);

            await Task.CompletedTask;
        }

        public async ValueTask IncluirAsync(IADocumentoSindical iaDocumentoSindical)
        {
            _context.IADocumentoSindical.Add(iaDocumentoSindical);

            await Task.CompletedTask;
        }

        public async ValueTask<IADocumentoSindical?> ObterPorIdAsync(long id)
        {
            return await _context.IADocumentoSindical.SingleOrDefaultAsync(x => x.Id == id);
        }

        public async ValueTask<bool> ExistePorDocumentoReferenciaIdAsync(int documentoReferenciaId)
        {
            return await _context.IADocumentoSindical.AnyAsync(iaDocumentoSindical => iaDocumentoSindical.DocumentoReferenciaId == documentoReferenciaId);
        }

        public async ValueTask LockAsync(long iADocumentoSindicalId, CancellationToken cancellationToken = default)
        {
            await _context.Database.ExecuteSqlRawAsync($"UPDATE ia_documento_sindical_tb SET motivo_erro = motivo_erro WHERE id = {iADocumentoSindicalId}", cancellationToken);
        }
    }
}
