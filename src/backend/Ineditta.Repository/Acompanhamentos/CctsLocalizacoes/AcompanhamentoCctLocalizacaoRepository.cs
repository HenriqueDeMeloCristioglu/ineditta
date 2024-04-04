using Ineditta.Application.Acompanhamentos.CctsLocalizacoes.Entities;
using Ineditta.Application.Acompanhamentos.CctsLocalizacoes.Repositories;
using Ineditta.Repository.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Ineditta.Repository.Acompanhamentos.CctsLocalizacoes
{
    public class AcompanhamentoCctLocalizacaoRepository : IAcompanhamentoCctLocalizacaoRepository
    {
        private readonly InedittaDbContext _context;

        public AcompanhamentoCctLocalizacaoRepository(InedittaDbContext context)
        {
            _context = context;
        }
        public async ValueTask DeletarAsync(AcompanhamentoCctLocalizacao acompanhamentoCctLocalizacao)
        {
            _context.AcompanhamentoCctLocalizacao.Remove(acompanhamentoCctLocalizacao);

            await Task.CompletedTask;
        }

        public async ValueTask IncluirAsync(AcompanhamentoCctLocalizacao acompanhamentoCctLocalizacao)
        {
            _context.AcompanhamentoCctLocalizacao.Add(acompanhamentoCctLocalizacao);

            await Task.CompletedTask;
        }

        public async ValueTask<IEnumerable<AcompanhamentoCctLocalizacao>?> ObterPorAcompanhamentoIdAsync(long acompanhamentoId)
        {
            return await _context.AcompanhamentoCctLocalizacao
                .Where(c => c.AcompanhamentoCctId == acompanhamentoId)
                .ToListAsync();
        }
    }
}
