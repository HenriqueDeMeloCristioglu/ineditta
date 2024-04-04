using System.Text;

using Ineditta.Application.CalendarioSindicais.Eventos.Dtos;
using Ineditta.Application.CalendarioSindicais.Eventos.Entities;
using Ineditta.Application.CalendarioSindicais.Eventos.Repositories;
using Ineditta.Application.CalendarioSindicais.Usuarios.Entities;
using Ineditta.Application.TiposEventosCalendarioSindical.Entities;
using Ineditta.Application.Usuarios.Entities;
using Ineditta.Application.UsuariosTiposEventosCalendarioSindical.Entities;
using Ineditta.Repository.Contexts;
using Ineditta.Repository.Extensions;
using Ineditta.Repository.Models;

using Microsoft.EntityFrameworkCore;

using MySqlConnector;

using Polly;

namespace Ineditta.Repository.EventosCalendario
{
    public class EventoRepository : IEventoRepository
    {
        private readonly InedittaDbContext _context;
        private readonly int id_modulo_calendario_sindical = 5;

        public EventoRepository(InedittaDbContext context)
        {
            _context = context;
        }

        public async ValueTask AtualizarAsync(CalendarioSindical evento)
        {
            _context.Entry(evento).Property("DataInclusao").CurrentValue = DateTime.Now;

            if (evento.Id <= 0) return;

            _context.Eventos.Update(evento);

            await Task.CompletedTask;
        }

        public async ValueTask IncluirAsync(CalendarioSindical evento)
        {
            _context.Entry(evento).Property("DataInclusao").CurrentValue = DateTime.Now;

            _context.Eventos.Add(evento);

            await Task.CompletedTask;
        }

        public async ValueTask<CalendarioSindical?> ObterPorIdAsync(long id)
        {
            return await _context.Eventos.FindAsync(id);
        }

