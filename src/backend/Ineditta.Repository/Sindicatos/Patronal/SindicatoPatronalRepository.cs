using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Ineditta.Application.Sindicatos.Patronais.Entities;
using Ineditta.Application.Sindicatos.Patronais.Repositories;
using Ineditta.BuildingBlocks.Core.Domain.ValueObjects;
using Ineditta.Repository.Contexts;
using Ineditta.Repository.Extensions;

using Microsoft.EntityFrameworkCore;

using MySqlConnector;

namespace Ineditta.Repository.Sindicatos.Patronal
{
    public sealed class SindicatoPatronalRepository : ISindicatoPatronalRepository
    {
        private readonly InedittaDbContext _context;

        public SindicatoPatronalRepository(InedittaDbContext context)
        {
            _context = context; 
        }
        public async ValueTask AtualizarAsync(SindicatoPatronal sindicato)
        {
            _context.SindPatrs.Update(sindicato);
            await Task.CompletedTask;
        }

        public async ValueTask IncluirAsync(SindicatoPatronal sindicato)
        {
            _context.SindPatrs.Add(sindicato);
            await Task.CompletedTask;
        }

        public async ValueTask<SindicatoPatronal?> ObterPorCNPJAsync(CNPJ cnpj)
        {
            return await _context.SindPatrs.FirstOrDefaultAsync(sp => sp.Cnpj.Value == cnpj.Value);
        }

        public async ValueTask<SindicatoPatronal?> ObterPorIdAsync(int id)
        {
            return await _context.SindPatrs.FirstOrDefaultAsync(sp => sp.Id == id);
        }

        public async ValueTask<bool> ExisteAsync(CNPJ cnpj, int ignorarId = 0)
        {
            return await _context.SindPatrs.AnyAsync(c => c.Cnpj.Value == cnpj.Value && c.Id != ignorarId);
        }

        public async ValueTask<bool> ExistePorIdAsync(int sindicatoPatronalId)
        {
            return await _context.SindPatrs.AnyAsync(sp => sp.Id == sindicatoPatronalId);
        }

        public async ValueTask<IEnumerable<SindicatoPatronal>?> ObterPorClienteUnidadesIdsAsync(IEnumerable<int> clientesUnidadesIds)
        {
            var parameters = new List<MySqlParameter>();
            var parametersCount = 0;

            var query = new StringBuilder(@"
                SELECT sp.* FROM sind_patr sp
                LEFT JOIN base_territorialsindpatro bt ON sp.id_sindp = bt.sind_patronal_id_sindp 
                WHERE EXISTS (
	                SELECT * FROM cliente_unidades cut
	                LEFT JOIN cnae_emp ce ON cut.id_unidade = ce.cliente_unidades_id_unidade
	                WHERE ce.classe_cnae_idclasse_cnae = bt.classe_cnae_idclasse_cnae 
	                      AND cut.localizacao_id_localizacao = bt.localizacao_id_localizacao1
            ");

            QueryBuilder.AppendListToQueryBuilder(query, clientesUnidadesIds, "cut.id_unidade", parameters, ref parametersCount);

            query.Append(@"
                )
                GROUP BY sp.id_sindp   
            ");

            var sindicatosPatronais = await _context.SindPatrs.FromSqlRaw(query.ToString(), parameters.ToArray()).ToListAsync();

            return sindicatosPatronais;

        }

        public async ValueTask<IEnumerable<SindicatoPatronal>?> ObterPorListaIdAsync(IEnumerable<int> ids)
        {
            return await _context.SindPatrs.Where(s => ids.Any(id => s.Id == id)).ToListAsync();
        }
    }
}
