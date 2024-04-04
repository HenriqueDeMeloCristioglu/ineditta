using Ineditta.BuildingBlocks.Core.Auth;
using Ineditta.Repository.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Ineditta.Repository.Extensions;
using Ineditta.Application.EstruturasClausulas.Gerais.Entities;
using Ineditta.Application.Documentos.Sindicais.Dtos;
using Ineditta.Application.Documentos.Sindicais.Entities;
using Ineditta.Application.Documentos.Sindicais.Repositories;

namespace Ineditta.Repository.Documentos.Sindicais.Repositories
{
    public class DocumentoSindicalRepository : IDocumentoSindicalRepository
    {
        private readonly InedittaDbContext _dbContext;
        private readonly IUserInfoService _userInfoService;

        public DocumentoSindicalRepository(InedittaDbContext dbContext, IUserInfoService userInfoService)
        {
            _dbContext = dbContext;
            _userInfoService = userInfoService;
        }

        public async ValueTask AtualizarAsync(DocumentoSindical documentoSindical, CancellationToken cancellationToken = default)
        {
            _dbContext.DocSinds.Update(documentoSindical);
            await Task.CompletedTask;
        }

        public async ValueTask<bool> ExistePorIdAsync(long id)
        {
            return await _dbContext.DocSinds.AnyAsync(ds => ds.Id == id);
        }

        public async ValueTask IncluirAsync(DocumentoSindical documentoSindical, CancellationToken cancellationToken = default)
        {
            _dbContext.DocSinds.Add(documentoSindical);

            var email = _userInfoService.GetEmail()!;

            var usuarioId = await _dbContext.UsuarioAdms.Where(x => x.Email.Valor == email).Select(x => x.Id).SingleAsync(cancellationToken);

            _dbContext.Entry(documentoSindical).Property("UsuarioCadastroId").CurrentValue = usuarioId;
            _dbContext.Entry(documentoSindical).Property("DataCadastro").CurrentValue = DateOnly.FromDateTime(DateTime.Now);


            await Task.CompletedTask;
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

        public async ValueTask<InfoDocumentoCriadoDto?> ObterInfoDocumentoCriadoEmail(long idDocumento)
        {
            var result = await (from ds in _dbContext.DocSinds
                                join td in _dbContext.TipoDocs on ds.TipoDocumentoId equals td.Id into _td
                                from td in _td.DefaultIfEmpty()
                                where ds.Id == (int)idDocumento
                                select new InfoDocumentoCriadoDto
                                {
                                    Estabelecimentos = ds.Estabelecimentos,
                                    Comentarios = ds.Observacao,
                                    Abrangencia = ds.Abrangencias,
                                    FonteLegislacaoSite = ds.FonteWeb,
                                    NumeroLegislacao = ds.NumeroLei,
                                    Restrito = ds.Restrito,
                                    NomeDocumento = td.Nome,
                                    SindicatosLaborais = (from dsl in _dbContext.DocumentosSindicatosLaborais
                                                          join sinde in _dbContext.SindEmps on dsl.SindicatoLaboralId equals sinde.Id
                                                          where dsl.DocumentoSindicalId == idDocumento
                                                          select new SindicatoLaboral
                                                          {
                                                              Id = sinde.Id,
                                                              Cnpj = sinde.Cnpj.Value,
                                                              Codigo = sinde.CodigoSindical.Valor,
                                                              Municipio = sinde.Municipio,
                                                              Sigla = sinde.Sigla,
                                                              Uf = sinde.Uf,
                                                              Denominacao = sinde.Denominacao
                                                          }).AsSplitQuery().ToList(),
                                    SindicatosPatronais = (from dsp in _dbContext.DocumentosSindicatosPatronais
                                                           join sindp in _dbContext.SindPatrs on dsp.SindicatoPatronalId equals sindp.Id
                                                           where dsp.DocumentoSindicalId == idDocumento
                                                           select new SindicatoPatronal
                                                           {
                                                               Id = sindp.Id,
                                                               Cnpj = sindp.Cnpj.Value,
                                                               Codigo = sindp.CodigoSindical.Valor,
                                                               Municipio = sindp.Municipio,
                                                               Sigla = sindp.Sigla,
                                                               Uf = sindp.Uf,
                                                               Denominacao = sindp.Denominacao
                                                           }).AsSplitQuery().ToList(),
                                    ValidadeFinal = ds.DataValidadeFinal,
                                    ValidadeInicial = ds.DataValidadeInicial,
                                    OrigemDocumento = ds.Origem,
                                    AssuntosIds = ds.Referencias,
                                    AtividadesEconomicas = ds.Cnaes,
                                }).FirstOrDefaultAsync();

            if (result is null) return result;

            var assuntos = await _dbContext.EstruturaClausula.ToListAsync();
            if (assuntos is not null && assuntos.Any())
            {
                var assuntosIds = result.AssuntosIds is not null ? result.AssuntosIds.Select(id => int.TryParse(id, out int idInt) ? idInt : 0) : new List<int>();
                if (assuntosIds is null || !assuntosIds.Any())
                {
                    assuntos = new List<EstruturaClausula>();
                }
                else
                {
                    assuntos = assuntos.Where(a => assuntosIds.Contains(a.Id)).ToList();
                }
            }

            result.Assuntos = assuntos is not null ? string.Join(", ", assuntos.Select(a => a.Nome)) : "";

            return result;

        }
        public async ValueTask<DocumentoSindical?> ObterPorIdAsync(long id)
        {
            return await _dbContext.DocSinds.FindAsync((int)id);
        }
    }
}
