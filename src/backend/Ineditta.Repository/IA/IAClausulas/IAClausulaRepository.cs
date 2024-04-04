using Ineditta.Application.AIs.Clausulas.Entities;
using Ineditta.Application.AIs.Clausulas.Repositories;
using Ineditta.Repository.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Ineditta.Repository.IA.IAClausulas
{
    public class IAClausulaRepository : IIAClausulaRepository
    {
        private readonly InedittaDbContext _context;

        public IAClausulaRepository(InedittaDbContext context)
        {
            _context = context;
        }

        public async ValueTask AtualizarAsync(IAClausula iaClausula)
        {
            _context.IAClausulas.Update(iaClausula);
            await Task.CompletedTask;
        }

        public async ValueTask InserirAsync(IAClausula iaClausula)
        {
            _context.IAClausulas.Add(iaClausula);
            await Task.CompletedTask;
        }

        public async ValueTask<IAClausula?> ObterPorIdAsync(long iaClausulaId)
        {
            return await _context.IAClausulas.SingleOrDefaultAsync(iaC => iaC.Id == iaClausulaId);
        }

        public async ValueTask DeleteAsync(IAClausula iaClausula)
        {
            _context.IAClausulas.Remove(iaClausula);
            await Task.CompletedTask;
        }

        public async ValueTask DeleteAsync(IEnumerable<IAClausula> iaClausula)
        {
            _context.IAClausulas.RemoveRange(iaClausula);
            await Task.CompletedTask;
        }

        public async ValueTask<bool> ExisteClausulaPendenteClassificacaoAsync(long iADocumentoSindicalId, int ignorarId)
        {
            return await _context.IAClausulas.AnyAsync(s => s.IADocumentoSindicalId == iADocumentoSindicalId && s.Id != ignorarId && ((s.EstruturaClausulaId ?? 0) <= 0));
        }

        public async ValueTask<IEnumerable<IAClausula>?> ObterTodosPorIADocumentoIdAsync(long iaDocumentoId, bool somenteNaoClassificado = false)
        {
            return await _context.IAClausulas.Where(iaClausula => iaClausula.IADocumentoSindicalId == iaDocumentoId && 
                                                                  (((iaClausula.EstruturaClausulaId ?? 0) == 0 && somenteNaoClassificado) || !somenteNaoClassificado)).ToListAsync();
        }

        public async ValueTask<bool> ExisteClausulaInconsistenteAsync(long iADocumentoSindicalId)
        {
            return await _context.IAClausulas.AnyAsync(s => s.IADocumentoSindicalId == iADocumentoSindicalId && (s.Status == IAClausulaStatus.Inconsistente || ((s.EstruturaClausulaId ?? 0) <= 0)));
        }
    }
}