        public async ValueTask<IEnumerable<EventoClausulaViewModel>?> ObterNotificacoesEventosClausulas()
        {
            var parameters = new Dictionary<string, object>();

            var notificacoesVencimentoMandatoPatronal = new StringBuilder(@"
                SELECT
	                cst.id Id,
	                ds.id_doc DocId,
	                cst.data_referencia DataVencimento,
	                cst.chave_referencia ClausulaGeralEstruturaClausulaId,
	                at2.nmtipoinformacaoadicional NomeCampo,
	                ec.nome_clausula NomeInfoAdicional,
	                td.nome_doc NomeDocumento,
	                ds.abrangencia Abrangencia,
	                ds.cnae_doc AtividadesEconomicas,
	                ds.sind_laboral SindicatoLaboral,
	                ds.sind_patronal SindicatoPatronal,
                    td.sigla_doc SiglaDocumento,
                    cst.tipo_evento TipoEventoId,
                    cst.subtipo_evento SubtipoEventoId
                FROM calendario_sindical_tb cst
                INNER JOIN clausula_geral_estrutura_clausula cgec ON cgec.id_clausulageral_estrutura_clausula = cst.chave_referencia
                LEFT JOIN estrutura_clausula ec ON ec.id_estruturaclausula = cgec.nome_informacao
                LEFT JOIN ad_tipoinformacaoadicional at2 ON at2.cdtipoinformacaoadicional = cgec.ad_tipoinformacaoadicional_cdtipoinformacaoadicional 
                LEFT JOIN doc_sind ds ON ds.id_doc = cgec.doc_sind_id_doc
                LEFT JOIN tipo_doc td ON td.idtipo_doc = ds.tipo_doc_idtipo_doc 
                WHERE cst.tipo_evento = @tipoEvento
                      AND cst.data_referencia >= CURDATE()
                      AND cst.data_referencia <= DATE_ADD(CURDATE(), INTERVAL TIME_TO_SEC(cst.notificar_antes) SECOND)
                      AND cst.id NOT IN (
                            SELECT evento_id FROM calendario_sindical_notificacao_tb
                            GROUP BY evento_id
                      )
            ");

            parameters.Add("@tipoEvento", TipoEventoCalendarioSindical.EventoClausula.Id);

            var query = _context.SelectFromRawSqlAsync<EventoClausulaViewModel>(notificacoesVencimentoMandatoPatronal.ToString(), parameters);

            var result = await query;

            return result;
        }

        public async ValueTask<IEnumerable<string>?> ObterListaEmailEventoPorDocumentoId(int documentoId, int tipoEventoId, int? subtipoEventoId = null)
        {
            var parameters = new List<MySqlParameter>();

            var queryBuilder = new StringBuilder(@"
                SELECT ua.* FROM usuario_adm ua
                LEFT JOIN doc_sind ds ON true
                LEFT JOIN cliente_unidades cut ON JSON_CONTAINS(ds.cliente_estabelecimento, JSON_ARRAY(cut.id_unidade)) 
                WHERE CASE WHEN ua.nivel = 'Grupo Econômico' THEN JSON_CONTAINS(ds.cliente_estabelecimento, CONCAT('{{""g"" :', ua.id_grupoecon,'}}'))
                           ELSE JSON_CONTAINS(ds.cliente_estabelecimento, CONCAT('{{""u"": ', cut.id_unidade,'}}'))
                      END
                      AND ds.id_doc = @documentoId
            ");

            parameters.Add(new MySqlParameter("@documentoId", documentoId));

            queryBuilder.Append(" GROUP BY ua.email_usuario");

            var emailQuery = _context.UsuarioAdms.FromSqlRaw(queryBuilder.ToString(), parameters.ToArray())
                .AsNoTracking();

            var listUsuarios = await emailQuery.ToListAsync();

            UsuarioTipoEventoCalendarioSindical? configUsuarioEvento = null;

            if (subtipoEventoId is null)
            {
                configUsuarioEvento = await _context.UsuariosTiposEventosCalendarioSindical.Where(utecs => utecs.TipoEventoId == tipoEventoId).SingleOrDefaultAsync();
            }
            else
            {
                configUsuarioEvento = await _context.UsuariosTiposEventosCalendarioSindical.Where(utecs => utecs.TipoEventoId == tipoEventoId && utecs.SubtipoEventoId == subtipoEventoId).SingleOrDefaultAsync();
            }

            var listEmail = listUsuarios.Where(ua => (configUsuarioEvento != null && configUsuarioEvento.NotificarEmail) && ua.Nivel != Nivel.Ineditta && ua.ModulosComerciais != null && ua.ModulosComerciais.Any(mc => mc.Id == id_modulo_calendario_sindical && mc.Consultar)).Select(e => e.Email.Valor);

            return listEmail is not null ? listEmail : (IEnumerable<string>?)null;
        }

        public async ValueTask<IEnumerable<AgendaEventosViewModel>?> ObterNotificacoesAgendaEventos()
        {
            var parameters = new Dictionary<string, object>();

            var agendasEvento = new StringBuilder(@"
                SELECT
                    cst.id Id,
                    csut.id CalendarioSindicalUsuarioId,
                    csut.comentarios Comentarios,
                    cst.data_referencia DataVencimento,
                    csut.local Local,
                    csut.titulo Titulo,
                    csut.id_usuario UsuarioCriadorId,
                    csut.visivel Visivel,
                    csut.notificar_antes NotificarAntes,
                    csut.recorrencia Recorrencia,
                    csut.validade_recorrencia ValidadeRecorrencia,
                    uad.email_usuario UsuarioCriadorEmail,
                    utecs.notificar_email UsuarioNotificarEmail
                FROM calendario_sindical_tb cst 
                JOIN calendario_sindical_usuario_tb csut ON cst.chave_referencia = csut.id
                LEFT JOIN usuario_adm uad ON uad.id_user = csut.id_usuario
                LEFT JOIN usuario_tipo_evento_calendario_sindical utecs ON utecs.usuario_id = uad.id_user AND utecs.tipo_evento_id = @tipoEvento
                WHERE cst.tipo_evento = @tipoEvento
                        AND cst.data_referencia >= CURRENT_TIMESTAMP
                        AND cst.data_referencia <= DATE_ADD(CURRENT_TIMESTAMP, INTERVAL TIME_TO_SEC(cst.notificar_antes) SECOND)
                        AND cst.id NOT IN (
                            SELECT evento_id FROM calendario_sindical_notificacao_tb
                            GROUP BY evento_id
                        )
            ");

            parameters.Add("@tipoEvento", TipoEventoCalendarioSindical.AgendaEventos.Id);

            var query = _context.SelectFromRawSqlAsync<AgendaEventosViewModel>(agendasEvento.ToString(), parameters);

            var result = await query;

            return result;
        }

        public async ValueTask<IEnumerable<string>?> ObterListaEmailAssembleiaPatronalReuniaoAsync(AssembleiaReuniaoViewModelBase assembleiaPatronalReuniao, int tipoEventoId)
        {
            var parameters = new List<MySqlParameter>();
            var parametersCount = 0;

            var queryBuilder = new StringBuilder(@"
                select uat.* from usuario_adm uat
                where exists(
			                 select 1 from cliente_unidades cut
					                 inner join cnae_emp cet on cut.id_unidade = cet.cliente_unidades_id_unidade 
					                 and true     
            "
            );

            if (assembleiaPatronalReuniao.SindicatosLaboraisIds is not null && assembleiaPatronalReuniao.SindicatosLaboraisIds.Any())
            {
                queryBuilder.Append(@" AND exists(select 1 
					 				                from base_territorialsindemp bemp
					 				                where bemp.localizacao_id_localizacao1 = cut.localizacao_id_localizacao
			 				   		                and bemp.classe_cnae_idclasse_cnae = cet.classe_cnae_idclasse_cnae 
                                                    ");

                QueryBuilder.AppendListToQueryBuilder(queryBuilder, assembleiaPatronalReuniao.SindicatosLaboraisIds, "bemp.sind_empregados_id_sinde1", parameters, ref parametersCount);

                queryBuilder.Append(@") ");
            }
            
            if (assembleiaPatronalReuniao.SindicatosPatronaisIds is not null && assembleiaPatronalReuniao.SindicatosPatronaisIds.Any())
            {
                queryBuilder.Append(@" AND exists(select 1 
					 							from base_territorialsindpatro bpatr
					 							where bpatr.localizacao_id_localizacao1 = cut.localizacao_id_localizacao
					 							and bpatr.classe_cnae_idclasse_cnae = cet.classe_cnae_idclasse_cnae 
					 							");
                QueryBuilder.AppendListToQueryBuilder(queryBuilder, assembleiaPatronalReuniao.SindicatosPatronaisIds, "bpatr.sind_patronal_id_sindp", parameters, ref parametersCount);

                queryBuilder.Append(@") ");
            }

            if (assembleiaPatronalReuniao.AtividadesEconomicasIds is not null && assembleiaPatronalReuniao.AtividadesEconomicasIds.Any())
            {
                QueryBuilder.AppendListToQueryBuilder(queryBuilder, assembleiaPatronalReuniao.AtividadesEconomicasIds, "cet.classe_cnae_idclasse_cnae", parameters, ref parametersCount);
            }

            queryBuilder.Append(@" and case when uat.nivel = 'Ineditta' then false 
                                        when uat.nivel = 'Grupo Econômico' then uat.id_grupoecon = cut.cliente_grupo_id_grupo_economico
                                        else JSON_CONTAINS(uat.ids_fmge, JSON_ARRAY(cut.id_unidade)) end)");

            var emailQuery = _context.UsuarioAdms.FromSqlRaw(queryBuilder.ToString(), parameters.ToArray())
                .AsNoTracking();

            var listUsuarios = await emailQuery.ToListAsync();

            var vencimentoDocumentoEventoId = tipoEventoId;
            var configUsuarioEvento = await _context.UsuariosTiposEventosCalendarioSindical.Where(utecs => utecs.TipoEventoId == vencimentoDocumentoEventoId).SingleOrDefaultAsync();

            var listEmail = listUsuarios.Where(ua => (configUsuarioEvento != null && configUsuarioEvento.NotificarEmail) && ua.Nivel != Nivel.Ineditta && ua.ModulosComerciais != null && ua.ModulosComerciais.Any(mc => mc.Id == id_modulo_calendario_sindical && mc.Consultar)).Select(e => e.Email.Valor);

            return listEmail is not null ? listEmail : (IEnumerable<string>?)null;
        }

        public async ValueTask<IEnumerable<AssembleiaReuniaoViewModelBase>?> ObterAssembleiaPatronalReunioesSindicaisAsync(int tipoEvento)
        {
            if (!(tipoEvento == TipoEventoCalendarioSindical.AssembleiaPatronal.Id || tipoEvento == TipoEventoCalendarioSindical.ReuniaoEntrePartes.Id)) throw new ArgumentException("Tipo do evento inválido");
            
            var parameters = new Dictionary<string, object>();

            var assembleiasPatronais = new StringBuilder(@"
                SELECT 
                    cst.id AS Id,
                    cst.chave_referencia AS AcompanhamentoCctId,
                    ac.abrangencia,
                    td.nome_doc AS NomeDocumento,
                    cnaes.classes AS AtividadesEconomicas,
                    cst.data_referencia AS DataHora,
                    ac.data_base AS `DataBase`,
                    ac.fase AS FaseNegociacao,
                    jt.*,
                    sind_laborais.siglas SindicatosLaborais,
                    sind_laborais.ids SindicatosLaboraisIdsString,
                    sind_patronais.siglas SindicatosPatronais,
                    sind_patronais.ids SindicatosPatronaisIdsString,
                    cnaes.cnaesIds AtividadesEconomicasIdsString,
                    ac.observacoes_gerais Observacoes
                FROM calendario_sindical_tb cst
                JOIN acompanhanto_cct ac ON ac.idacompanhanto_cct = cst.chave_referencia
                LEFT JOIN tipo_doc td ON td.idtipo_doc = ac.tipo_doc_idtipo_doc
                LEFT JOIN LATERAL (
                    SELECT 
                        GROUP_CONCAT(cc.descricao_subclasse SEPARATOR ';') AS classes,
                        JSON_ARRAYAGG(cc.id_cnae) AS cnaesIds
                    FROM 
                        classe_cnae cc
                    WHERE 
                        JSON_CONTAINS(ac.ids_cnaes, JSON_ARRAY(CAST(cc.id_cnae AS char)))
                ) cnaes ON true
                LEFT JOIN LATERAL (
                    SELECT 
                        jt.*
                    FROM 
                        JSON_TABLE(
                            ac.scripts_salvos,
                            '$[*]' COLUMNS(
                                HorarioResposta datetime PATH '$.Horario',
                                RespostasScriptString json PATH '$.Respostas'
                            )
                        ) AS jt
                    order by jt.HorarioResposta desc
                    LIMIT 1
                ) jt ON true
                LEFT JOIN LATERAL (
                    SELECT GROUP_CONCAT(sigla_sinde SEPARATOR ',') siglas, JSON_ARRAYAGG(se.id_sinde) AS ids FROM sind_emp se
                    WHERE se.id_sinde = ac.sind_emp_id_sinde 
                    OR JSON_CONTAINS(ac.ids_sindemp_adicionais, JSON_ARRAY(CAST(se.id_sinde AS char))) 
                ) sind_laborais ON true
                LEFT JOIN LATERAL (
                    SELECT GROUP_CONCAT(sigla_sp SEPARATOR ',') siglas, JSON_ARRAYAGG(sp.id_sindp) AS ids FROM sind_patr sp
                    WHERE sp.id_sindp = ac.sind_patr_id_sindp 
                    OR JSON_CONTAINS(ac.ids_sindpatr_adicionais , JSON_ARRAY(CAST(sp.id_sindp AS char))) 
                ) sind_patronais ON true
                WHERE 
                    cst.tipo_evento = @tipoEvento
                    AND cst.data_referencia >= CURRENT_TIMESTAMP
                    AND cst.data_referencia <= DATE_ADD(CURRENT_TIMESTAMP, INTERVAL TIME_TO_SEC(cst.notificar_antes) SECOND)
                    AND cst.id NOT IN (
                        SELECT evento_id 
                        FROM calendario_sindical_notificacao_tb
                        GROUP BY evento_id
                    );
            ");

            parameters.Add("@tipoEvento", tipoEvento);

            var query = _context.SelectFromRawSqlAsync<AssembleiaReuniaoViewModelBase>(assembleiasPatronais.ToString(), parameters);

            var result = await query;

            return result;
        }
    }
}
