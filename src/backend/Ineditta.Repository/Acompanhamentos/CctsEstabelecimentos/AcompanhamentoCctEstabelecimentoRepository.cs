using Ineditta.Application.Acompanhamentos.CctsEstabelecimentos.Entities;
using Ineditta.Application.Acompanhamentos.CctsEstabelecimentos.Repositories;
using Ineditta.Repository.Contexts;

namespace Ineditta.Repository.Acompanhamentos.CctsEstabelecimentos
{
    public class AcompanhamentoCctEstabelecimentoRepository : IAcompanhamentoCctEstabelecimentoRepository
    {
        private readonly InedittaDbContext _context;

        public AcompanhamentoCctEstabelecimentoRepository(InedittaDbContext context)
        {
            _context = context;
        }

        public async ValueTask IncluirTask(AcompanhamentoCctEstabelecimento acompanhamentoCctEstabelecimento)
        {
            _context.Add(acompanhamentoCctEstabelecimento);

            await Task.CompletedTask;
        }
    }
}
