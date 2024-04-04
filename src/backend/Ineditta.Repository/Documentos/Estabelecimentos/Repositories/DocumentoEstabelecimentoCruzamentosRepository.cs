using System.Text;

using Ineditta.Application.Documentos.Estabelecimentos.Repositories;
using Ineditta.Application.Documentos.Sindicais.Dtos;
using Ineditta.Repository.Contexts;
using Ineditta.Repository.Extensions;

namespace Ineditta.Repository.Documentos.Estabelecimentos.Repositories
{
    public class DocumentoEstabelecimentoCruzamentosRepository : IDocumentoEstabelecimentoCruzamentosRepository
    {
        private readonly InedittaDbContext _dbContext;

        public DocumentoEstabelecimentoCruzamentosRepository(InedittaDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async ValueTask<IEnumerable<Estabelecimento>?> ObterEstabelecimentosDocumentoPorCruzamento(IEnumerable<int>? cnaesIds, IEnumerable<int>? abrangenciaIds, IEnumerable<int>? sindicatosPatronaisIds, IEnumerable<int>? sindicatosLaboraisIds, string mesDataBaseDoc)
        {
            var parameters = new Dictionary<string, object>();

            var query = new StringBuilder(@"
                SELECT
                    id_unidade Id, 
                    nome_unidade Nome, 
                    cliente_matriz_id_empresa EmpresaId,
                    cliente_grupo_id_grupo_economico GrupoEconomicoId
                FROM cliente_unidades cut
                WHERE TRUE
            ");

            if (abrangenciaIds is not null && abrangenciaIds.Any())
            {
                QueryBuilder.AppendListToQueryBuilder(query, abrangenciaIds, "cut.localizacao_id_localizacao", parameters);
            }
            else
            {
                query.Append(" AND FALSE");
            }

            if (cnaesIds is not null && cnaesIds.Any())
            {
                query.Append(@"
                    AND EXISTS (
	  	                SELECT * FROM cnae_emp ce
	  	                WHERE ce.cliente_unidades_id_unidade = cut.id_unidade
                ");

                QueryBuilder.AppendListToQueryBuilder(query, cnaesIds, "ce.classe_cnae_idclasse_cnae", parameters);

                query.Append(')');
            }
            else
            {
                query.Append(" AND FALSE");
            }

            if (sindicatosLaboraisIds is not null && sindicatosLaboraisIds.Any())
            {
                query.Append(@"
                    AND EXISTS (
	  	                SELECT * FROM base_territorialsindemp bt2 
	  	                WHERE bt2.localizacao_id_Localizacao1 = cut.localizacao_id_localizacao                  
                ");

                QueryBuilder.AppendListToQueryBuilder(query, sindicatosLaboraisIds, "bt2.sind_empregados_id_sinde1", parameters);

                if (cnaesIds is not null && cnaesIds.Any())
                {
                    QueryBuilder.AppendListToQueryBuilder(query, cnaesIds, "bt2.classe_cnae_idclasse_cnae", parameters);
                }

                query.Append(@"
                    AND bt2.dataneg = @dataBaseDoc
                ");

                parameters.Add("@dataBaseDoc", mesDataBaseDoc);

                query.Append(')');
            }

            if (sindicatosPatronaisIds is not null && sindicatosPatronaisIds.Any())
            {
                query.Append(@"
                    AND EXISTS (
	  	                SELECT * FROM base_territorialsindpatro bt 
	  	                WHERE bt.localizacao_id_localizacao1 = cut.localizacao_id_localizacao               
                ");

                QueryBuilder.AppendListToQueryBuilder(query, sindicatosPatronaisIds, "bt.sind_patronal_id_sindp", parameters);

                if (cnaesIds is not null && cnaesIds.Any())
                {
                    QueryBuilder.AppendListToQueryBuilder(query, cnaesIds, "bt.classe_cnae_idclasse_cnae", parameters);
                }
                query.Append(')');
            }
            var sql = query.ToString();

            var estabelecimentos = await _dbContext.SelectFromRawSqlAsync<Estabelecimento>(sql, parameters);

            return estabelecimentos;
        }

        public async ValueTask<IEnumerable<Estabelecimento>?> ObterEstabelecimentosDocumentoPorIds(IEnumerable<int> estabelecimentosIds)
        {
            var parameters = new Dictionary<string, object>();

            var query = new StringBuilder(@"
                    SELECT 
                        id_unidade Id, 
                        nome_unidade Nome, 
                        cliente_matriz_id_empresa EmpresaId,
                        cliente_grupo_id_grupo_economico GrupoEconomicoId
                    FROM cliente_unidades 
                    WHERE true
                "
            );

            QueryBuilder.AppendListToQueryBuilder(query, estabelecimentosIds, "id_unidade", parameters);

            var estabelecimentos = await _dbContext.SelectFromRawSqlAsync<Estabelecimento>(query.ToString(), parameters);

            return estabelecimentos;
        }

        public async ValueTask<IEnumerable<Estabelecimento>?> ObterEstabelecimentosDocumentoComercialPorCruzamento(IEnumerable<int>? cnaesIds, IEnumerable<int>? abrangenciaIds, IEnumerable<int>? sindicatosPatronaisIds, IEnumerable<int>? sindicatosLaboraisIds, string emailUsuario)
        {
            var parameters = new Dictionary<string, object>();

            var query = new StringBuilder(@"
                SELECT
                    id_unidade Id, 
                    nome_unidade Nome, 
                    cliente_matriz_id_empresa EmpresaId,
                    cliente_grupo_id_grupo_economico GrupoEconomicoId
                FROM cliente_unidades cut
                INNER JOIN usuario_adm uat ON uat.email_usuario = @emailUsuario
                WHERE CASE WHEN uat.nivel = 'Ineditta' THEN TRUE
		                   WHEN uat.nivel = 'Grupo Econômico' THEN uat.id_grupoecon = cut.cliente_grupo_id_grupo_economico
		                   ELSE JSON_CONTAINS(uat.ids_fmge, JSON_ARRAY(cut.id_unidade))
                      END
            ");

            parameters.Add("@emailUsuario", emailUsuario);

            if (abrangenciaIds is not null && abrangenciaIds.Any())
            {
                QueryBuilder.AppendListToQueryBuilder(query, abrangenciaIds, "cut.localizacao_id_localizacao", parameters);
            }

            if (cnaesIds is not null && cnaesIds.Any())
            {
                query.Append(@"
                    AND EXISTS (
	  	                SELECT * FROM cnae_emp ce
	  	                WHERE ce.cliente_unidades_id_unidade = cut.id_unidade
                ");

                QueryBuilder.AppendListToQueryBuilder(query, cnaesIds, "ce.classe_cnae_idclasse_cnae", parameters);

                query.Append(')');
            }

            if (sindicatosLaboraisIds is not null && sindicatosLaboraisIds.Any())
            {
                query.Append(@"
                    AND EXISTS (
	  	                SELECT * FROM base_territorialsindemp bt2 
	  	                WHERE bt2.localizacao_id_Localizacao1 = cut.localizacao_id_localizacao
                              AND bt2.classe_cnae_idclasse_cnae IN (
                                    SELECT ce.classe_cnae_idclasse_cnae FROM cnae_emp ce
	  	                            WHERE ce.cliente_unidades_id_unidade = cut.id_unidade
                              ) 
                ");

                QueryBuilder.AppendListToQueryBuilder(query, sindicatosLaboraisIds, "bt2.sind_empregados_id_sinde1", parameters);

                if (cnaesIds is not null && cnaesIds.Any())
                {
                    QueryBuilder.AppendListToQueryBuilder(query, cnaesIds, "bt2.classe_cnae_idclasse_cnae", parameters);
                }

                query.Append(')');
            }

            if (sindicatosPatronaisIds is not null && sindicatosPatronaisIds.Any())
            {
                query.Append(@"
                    AND EXISTS (
	  	                SELECT * FROM base_territorialsindpatro bt 
	  	                WHERE bt.localizacao_id_localizacao1 = cut.localizacao_id_localizacao
                              AND bt.classe_cnae_idclasse_cnae IN (
                                    SELECT ce.classe_cnae_idclasse_cnae FROM cnae_emp ce
	  	                            WHERE ce.cliente_unidades_id_unidade = cut.id_unidade
                              )
                ");

                QueryBuilder.AppendListToQueryBuilder(query, sindicatosPatronaisIds, "bt.sind_patronal_id_sindp", parameters);

                if (cnaesIds is not null && cnaesIds.Any())
                {
                    QueryBuilder.AppendListToQueryBuilder(query, cnaesIds, "bt.classe_cnae_idclasse_cnae", parameters);
                }
                query.Append(')');
            }

            var sql = query.ToString();

            var estabelecimentos = await _dbContext.SelectFromRawSqlAsync<Estabelecimento>(sql, parameters);

            return estabelecimentos;
        }

    }
}
