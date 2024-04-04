using System.Text;

using Ineditta.Application.CalendarioSindicais.Eventos.Dtos;
using Ineditta.Application.CalendarioSindicais.Eventos.Repositories;
using Ineditta.Application.TiposEventosCalendarioSindical.Entities;
using Ineditta.Application.Usuarios.Entities;
using Ineditta.Repository.Contexts;

using Microsoft.EntityFrameworkCore;

using MySqlConnector;

namespace Ineditta.Repository.EventosCalendario
{
    public class EventoVencimentoMandatoLaboralRepository : IEventoVencimentoMandatoLaboralRepository
    {
        private readonly InedittaDbContext _context;
        private readonly int id_modulo_calendario_sindical = 5;

        public EventoVencimentoMandatoLaboralRepository(InedittaDbContext context)
        {
            _context = context;
        }

        public async ValueTask<IEnumerable<string>> ObterListaEmailNotificacao(int sindicatoLaboralId)
        {
            var parameters = new List<MySqlParameter>();

            var queryBuilder = new StringBuilder(@"
                    select uat.*
	                from usuario_adm uat 
                    where exists(select 1 
                                from cliente_matriz as cmt 
                                inner join sind_emp sinde on sinde.id_sinde = @sindicatoLaboralId
                                LEFT JOIN cliente_unidades as cut on cut.cliente_matriz_id_empresa = cmt.id_empresa 
                                LEFT join cnae_emp as cet ON (cet.cliente_unidades_id_unidade = cut.id_unidade AND cet.data_final = '00-00-0000') 
                                LEFT join base_territorialsindemp AS bl 
                                    on (bl.classe_cnae_idclasse_cnae = cet.classe_cnae_idclasse_cnae 
                                    AND bl.localizacao_id_localizacao1 = cut.localizacao_id_localizacao)
                                LEFT JOIN localizacao loc on bl.localizacao_id_localizacao1 = loc.id_localizacao
                                LEFT join base_territorialsindpatro AS bp 
                                    on (bp.classe_cnae_idclasse_cnae = cet.classe_cnae_idclasse_cnae 
                                    AND bp.localizacao_id_localizacao1 = cut.localizacao_id_localizacao)
                                    where 
                                    case WHEN uat.nivel = 'Ineditta' then true 
                                    when uat.nivel = 'Grupo Econômico' then uat.id_grupoecon = cut.cliente_grupo_id_grupo_economico
                                    else JSON_CONTAINS(uat.ids_fmge, JSON_ARRAY(cut.id_unidade)) end
                                    and  sinde.id_sinde = bl.sind_empregados_id_sinde1)
            ");

            parameters.Add(new MySqlParameter("@sindicatoLaboralId", sindicatoLaboralId));

            queryBuilder.Append(" GROUP BY uat.email_usuario");

            var emailQuery = _context.UsuarioAdms.FromSqlRaw(queryBuilder.ToString(), parameters.ToArray())
                .AsNoTracking();

            var listUsuarios = await emailQuery.ToListAsync();

            var tipoEventoId = TipoEventoCalendarioSindical.VencimentoMandatoSindicalLaboral.Id;

            List<string> listEmail = new();

            foreach (var usuario in listUsuarios)
            {
                var definicoesDoUsuarioParaEventoMandatoLaboral = await _context.UsuariosTiposEventosCalendarioSindical
                    .SingleOrDefaultAsync(utecs => utecs.TipoEventoId == tipoEventoId && utecs.UsuarioId == usuario.Id);

                var isPermitidoConsultarModuloCalendario = usuario.ModulosComerciais != null && usuario.ModulosComerciais.Any(mc => mc.Id == id_modulo_calendario_sindical && mc.Consultar);
                var isPermitidoReceberEmailsCalendarioNesseEvento = definicoesDoUsuarioParaEventoMandatoLaboral != null && definicoesDoUsuarioParaEventoMandatoLaboral.NotificarEmail;
                var isUsuarioIneditta = usuario.Nivel != Nivel.Ineditta;

                if (isPermitidoConsultarModuloCalendario && isPermitidoReceberEmailsCalendarioNesseEvento && !isUsuarioIneditta)
                {
                    listEmail.Add(usuario.Email.Valor);
                }
            }

            return listEmail;
        }

        public async ValueTask<IEnumerable<VencimentoMandatoLaboralViewModel>> ObterNotificacoesVencimentoMandatoLaboral()
        {
            var parameters = new Dictionary<string, object>();

            var notificacoesVencimentoMandatoPatronal = new StringBuilder(@"
                SELECT
                    cst.id Id,
                    cst.data_referencia DataVencimento,
                    sp.id_sinde SindicatoLaboralId,
                    sp.razaosocial_sinde SindicatoLaboral,
                    sp.uf_sinde Uf,
                    sp.sigla_sinde SiglaSindicato,
                    d.dirigentes DirigentesJson,
                    a.abrangencia AbrangenciaJson
                FROM calendario_sindical_tb cst
                LEFT JOIN sind_emp sp ON sp.id_sinde = cst.chave_referencia
                LEFT JOIN LATERAL (
	                SELECT JSON_ARRAYAGG(JSON_OBJECT('Id',sd.id_diretoriae, 'Nome', sd.dirigente_e, 'Funcao', sd.funcao_e)) dirigentes
	                FROM sind_diremp sd WHERE sd.sind_emp_id_sinde = sp.id_sinde
                ) d ON true
                LEFT JOIN LATERAL (
	                SELECT JSON_ARRAYAGG(
	                JSON_OBJECT(
		                'LocalizacaoId', l.id_localizacao,
		                'Municipio', l.municipio,
		                'Uf', l.uf
		                ) 
	                ) as abrangencia FROM
	                (
                        SELECT DISTINCT loc.*
                        FROM base_territorialsindemp bt
                        JOIN localizacao loc ON loc.id_localizacao = bt.localizacao_id_localizacao1
                        WHERE bt.sind_empregados_id_sinde1 = sp.id_sinde 
                    ) l
                ) a ON true
                WHERE cst.tipo_evento = @tipoEvento
                      AND cst.data_referencia >= CURDATE()
                      AND cst.data_referencia <= DATE_ADD(CURDATE(), INTERVAL TIME_TO_SEC(cst.notificar_antes) SECOND)
                      AND cst.id NOT IN (
                            SELECT evento_id FROM calendario_sindical_notificacao_tb
                            GROUP BY evento_id
                      )
            ");

            parameters.Add("@tipoEvento", TipoEventoCalendarioSindical.VencimentoMandatoSindicalLaboral.Id);

            var query = _context.SelectFromRawSqlAsync<VencimentoMandatoLaboralViewModel>(notificacoesVencimentoMandatoPatronal.ToString(), parameters);

            var result = await query;

            return result;
        }
    }
}
