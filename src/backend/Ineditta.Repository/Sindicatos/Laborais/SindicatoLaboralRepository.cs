using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Ineditta.Application.Sindicatos.Laborais.Entities;
using Ineditta.Application.Sindicatos.Laborais.Repositories;
using Ineditta.BuildingBlocks.Core.Domain.ValueObjects;
using Ineditta.Repository.Contexts;
using Ineditta.Repository.Extensions;

using Microsoft.EntityFrameworkCore;

using MySqlConnector;

namespace Ineditta.Repository.Sindicatos.Laborais
{
    public class SindicatoLaboralRepository : ISindicatoLaboralRepository
    {
        private readonly InedittaDbContext _context;

        public SindicatoLaboralRepository(InedittaDbContext context)
        {
            _context = context;
        }
        public async ValueTask AtualizarAsync(SindicatoLaboral sindicato)
        {
            _context.SindEmps.Update(sindicato);
            await Task.CompletedTask;
        }

        public async ValueTask IncluirAsync(SindicatoLaboral sindicato)
        {
            _context.SindEmps.Add(sindicato);
            await Task.CompletedTask;
        }

        public async ValueTask<SindicatoLaboral?> ObterPorCNPJAsync(CNPJ cnpj)
        {
            return await _context.SindEmps.FirstOrDefaultAsync(sp => sp.Cnpj.Value == cnpj.Value);
        }

        public async ValueTask<SindicatoLaboral?> ObterPorIdAsync(int id)
        {
            return await _context.SindEmps.FirstOrDefaultAsync(sp => sp.Id == id);
        }

        public async ValueTask<bool> ExisteAsync(CNPJ cnpj, int ignorarId = 0)
        {
            return await _context.SindEmps.AnyAsync(c => c.Cnpj.Value == cnpj.Value && c.Id != ignorarId);
        }

        public async ValueTask<IEnumerable<SindicatoLaboral>?> ObterPorClientesUnidadeIdsAsync(IEnumerable<int> clientesUnidadeIds)
        {
            var parameters = new List<MySqlParameter>();
            var parametersCount = 0;

            var query = new StringBuilder(@"
                SELECT se.* FROM sind_emp se 
                LEFT JOIN base_territorialsindemp bt ON bt.sind_empregados_id_sinde1 = se.id_sinde 
                WHERE EXISTS (
	                SELECT * FROM cliente_unidades cut
	                LEFT JOIN cnae_emp ce ON cut.id_unidade = ce.cliente_unidades_id_unidade
	                WHERE ce.classe_cnae_idclasse_cnae = bt.classe_cnae_idclasse_cnae 
	                        AND cut.localizacao_id_localizacao = bt.localizacao_id_localizacao1 
            ");

            QueryBuilder.AppendListToQueryBuilder(query, clientesUnidadeIds, "cut.id_unidade", parameters, ref parametersCount);

            query.Append(@"
                )
                GROUP BY se.id_sinde  
            ");

            var sindicatosLaborais = await _context.SindEmps.FromSqlRaw(query.ToString(), parameters.ToArray()).ToListAsync();

            return sindicatosLaborais;
        }

        public async ValueTask<IEnumerable<SindicatoLaboral>?> ObterPorListaIdsAsync(IEnumerable<int> ids)
        {
            return await _context.SindEmps.Where(s => ids.Any(id => s.Id == id)).ToListAsync();
        }
    }
}
