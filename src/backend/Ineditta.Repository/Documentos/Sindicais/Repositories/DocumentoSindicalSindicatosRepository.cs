using CSharpFunctionalExtensions;

using Ineditta.Application.Documentos.Sindicais.Repositories;
using Ineditta.Repository.Contexts;

namespace Ineditta.Repository.Documentos.Sindicais.Repositories
{
    public class DocumentoSindicalSindicatosRepository : IDocumentoSindicalSindicatosRepository
    {
        private readonly InedittaDbContext _context;

        public DocumentoSindicalSindicatosRepository(InedittaDbContext context)
        {
            _context = context;
        }

        public async ValueTask LimparRelacionamentosDocumentoSindicato()
        {
            await _context.SelectFromRawSqlAsync<dynamic>(@"
                TRUNCATE documento_sindicato_laboral_tb;
                TRUNCATE documento_sindicato_patronal_tb;
            ", new Dictionary<string, object>());
        }

        public async ValueTask<Result> RefazerDocumentosSindicatos()
        {
            _ = await _context.SelectFromRawSqlAsync<dynamic>(@"
                TRUNCATE documento_sindicato_laboral_tb;
                TRUNCATE documento_sindicato_patronal_tb;
                CALL inserir_documento_sindicatos();
            ", new Dictionary<string, object>());

            return Result.Success();
        }
    }
}
