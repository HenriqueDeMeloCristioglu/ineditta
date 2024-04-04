using CSharpFunctionalExtensions;

using Ineditta.Application.Documentos.AtividadesEconomicas.Repositories;
using Ineditta.Repository.Contexts;

using Microsoft.Extensions.Logging;

namespace Ineditta.Repository.Documentos.AtividadesEconomicas
{
    public class DocumentoAtividadeEconomicaRepository : IDocumentoAtividadeEconomicaRepository
    {
        private readonly InedittaDbContext _context;
        private readonly ILogger<DocumentoAtividadeEconomicaRepository> _logger;

        public DocumentoAtividadeEconomicaRepository(InedittaDbContext context, ILogger<DocumentoAtividadeEconomicaRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async ValueTask<Result> IncluirAtividadeEconomicaDocumento()
        {
            var result = await _context.InserirCnaesDocumentos();

            _logger.LogError("Cnaes Documentos inseridos com sucesso");

            if (!result)
            {
                _logger.LogError("Erro ao inserir cnaes documentos");

                return Result.Failure("Erro ao inserir cnaes documentos");
            }

            return Result.Success();
        }
    }
}
