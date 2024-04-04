using System.Text;

using Ineditta.Application.Cnaes.Entities;
using Ineditta.Application.Cnaes.Repositories;
using Ineditta.Repository.Contexts;
using Ineditta.Repository.Extensions;

using Microsoft.EntityFrameworkCore;

using MySqlConnector;

namespace Ineditta.Repository.ClassesCnaes
{
    public class CnaeRepository : ICnaeRepository
    {
        private readonly InedittaDbContext _context;

        public CnaeRepository(InedittaDbContext context)
        {
            _context = context;
        }

        public async ValueTask AtualizarAsync(Cnae cnae)
        {
            _context.ClasseCnaes.Update(cnae);

            await Task.CompletedTask;
        }

        public async ValueTask IncluirAsync(Cnae cnae)
        {
            _context.ClasseCnaes.Add(cnae);

            await Task.CompletedTask;
        }

        public async ValueTask<Cnae?> ObterPorIdAsync(int id)
        {
            return await _context.ClasseCnaes.FindAsync(id);
        }

        public async ValueTask<IEnumerable<Cnae>?> ObterPorListaIdAsync(IEnumerable<int> ids)
        {
            return await _context.ClasseCnaes.Where(c => ids.Any(id => c.Id == id)).ToListAsync();
        }

        public async ValueTask<IEnumerable<Cnae>?> ObterPorUnidadesIds(IEnumerable<int> unidadesIds)
        {
            var parameters = new List<MySqlParameter>();
            var parametersCount = 0;

            var query = new StringBuilder(@"
                SELECT cc.* FROM cnae_emp ce 
                LEFT JOIN classe_cnae cc ON cc.id_cnae = ce.classe_cnae_idclasse_cnae 
                WHERE TRUE
            ");

            QueryBuilder.AppendListToQueryBuilder(query, unidadesIds, "ce.cliente_unidades_id_unidade", parameters, ref parametersCount);

            var cnaes = await _context.ClasseCnaes.FromSqlRaw(query.ToString(), parameters.ToArray()).ToListAsync();

            return cnaes;
        }
    }
}
