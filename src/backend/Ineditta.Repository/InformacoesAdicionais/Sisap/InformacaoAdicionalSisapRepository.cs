using Ineditta.Application.InformacoesAdicionais.Sisap.Entities;
using Ineditta.Application.InformacoesAdicionais.Sisap.Repositiories;
using Ineditta.Repository.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Ineditta.Repository.InformacoesAdicionais.Sisap
{
    public class InformacaoAdicionalSisapRepository : IInformacaoAdicionalSisapRepository
    {
        private readonly InedittaDbContext _context;

        public InformacaoAdicionalSisapRepository(InedittaDbContext context)
        {
            _context = context;
        }

        public async ValueTask AtualizarAsync(InformacaoAdicionalSisap informacaoAdicionalSisap)
        {
            _context.InformacaoAdicionalSisap.Update(informacaoAdicionalSisap);

            await Task.CompletedTask;
        }

        public async ValueTask ExcluirAsync(InformacaoAdicionalSisap informacaoAdicionalSisap)
        {
            _context.InformacaoAdicionalSisap.Remove(informacaoAdicionalSisap);

            await Task.CompletedTask;
        }

        public async ValueTask IncluirAsync(InformacaoAdicionalSisap informacaoAdicionalSisap)
        {
            await _context.InformacaoAdicionalSisap.AddAsync(informacaoAdicionalSisap);
        }

        public async ValueTask IncluirMuitosAsync(IEnumerable<InformacaoAdicionalSisap> informacosAdicionaisSisap)
        {
            await _context.InformacaoAdicionalSisap.AddRangeAsync(informacosAdicionaisSisap);
        }

        public async ValueTask<List<InformacaoAdicionalSisap>?> ObterPorClausulaId(int clausulaId)
        {
            return await _context.InformacaoAdicionalSisap.Where(i => i.ClausulaGeralId == clausulaId).ToListAsync();
        }
    }
}
