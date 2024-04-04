using System.Text;

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
    public class SindicatoLaboralRelatorioFactory
    {
        private readonly InedittaDbContext _context;
        private readonly IUserInfoService _userInfoService;

        public SindicatoLaboralRelatorioFactory(InedittaDbContext context, IUserInfoService userInfoService)
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

            query.Append(@" group by vw.sind_laboral_cnpj");

            var result = await _context.SindicatoVw.FromSqlRaw(query.ToString(), parameters.ToArray())
                                .AsNoTracking()
                                .Select(vw => new RelatorioSindicatoViewModel
                                {
                                    SindLaboralSituacao = vw.SindLaboralSituacao,
                                    SindLaboralCnpj = vw.SindLaboralCnpj,
                                    SindLaboralCodigo = vw.SindLaboralCodigo,
                                    SindLaboralSigla = vw.SindLaboralSigla,
                                    SindLaboralRazao = vw.SindLaboralRazao,
                                    SindLaboralDenominacao = vw.SindLaboralDenominacao,
                                    SindLaboralLogradouro = vw.SindLaboralLogradouro,
                                    SindLaboralMunicipio = vw.SindLaboralMunicipio,
                                    SindLaboralUf = vw.SindLaboralUf,
                                    SindLaboralFone1 = vw.SindLaboralFone1,
                                    SindLaboralFone2 = vw.SindLaboralFone2,
                                    SindLaboralFone3 = vw.SindLaboralFone3,
                                    SindLaboralRamal = vw.SindLaboralRamal,
                                    SindLaboralNegociador = vw.SindLaboralNegociador,
                                    SindLaboralContribuicao = vw.SindLaboralContribuicao,
                                    SindLaboralEnquadramento = vw.SindLaboralEnquadramento,
                                    SindLaboralEmail1 = vw.SindLaboralEmail1,
                                    SindLaboralEmail2 = vw.SindLaboralEmail2,
                                    SindLaboralEmail3 = vw.SindLaboralEmail3,
                                    SindLaboralSite = vw.SindLaboralSite,
                                    SindLaboralTwitter = vw.SindLaboralTwitter,
                                    SindLaboralFacebook = vw.SindLaboralFacebook,
                                    SindLaboralInstagram = vw.SindLaboralInstagram,
                                    SindLaboralGrau = vw.SindLaboralGrau,
                                    SindLaboralStatus = vw.SindLaboralStatus,
                                    SindLaboralDataNegociacao = vw.SindLaboralDataNegociacao,
                                    FederacaoLaboralSigla = vw.FederacaoLaboralSigla,
                                    FederacaoLaboralNome = vw.FederacaoLaboralNome,
                                    FederacaoLaboralCnpj = vw.FederacaoLaboralCnpj,
                                    ConfederacaoLaboralSigla = vw.ConfederacaoLaboralSigla,
                                    ConfederacaoLaboralNome = vw.ConfederacaoLaboralNome,
                                    ConfederacaoLaboralCnpj = vw.ConfederacaoLaboralCnpj,
                                    CentralSindicalSigla = vw.CentralSindicalSigla,
                                    CentralSindicalNome = vw.CentralSindicalNome,
                                    CentralSindicalCnpj = vw.CentralSindicalCnpj,
                                }).ToListAsync();
            if (result is null)
            {
                return Enumerable.Empty<RelatorioSindicatoViewModel>();
            }

            var datasNegociacoesDictionary = new Dictionary<string, string>();

            var sindicatosLaboraisSiglas = result.Select(r => r.SindLaboralSigla).Distinct();

            foreach (var siglaLaboral in sindicatosLaboraisSiglas)
            {
                if (string.IsNullOrEmpty(siglaLaboral)) continue;

                var datasNegociacoes = result.Where(r => r.SindLaboralSigla == siglaLaboral && r.SindLaboralDataNegociacao is not null).Select(r => r.SindLaboralDataNegociacao!).Distinct();
                var datasNegociacoesString = string.Join(", ", datasNegociacoes);

                if (string.IsNullOrEmpty(datasNegociacoesString)) continue;

                datasNegociacoesDictionary.Add(siglaLaboral, datasNegociacoesString); 
            }

            foreach (var r in result)
            {
                if (string.IsNullOrEmpty(r.SindLaboralSigla)) continue;
                r.SindLaboralDataNegociacao = datasNegociacoesDictionary[r.SindLaboralSigla];
            }

            var groupedResult = result.GroupBy(r => new { r.SindLaboralCnpj })
                                    .SelectMany(group => group.ToList())
                                    .ToList();

            return groupedResult;
        }
    }
}
