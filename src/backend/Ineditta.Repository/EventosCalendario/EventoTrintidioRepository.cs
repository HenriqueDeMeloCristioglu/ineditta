using System.Text;

using Ineditta.Application.CalendarioSindicais.Eventos.Dtos;
using Ineditta.Application.CalendarioSindicais.Eventos.Repositories;
using Ineditta.Application.TiposEventosCalendarioSindical.Entities;
using Ineditta.Application.Usuarios.Entities;
using Ineditta.Application.UsuariosTiposEventosCalendarioSindical.Entities;
using Ineditta.Repository.Contexts;

using Microsoft.EntityFrameworkCore;

using MySqlConnector;

namespace Ineditta.Repository.EventosCalendario
{
    public class EventoTrintidioRepository : IEventoTrintidioRepository
    {
        private readonly InedittaDbContext _context;
        private readonly int id_modulo_calendario_sindical = 5;

        public EventoTrintidioRepository(InedittaDbContext context)
        {
            _context = context;
        }

        public async ValueTask<IEnumerable<EventoTrintidioViewModel>?> ObterEventosTrintidioAsync()
        {
            var parameters = new Dictionary<string, object>();

            var notificacoesTrintidio = new StringBuilder(@"
                SELECT
                    ev.id Id,
                    ds.id_doc DocId,
                    td.nome_doc NomeDocumento,
                    td.sigla_doc SiglaDocumento,
                    ds.cnae_doc AtividadesEconomicas,
                    ds.sind_laboral SindicatoLaboral,
                    ds.sind_patronal SindicatoPatronal,
                    ds.abrangencia Abrangencia,
                    (ds.validade_inicial - interval 60 day) AS VigenciaInicial,
                    (ds.validade_inicial - interval 29 day) AS VigenciaFinal
                FROM
	                calendario_sindical_tb ev
                LEFT JOIN doc_sind ds ON ev.chave_referencia = ds.id_doc
                LEFT JOIN tipo_doc td ON ds.tipo_doc_idtipo_doc = td.idtipo_doc
                WHERE ev.tipo_evento = @tipoEvento
                      AND ev.data_referencia >= CURDATE()
                      AND ev.data_referencia <= DATE_ADD(CURDATE(), INTERVAL TIME_TO_SEC(ev.notificar_antes) SECOND)
                      AND ev.id NOT IN (
                            SELECT evento_id FROM calendario_sindical_notificacao_tb
                            GROUP BY evento_id
                      )
            ");

            parameters.Add("@tipoEvento", TipoEventoCalendarioSindical.Trintidio.Id);

            var query = _context.SelectFromRawSqlAsync<EventoTrintidioViewModel>(notificacoesTrintidio.ToString(), parameters);

            var result = await query;

            return result;
        }

        public async ValueTask<IEnumerable<string>> ObterListaEmailPorDocumentoId(int documentoId)
        {
            var parameters = new List<MySqlParameter>();

            var queryBuilder = new StringBuilder(@"
                SELECT * FROM usuario_adm ua 
                WHERE EXISTS (
	                SELECT 1 FROM doc_sind ds 
	                WHERE ds.id_doc = @documentoId
	                      AND CASE WHEN ua.nivel = 'Grupo Econômico' THEN JSON_CONTAINS(ds.cliente_estabelecimento, CONCAT('{{""g"": ', ua.id_grupoecon,'}}'))
	                               ELSE EXISTS (
      		                            SELECT 1 FROM cliente_unidades cut
      		               	            WHERE EXISTS (
      		               		            SELECT 
								                jt.*
								            FROM 
								                JSON_TABLE(
								                    ds.cliente_estabelecimento,
								                    ""$[*]"" COLUMNS (
								                        g INT PATH ""$.g"",
								                        m INT PATH ""$.m"",
								                        u INT PATH ""$.u"",
								                        nome_unidade VARCHAR(255) PATH ""$.nome_unidade""
								                    )
								                ) AS jt
								             WHERE jt.u = cut.id_unidade AND JSON_CONTAINS(ua.ids_fmge, JSON_ARRAY(cut.id_unidade))
      		               	            )	
                                  )
	                          END
                )
            ");

            parameters.Add(new MySqlParameter("@documentoId", documentoId));

            var emailQuery = _context.UsuarioAdms.FromSqlRaw(queryBuilder.ToString(), parameters.ToArray())
                .AsNoTracking();

            var listUsuarios = await emailQuery.ToListAsync();

            var tipoEventoId = TipoEventoCalendarioSindical.Trintidio.Id;

            List<string> listEmail = new(); 

            foreach (var usuario in listUsuarios)
            {
                var definicoesDoUsuarioParaEventoTrintidio = await _context.UsuariosTiposEventosCalendarioSindical
                    .SingleOrDefaultAsync(utecs => utecs.TipoEventoId == tipoEventoId && utecs.UsuarioId == usuario.Id);

                var isPermitidoConsultarModuloCalendario = usuario.ModulosComerciais != null && usuario.ModulosComerciais.Any(mc => mc.Id == id_modulo_calendario_sindical && mc.Consultar);
                var isPermitidoReceberEmailsCalendarioNesseEvento = definicoesDoUsuarioParaEventoTrintidio != null && definicoesDoUsuarioParaEventoTrintidio.NotificarEmail;
                var isUsuarioIneditta = usuario.Nivel != Nivel.Ineditta;

                if (isPermitidoConsultarModuloCalendario && isPermitidoReceberEmailsCalendarioNesseEvento && !isUsuarioIneditta)
                {
                    listEmail.Add(usuario.Email.Valor);
                }
            }

            return listEmail;
        }
    }
}
