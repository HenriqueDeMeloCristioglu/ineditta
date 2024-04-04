using System.Text;

using Ineditta.Application.EstruturasClausulas.Gerais.Entities;
using Ineditta.Application.EstruturasClausulas.Gerais.Repositories;
using Ineditta.Repository.Contexts;

using Microsoft.EntityFrameworkCore;

using MySqlConnector;

namespace Ineditta.Repository.EstruturasClausulas.Gerais
{
    public class EstruturaClausulaRepository : IEstruturaClausulaRepository
    {
        private readonly InedittaDbContext _context;

        public EstruturaClausulaRepository(InedittaDbContext context)
        {
            _context = context;
        }

        public async ValueTask<IEnumerable<EstruturaClausula>?> ObterTodasNaoContaDocumento(int documentoId)
        {
            var sql = new StringBuilder(@"SELECT * from estrutura_clausula ec
	                                where not EXISTS (
		                                select 1 from clausula_geral cg
		                                WHERE ec.id_estruturaclausula = cg.estrutura_id_estruturaclausula
		                                and cg.doc_sind_id_documento = @id
	                                );");

            var parameters = new List<MySqlParameter>
            {
                new MySqlParameter("@id", documentoId),
            };

            var result = await _context.EstruturaClausula.FromSqlRaw(sql.ToString(), parameters.ToArray())
                .ToListAsync();

            if (result == null)
            {
                return null;
            }

            return result;
        }

        public async ValueTask<IEnumerable<string>> ObterRelacaoClassificacaoAsync()
        {
            var parameters = new Dictionary<string, object>();

            var query = new StringBuilder(@"
                SELECT
                    JSON_OBJECT('Grupo', gc.nome_grupo, 'Id', ec.id_estruturaclausula, 'Estrutura', ec.nome_clausula) as JsonColuna
                FROM
                    estrutura_clausula ec
                    inner join grupo_clausula gc on gc.idgrupo_clausula = ec.grupo_clausula_idgrupo_clausula
                ORDER BY gc.idgrupo_clausula, ec.id_estruturaclausula
                "
            );

            var classificacaoes = await _context.SelectFromRawSqlAsync<string>(query.ToString(), parameters);

            return classificacaoes;
        }

        public async ValueTask<EstruturaClausula?> ObterPorIdAsync(int id)
        {
            return await _context.EstruturaClausula.Where(e => e.Id == id).SingleOrDefaultAsync();
        }

        public async ValueTask<IEnumerable<EstruturaClausula>?> ObterTodasPorIds(IEnumerable<int> estruturasClausulasIds)
        {
            return await _context.EstruturaClausula.Where(e => estruturasClausulasIds.Any(id => id == e.Id)).ToListAsync();
        }
    }
}
