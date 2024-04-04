using System.Text;

using Ineditta.API.ViewModels.Shared;
using Ineditta.API.ViewModels.Sindicatos;
using Ineditta.API.ViewModels.Sindicatos.Dtos;
using Ineditta.API.ViewModels.Sindicatos.ViewModels;
using Ineditta.Application.Usuarios.Entities;
using Ineditta.BuildingBlocks.Core.Auth;
using Ineditta.BuildingBlocks.Core.Extensions;
using Ineditta.Repository.Contexts;
using Ineditta.Repository.Extensions;

using Microsoft.EntityFrameworkCore;

using MySqlConnector;

namespace Ineditta.API.Factories.Sindicatos
{
    public class SindicatoPatronalRelatorioFactory
    {
        private readonly InedittaDbContext _context;
        private readonly IUserInfoService _userInfoService;

        public SindicatoPatronalRelatorioFactory(InedittaDbContext context, IUserInfoService userInfoService)
        {
            _userInfoService = userInfoService;
            _context = context;
        }

        public async ValueTask<IEnumerable<RelatorioSindicatoViewModel>> CriarAsync(SindicatosRequest request)
        {
            var parameters = new List<MySqlParameter>();
            int parametersCount = 0;

            var query = new StringBuilder(@"select * from sindicato_vw vw
                                                inner join usuario_adm uat on uat.email_usuario = @email
                                                where JSON_CONTAINS(vw.cnae_unidade, concat('{{""id"":', vw.classe_cnae_idclasse_cnae,'}}'))
                                                    and case WHEN uat.nivel = @nivel then true 
                                                            when uat.nivel = @nivelGrupoEconomico then uat.id_grupoecon = vw.cliente_grupo_id_grupo_economico
                                                            else json_contains(uat.ids_fmge, JSON_ARRAY(vw.id_unidade)) end");

            parameters.Add(new MySqlParameter("@email", _userInfoService.GetEmail()));
            parameters.Add(new MySqlParameter("@nivel", nameof(Nivel.Ineditta)));
            parameters.Add(new MySqlParameter("@nivelGrupoEconomico", Nivel.GrupoEconomico.GetDescription()));

            if (request.GrupoEconomicoId > 0)
            {
                query.Append(" and vw.cliente_grupo_id_grupo_economico = @grupoEconomicoId");
                parameters.Add(new MySqlParameter("@grupoEconomicoId", request.GrupoEconomicoId));
            }

            if (request.MatrizesIds != null && request.MatrizesIds.Any())
            {
                QueryBuilder.AppendListToQueryBuilder(query, request.MatrizesIds, "vw.cliente_matriz_id_empresa", parameters, ref parametersCount);
            }

            if (request.ClientesUnidadesIds != null && request.ClientesUnidadesIds.Any())
            {
                QueryBuilder.AppendListToQueryBuilder(query, request.ClientesUnidadesIds, "vw.id_unidade", parameters, ref parametersCount);
            }

            if (request.LocalizacoesIds != null && request.LocalizacoesIds.Any())
            {
                QueryBuilder.AppendListToQueryBuilder(query, request.LocalizacoesIds, "vw.id_localizacao", parameters, ref parametersCount);
            }

            if (request.Ufs != null && request.Ufs.Any())
            {
                QueryBuilder.AppendListToQueryBuilder(query, request.Ufs, "vw.uf", parameters, ref parametersCount);
            }

            if (request.Regioes != null && request.Regioes.Any())
            {
                QueryBuilder.AppendListToQueryBuilder(query, request.Regioes, "vw.regiao", parameters, ref parametersCount);
            }

            if (request.CnaesIds != null && request.CnaesIds.Any())
            {
                QueryBuilder.AppendListJsonToQueryBuilder(query, request.CnaesIds, "vw.cnae_unidade", "id", parameters, ref parametersCount);
            }

            if (request.SindLaboraisIds != null && request.SindLaboraisIds.Any())
            {
                QueryBuilder.AppendListToQueryBuilder(query, request.SindLaboraisIds, "vw.id_sinde", parameters, ref parametersCount);
            }

            if (request.SindPatronaisIds != null && request.SindPatronaisIds.Any())
            {
                QueryBuilder.AppendListToQueryBuilder(query, request.SindPatronaisIds, "vw.sind_patronal_id_sindp", parameters, ref parametersCount);
            }

            if (request.DataBase != null && request.DataBase.Any())
            {
                QueryBuilder.AppendListToQueryBuilder(query, request.DataBase, "vw.dataneg", parameters, ref parametersCount);
            }

            query.Append(@" group by vw.sind_patronal_cnpj");

            var result = await _context.SindicatoVw.FromSqlRaw(query.ToString(), parameters.ToArray())
                                .AsNoTracking()
                                .Select(vw => new RelatorioSindicatoViewModel
                                {
                                    SindPatronalSituacao = vw.SindPatronalSituacao,
                                    SindPatronalCnpj = vw.SindPatronalCnpj,
                                    SindPatronalCodigo = vw.SindPatronalCodigo,
                                    SindPatronalSigla = vw.SindPatronalSigla,
                                    SindPatronalRazao = vw.SindPatronalRazao,
                                    SindPatronalDenominacao = vw.SindPatronalDenominacao,
                                    SindPatronalLogradouro = vw.SindPatronalLogradouro,
                                    SindPatronalMunicipio = vw.SindPatronalMunicipio,
                                    SindPatronalUf = vw.SindPatronalUf,
                                    SindPatronalFone1 = vw.SindPatronalFone1,
                                    SindPatronalFone2 = vw.SindPatronalFone2,
                                    SindPatronalFone3 = vw.SindPatronalFone3,
                                    SindPatronalRamal = vw.SindPatronalRamal,
                                    SindPatronalNegociador = vw.SindPatronalNegociador,
                                    SindPatronalContribuicao = vw.SindPatronalContribuicao,
                                    SindPatronalEnquadramento = vw.SindPatronalEnquadramento,
                                    SindPatronalEmail1 = vw.SindPatronalEmail1,
                                    SindPatronalEmail2 = vw.SindPatronalEmail2,
                                    SindPatronalEmail3 = vw.SindPatronalEmail3,
                                    SindPatronalSite = vw.SindPatronalSite,
                                    SindPatronalTwitter = vw.SindPatronalTwitter,
                                    SindPatronalFacebook = vw.SindPatronalFacebook,
                                    SindPatronalInstagram = vw.SindPatronalInstagram,
                                    SindPatronalGrau = vw.SindPatronalGrau,
                                    SindPatronalStatus = vw.SindPatronalStatus,
                                    FederacaoPatronalSigla = vw.FederacaoPatronalSigla,
                                    FederacaoPatronalNome = vw.FederacaoPatronalNome,
                                    FederacaoPatronalCnpj = vw.FederacaoPatronalCnpj,
                                    ConfederacaoPatronalSigla = vw.ConfederacaoPatronalSigla,
                                    ConfederacaoPatronalNome = vw.ConfederacaoPatronalNome,
                                    ConfederacaoPatronalCnpj = vw.ConfederacaoPatronalCnpj,
                                }).ToListAsync();
            if (result is null)
            {
                return Enumerable.Empty<RelatorioSindicatoViewModel>();
            }

            return result;
        }
    }
}
