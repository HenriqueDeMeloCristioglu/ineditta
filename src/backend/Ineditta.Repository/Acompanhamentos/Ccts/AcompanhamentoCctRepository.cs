using CSharpFunctionalExtensions;

using Ineditta.Application.Acompanhamentos.Ccts.Entities;
using Ineditta.Application.Acompanhamentos.Ccts.Repositories;
using Ineditta.Repository.Contexts;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Ineditta.Repository.Acompanhamentos.Ccts
{
    public class AcompanhamentoCctRepository : IAcompanhamentoCctRepository
    {
        private readonly InedittaDbContext _context;
        private readonly ILogger<IAcompanhamentoCctRepository> _logger;

        public AcompanhamentoCctRepository(InedittaDbContext context, ILogger<IAcompanhamentoCctRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async ValueTask AtualizarAsync(AcompanhamentoCct acompanhamentoCct)
        {
            _context.AcompanhamentoCct.Update(acompanhamentoCct);

            await Task.CompletedTask;
        }

        public async ValueTask IncluirAsync(AcompanhamentoCct acompanhamentoCct)
        {
            _context.AcompanhamentoCct.Add(acompanhamentoCct);

            await Task.CompletedTask;
        }

        public async ValueTask<Result> IncluirEstabelecimentos()
        {
            var result = await _context.InserirEstabelecimentosAcompanhamentoProcedure();

            _logger.LogError("Estabelecimentos e sindicatos inseridos com sucesso");

            if (!result)
            {
                _logger.LogError("Erro ao inserir estabelecimentos e sindicatos");

                return Result.Failure("Erro ao inserir estabelecimentos e sindicatos");
            }

            return Result.Success();
        }

        public async ValueTask<AcompanhamentoCct?> ObterPorIdAsync(long id)
        {
            return await _context.AcompanhamentoCct
                .Include(x => x.Assuntos)
                .Include(x => x.Etiquetas)
                .AsSplitQuery()
                .SingleOrDefaultAsync(a => a.Id == id);
        }

        public async ValueTask<IEnumerable<AcompanhamentoCct>?> ObterPorIdsAsync(IEnumerable<long> ids)
        {
            return await _context.AcompanhamentoCct
                .Include(x => x.Assuntos)
                .Include(x => x.Etiquetas)
                .AsSplitQuery()
                .Where(a => ids.Any(id => id == a.Id))
                .ToListAsync();
        }
    }
}
