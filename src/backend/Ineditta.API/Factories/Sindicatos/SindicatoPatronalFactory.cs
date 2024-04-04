using Ineditta.API.ViewModels.SindicatosPatronais.ViewModels;
using Ineditta.Application.Usuarios.Entities;
using System.Text;

using Ineditta.BuildingBlocks.Core.Auth;
using Ineditta.BuildingBlocks.Core.Domain.ValueObjects;
using Ineditta.Repository.Contexts;

using Microsoft.EntityFrameworkCore;

using MySqlConnector;
using Ineditta.Repository.Extensions;
using Ineditta.BuildingBlocks.Core.Extensions;
using Ineditta.API.ViewModels.SindicatosPatronais.Requests;

namespace Ineditta.API.Factories.Sindicatos
{
    public class SindicatoPatronalFactory
    {
        private readonly InedittaDbContext _context;
        private readonly IUserInfoService _userInfoService;

        public SindicatoPatronalFactory(InedittaDbContext context, IUserInfoService userInfoService)
        {
            _context = context;
            _userInfoService = userInfoService;
        }

        public async ValueTask<IEnumerable<SindicatoPatronalViewModel>?> CriarPorUsuario(SindicatoPatronalRequest request)
        {
            var parameters = new List<MySqlParameter>();
            int parametersCount = 0;

            var stringBuilder = new StringBuilder(@"
                select spt.* from sind_patr spt 
                where exists(select 1 from cliente_unidades cut
                             inner join localizacao lt on cut.localizacao_id_localizacao = lt.id_localizacao
                             inner join usuario_adm uat on uat.email_usuario = @email
                             inner join cnae_emp cet ON (cet.cliente_unidades_id_unidade = cut.id_unidade AND (cet.data_final = '00-00-0000' OR cet.data_final IS NULL)) 
                             inner join base_territorialsindpatro btspt on cut.localizacao_id_localizacao = btspt.localizacao_id_localizacao1 AND cet.classe_cnae_idclasse_cnae = btspt.classe_cnae_idclasse_cnae
                             where case when uat.nivel = @nivel then true 
                                        when uat.nivel = @nivelGrupoEconomico then uat.id_grupoecon = cut.cliente_grupo_id_grupo_economico
                                        else JSON_CONTAINS(uat.ids_fmge, JSON_ARRAY(cut.id_unidade)) end
                             and spt.id_sindp = btspt.sind_patronal_id_sindp
            ");

            parameters.Add(new MySqlParameter("@email", _userInfoService.GetEmail()));
            parameters.Add(new MySqlParameter("@nivel", nameof(Nivel.Ineditta)));
            parameters.Add(new MySqlParameter("@nivelGrupoEconomico", Nivel.GrupoEconomico.GetDescription()));

            if (request.GrupoEconomicoId > 0)
            {
                stringBuilder.Append(" and cut.cliente_grupo_id_grupo_economico = @grupoEconomicoId");
                parameters.Add(new MySqlParameter("@grupoEconomicoId", request.GrupoEconomicoId));
            }

            if (request.GruposEconomicosIds != null && request.GruposEconomicosIds.Any())
            {
                QueryBuilder.AppendListToQueryBuilder(stringBuilder, request.GruposEconomicosIds, "cut.cliente_grupo_id_grupo_economico", parameters, ref parametersCount);
            }

            if (request.MatrizesIds != null && request.MatrizesIds.Any())
            {
                QueryBuilder.AppendListToQueryBuilder(stringBuilder, request.MatrizesIds, "cut.cliente_matriz_id_empresa", parameters, ref parametersCount);
            }

            if (request.ClientesUnidadesIds != null && request.ClientesUnidadesIds.Any())
            {
                QueryBuilder.AppendListToQueryBuilder(stringBuilder, request.ClientesUnidadesIds, "cut.id_unidade", parameters, ref parametersCount);
            }

            if (request.CnaesIds != null && request.CnaesIds.Any())
            {
                QueryBuilder.AppendListToQueryBuilder(stringBuilder, request.CnaesIds, "btspt.classe_cnae_idclasse_cnae", parameters, ref parametersCount);
            }

            if ((request.LocalizacoesIds != null && request.LocalizacoesIds.Any()) || (request.Ufs != null && request.Ufs.Any()))
            {
                if ((request.LocalizacoesIds != null && request.LocalizacoesIds.Any()) && (request.Ufs != null && request.Ufs.Any()))
                {
                    QueryBuilder.AppendListToQueryBuilder(stringBuilder, request.LocalizacoesIds, "lt.id_localizacao", request.Ufs, "lt.uf", parameters, ref parametersCount);
                }
                else if (request.LocalizacoesIds != null && request.LocalizacoesIds.Any())
                {
                    QueryBuilder.AppendListToQueryBuilder(stringBuilder, request.LocalizacoesIds, "lt.id_localizacao", parameters, ref parametersCount);
                }
                else if (request.Ufs != null && request.Ufs.Any())
                {
                    QueryBuilder.AppendListToQueryBuilder(stringBuilder, request.Ufs, "lt.uf", parameters, ref parametersCount);
                }
            }

            if (request.Regioes != null && request.Regioes.Any())
            {
                QueryBuilder.AppendListToQueryBuilder(stringBuilder, request.Regioes, "lt.regiao", parameters, ref parametersCount);
            }

            stringBuilder.Append(')');

            var result = await _context.SindPatrs
            .FromSqlRaw(stringBuilder.ToString(), parameters.ToArray())
            .AsNoTracking()
            .Select(sdt => new SindicatoPatronalViewModel
            {
                Id = sdt.Id,
                Cnpj = CNPJ.Formatar(sdt.Cnpj.Value),
                Sigla = sdt.Sigla,
                Municipio = sdt.Municipio,
                Uf = sdt.Uf,
                Telefone = sdt.Telefone1.Valor,
                Email = sdt.Email1 != null ? sdt.Email1.Valor : string.Empty,
                RazaoSocial = sdt.RazaoSocial
            }).ToListAsync();

            return result;
        }
    }
}
