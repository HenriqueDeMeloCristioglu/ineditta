using System.Globalization;

using CSharpFunctionalExtensions;

using Humanizer.Localisation.DateToOrdinalWords;

using Ineditta.Application.CalendarioSindicais.Eventos.Dtos;
using Ineditta.Application.CalendarioSindicais.Eventos.Entities;
using Ineditta.Application.CalendarioSindicais.Eventos.Services;
using Ineditta.Application.CalendarioSindicais.Usuarios.Dtos;
using Ineditta.Application.CalendarioSindicais.Usuarios.Entities;
using Ineditta.Application.CalendarioSindicais.Usuarios.Factories;
using Ineditta.Application.InformacoesAdicionais.Sisap.Entities;
using Ineditta.Application.TiposEventosCalendarioSindical.Entities;
using Ineditta.BuildingBlocks.Core.Domain.Models;
using Ineditta.Repository.Contexts;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using MySqlConnector;

namespace Ineditta.Repository.CalendariosSindicais
{
    public class CalendarioSindicalGeradorService : ICalendarioSindicalGeradorService
    {
        private readonly InedittaDbContext _context;
        private readonly ILogger<ICalendarioSindicalGeradorService> _logger;

        public CalendarioSindicalGeradorService(InedittaDbContext context, ILogger<ICalendarioSindicalGeradorService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async ValueTask<Result<bool, Error>> GerarAsync(CancellationToken cancellationToken)
        {
            try
            {
                await IncluirVencimentosPorDocumentoAsync(cancellationToken);
                await IncluirMandatosSindicaisLaborais(cancellationToken);
                await IncluirMandatosSindicaisPatronais(cancellationToken);
                await IncluirEventosTrintidio(cancellationToken);
                await IncluirEventosClausulaNaoPeriodicos(cancellationToken);
                await IncluirEventosClausulasPeriodicos(cancellationToken);
                await IncluirEventosAgenda(cancellationToken);

                _logger.LogInformation("Eventos do Calendario Sindical gerados e/ou atualizados");
                return Result.Success<bool, Error>(true);
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Ocorreu um erro no banco de dados ao incluir vencimentos");
                return Result.Failure<bool, Error>(Errors.General.Business("Ocorreu uma exceção ao processar gerador do calendario sindical"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocorreu um erro ao incluir vencimentos");
                return Result.Failure<bool, Error>(Errors.General.Business("Ocorreu uma exceção ao processar gerador do calendario sindical"));
            }
        }

        public async ValueTask<Result<bool, Error>> GerarAgendaEventosAsync(CancellationToken cancellationToken)
        {
            try
            {
                await IncluirEventosAgenda(cancellationToken);

                _logger.LogInformation("Eventos do Calendario Sindical gerados e/ou atualizados");
                return Result.Success<bool, Error>(true);
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Ocorreu um erro no banco de dados ao incluir vencimentos");
                return Result.Failure<bool, Error>(Errors.General.Business("Ocorreu uma exceção ao processar gerador do calendario sindical"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocorreu um erro ao incluir vencimentos");
                return Result.Failure<bool, Error>(Errors.General.Business("Ocorreu uma exceção ao processar gerador do calendario sindical"));
            }
        }

        private async ValueTask IncluirVencimentosPorDocumentoAsync(CancellationToken cancellationToken)
        {
            try
            {
                await _context.Database.ExecuteSqlRawAsync(@"
                    INSERT INTO calendario_sindical_tb (chave_referencia, tipo_evento, origem, notificar_antes, data_referencia)
                    SELECT 
                        ds.id_doc chave_referencia,
                        1 tipo_evento,
                        1 origem,
                        '120:00:00' notificar_antes,
                        ds.validade_final data_referencia
                    FROM doc_sind ds
                    WHERE ds.modulo = 'SISAP'
                          and ds.validade_final <> '0000-00-00' and ds.validade_final is not null and ds.validade_final >= CURRENT_DATE()
                          and ds.data_aprovacao is not null
                          and NOT EXISTS (
                              SELECT 1 FROM calendario_sindical_tb cs
                              WHERE cs.chave_referencia = ds.id_doc
                                    and cs.tipo_evento = 1
                                    and cs.origem = 1
                          )
                ", cancellationToken);
            }
            catch {
                _logger.LogInformation("Ocorreu um erro ao incluir os eventos de vencimento de documentos");
            }    
        }

        private async ValueTask IncluirMandatosSindicaisLaborais(CancellationToken cancellationToken)
        {
            try
            {
                await _context.Database.ExecuteSqlRawAsync(@"
                    INSERT INTO calendario_sindical_tb (chave_referencia, tipo_evento, origem, notificar_antes, data_referencia)
                    SELECT 
                        sd.sind_emp_id_sinde chave_referencia,
                        2 tipo_evento,
                        1 origem,
                        '120:00:00' notificar_antes,
                        sd.termino_mandatoe data_referencia
                    FROM sind_diremp sd
                    WHERE sd.termino_mandatoe <> '0000-00-00' and sd.termino_mandatoe is not null
                          and NOT EXISTS (
                              SELECT 1 FROM calendario_sindical_tb cs
                              WHERE cs.chave_referencia = sd.sind_emp_id_sinde
                                    and cs.tipo_evento = 2
                                    and cs.origem = 1
                                    and cs.data_referencia = sd.termino_mandatoe
                              )
                    GROUP BY sd.termino_mandatoe
                ", cancellationToken);
            }
            catch
            {
                _logger.LogInformation("Ocorreu um erro ao incluir os eventos de vencimento de mandatos sindicais laborais");
            }
        }

        private async ValueTask IncluirMandatosSindicaisPatronais(CancellationToken cancellationToken)
        {
            try
            {
                await _context.Database.ExecuteSqlRawAsync(@"
                    INSERT INTO calendario_sindical_tb (chave_referencia, tipo_evento, origem, notificar_antes, data_referencia)
                    SELECT 
                        sd.sind_patr_id_sindp chave_referencia,
                        3 tipo_evento,
                        1 origem,
                        '120:00:00' notificar_antes,
                        sd.termino_mandatop data_referencia
                    FROM sind_dirpatro sd
                    WHERE sd.termino_mandatop <> '0000-00-00' and sd.termino_mandatop is not null
                          and NOT EXISTS (
                              SELECT 1 FROM calendario_sindical_tb cs
                              WHERE cs.chave_referencia = sd.sind_patr_id_sindp
                                    and cs.tipo_evento = 3
                                    and cs.origem = 1
                                    and cs.data_referencia = sd.termino_mandatop
                              )
                    GROUP BY sd.termino_mandatop
                ", cancellationToken);
            }
            catch
            {
                _logger.LogInformation("Ocorreu um erro ao incluir os eventos de vencimento de mandatos sindicais patronais");
            }
            
        }

        private async ValueTask IncluirEventosTrintidio(CancellationToken cancellationToken)
        {
            const int tipoDocumentoAcordoColetivoId = 5;
            const int tipoDocumentoConvencaoColetivaId = 6;
            const int tipoDocumentoTermoAditivoAcordoColetivoId = 8;
            const int tipoDocumentoTermoAditivoConvencaoColetivaId = 9;
            const string estruturaClausulaVigenciaEDataBaseId = "197";

            try
            {
                await _context.Database.ExecuteSqlInterpolatedAsync(@$"
                    INSERT INTO calendario_sindical_tb (chave_referencia, tipo_evento, origem, notificar_antes, data_referencia)
                    SELECT 
                        ds.id_doc chave_referencia,
                        {TipoEvento.Trintidio} tipo_evento,
                        {OrigemEvento.Ineditta} origem,
                        '120:00:00' notificar_antes,
                        (ds.validade_inicial - interval 60 day) AS validade_inicial
                    from
                        doc_sind ds
                    join tipo_doc td on
                        ((td.idtipo_doc = ds.tipo_doc_idtipo_doc))
                    where
                        (
                          (ds.tipo_doc_idtipo_doc in ({tipoDocumentoAcordoColetivoId}, {tipoDocumentoConvencaoColetivaId}))
                          or (
                             (ds.tipo_doc_idtipo_doc in ({tipoDocumentoTermoAditivoAcordoColetivoId}, {tipoDocumentoTermoAditivoConvencaoColetivaId}))
                             and json_contains(ds.referencia, json_array({estruturaClausulaVigenciaEDataBaseId}))
                          )
                        )
                        and (ds.modulo = 'SISAP')
                        and ds.data_aprovacao is not null
                        and NOT EXISTS (
                            SELECT 1 FROM calendario_sindical_tb cs
                            WHERE cs.chave_referencia = ds.id_doc
                                and cs.tipo_evento = {TipoEvento.Trintidio}
                                and cs.origem = {OrigemEvento.Ineditta}
                        )
                ", cancellationToken);
            }
            catch
            {
                _logger.LogInformation("Ocorreu um erro durante a geração dos eventos tríntido.");
            }
        }

        private async ValueTask IncluirEventosClausulaNaoPeriodicos(CancellationToken cancellationToken)
        {
            try
            {
                await _context.Database.ExecuteSqlRawAsync(@$"
                    INSERT INTO calendario_sindical_tb (chave_referencia, tipo_evento, origem, notificar_antes, data_referencia, subtipo_evento)
                    SELECT
	                    cgec.id_clausulageral_estrutura_clausula id,
	                    cgec2.id_clausulageral_estrutura_clausula id2,
                        cgec2.id_clausulageral_estrutura_clausula chave_referencia,
                        cgec.combo adfasd,
                        {TipoEvento.EventoClausula} tipo_evento,
                        {OrigemEvento.Ineditta} origem,
                        '120:00:00' notificar_antes,
                        cgec2.data data_referencia,
                        subtipo.id subtipo_evento,
                        gc.nome_grupo gc,
                        at2.nmtipoinformacaoadicional
                    FROM clausula_geral_estrutura_clausula cgec
                    INNER JOIN clausula_geral_estrutura_clausula cgec2 ON 
                        cgec2.clausula_geral_id_clau = cgec.clausula_geral_id_clau 
                        and cgec2.grupo_dados = cgec.grupo_dados 
                        and cgec2.ad_tipoinformacaoadicional_cdtipoinformacaoadicional IN (
                            {TipoInformacaoAdicional.DataPagamento},
                            {TipoInformacaoAdicional.DataDesconto},
                            {TipoInformacaoAdicional.DataObrigacao},
                            {TipoInformacaoAdicional.DataRecolhimento}
                        )
                    LEFT JOIN doc_sind ds ON ds.id_doc = cgec.doc_sind_id_doc
                    LEFT JOIN tipo_doc td ON td.idtipo_doc = ds.tipo_doc_idtipo_doc
                    LEFT JOIN ad_tipoinformacaoadicional at2 ON at2.cdtipoinformacaoadicional = cgec2.ad_tipoinformacaoadicional_cdtipoinformacaoadicional
                    LEFT JOIN estrutura_clausula ec2 ON ec2.id_estruturaclausula = cgec.nome_informacao
                    LEFT JOIN grupo_clausula gc ON gc.idgrupo_clausula = ec2.grupo_clausula_idgrupo_clausula
                    LEFT JOIN LATERAL (
	                    SELECT id FROM subtipo_evento_calendario_sindical 
                        WHERE nome = CONCAT(gc.nome_grupo, ' - ', at2.nmtipoinformacaoadicional)
                    ) subtipo ON TRUE
                    WHERE cgec.estrutura_clausula_id_estruturaclausula IN (
                          SELECT ec.id_estruturaclausula FROM estrutura_clausula ec
                          WHERE ec.calendario = 'S'
                    )
                          AND cgec.ad_tipoinformacaoadicional_cdtipoinformacaoadicional IN ({TipoInformacaoAdicional.CalendarioSindicalRecorrencia})
                          AND CASE WHEN cgec.ad_tipoinformacaoadicional_cdtipoinformacaoadicional = {TipoInformacaoAdicional.CalendarioSindicalRecorrencia} THEN cgec.combo = 'Datas específicas' ELSE true END
                          AND cgec2.data is not null
                          AND ds.modulo = 'SISAP'
                          AND ds.data_aprovacao is not null
                          AND cgec.combo = 'Datas específicas'
                          AND NOT EXISTS (
                              SELECT 1 FROM calendario_sindical_tb cs
                              WHERE cs.chave_referencia = cgec2.id_clausulageral_estrutura_clausula
                                    and cs.tipo_evento = {TipoEvento.EventoClausula}
                                    and cs.origem = {OrigemEvento.Ineditta}
                          )
                ", cancellationToken);
            }
            catch
            {
                _logger.LogInformation("Ocorreu um erro durante a geração dos eventos não periodicos de eventos de clausulas");
            }
        }

        private async ValueTask IncluirEventosClausulasPeriodicos(CancellationToken cancellationToken)
        {
            var eventosPeriodicosBase = await _context.SelectFromRawSqlAsync<EventoClausulaBaseDto>(@"
                SELECT
	                cgec2.id_clausulageral_estrutura_clausula ChaveReferencia,
	                STR_TO_DATE(cgec2.`data`, '%Y-%m-%d') DataReferencia,
                    STR_TO_DATE(COALESCE(cgec3.`data`, ds.validade_final), '%Y-%m-%d') VigenciaFinal,
	                cgec.combo Frequencia,
                    (SELECT id FROM subtipo_evento_calendario_sindical 
                    WHERE nome = CONCAT(gc.nome_grupo, ' - ', at2.nmtipoinformacaoadicional)) SubtipoEvento
                FROM clausula_geral_estrutura_clausula cgec
                LEFT JOIN clausula_geral_estrutura_clausula cgec2 ON 
                    cgec2.clausula_geral_id_clau = cgec.clausula_geral_id_clau 
                    and cgec2.grupo_dados = cgec.grupo_dados 
                    and cgec2.ad_tipoinformacaoadicional_cdtipoinformacaoadicional IN (24,27,28,29)
                LEFT JOIN clausula_geral_estrutura_clausula cgec3 ON 
                    cgec3.clausula_geral_id_clau = cgec.clausula_geral_id_clau 
                    and cgec3.grupo_dados = cgec.grupo_dados 
                    and cgec3.ad_tipoinformacaoadicional_cdtipoinformacaoadicional = 249
                LEFT JOIN doc_sind ds ON ds.id_doc = cgec.doc_sind_id_doc
                LEFT JOIN tipo_doc td ON td.idtipo_doc = ds.tipo_doc_idtipo_doc
                LEFT JOIN ad_tipoinformacaoadicional at2 ON at2.cdtipoinformacaoadicional = cgec2.ad_tipoinformacaoadicional_cdtipoinformacaoadicional
                LEFT JOIN estrutura_clausula ec2 ON ec2.id_estruturaclausula = cgec.nome_informacao
                LEFT JOIN grupo_clausula gc ON gc.idgrupo_clausula = ec2.grupo_clausula_idgrupo_clausula
                WHERE cgec.estrutura_clausula_id_estruturaclausula IN (
                      SELECT ec.id_estruturaclausula FROM estrutura_clausula ec
                      WHERE ec.calendario = 'S'
                )
                      AND cgec.ad_tipoinformacaoadicional_cdtipoinformacaoadicional IN (11)
                      AND CASE WHEN cgec.ad_tipoinformacaoadicional_cdtipoinformacaoadicional = 11 THEN cgec.combo <> 'Não se aplica' and cgec.combo <> 'Não Consta' ELSE true END
                      AND cgec2.`data` is not null
                      AND ds.modulo = 'SISAP'
                      AND ds.data_aprovacao is not null
                      AND (cgec.combo = 'Mensal' OR cgec.combo = 'mensal')
            ", new Dictionary<string, object>());

            if (eventosPeriodicosBase != null)
            {
                foreach (var evento in eventosPeriodicosBase.ToList())
                {
                    if (evento.Frequencia == "Mensal" || evento.Frequencia == "mensal")
                    {
                        var dataReferenciaBase = evento.DataReferencia!.Value;

                        var diaBase = evento.DataReferencia!.Value.Day;

                        while (dataReferenciaBase < evento.VigenciaFinal)
                        {
                            var ano = dataReferenciaBase.Year;
                            var mes = dataReferenciaBase.Month;
                            int diasNoMes = DateTime.DaysInMonth(ano, mes);

                            if (diaBase <= diasNoMes)
                            {
                                dataReferenciaBase = new DateOnly(ano, mes, diaBase);
                            }

                            var parameters = new List<MySqlParameter>();
                            parameters.Add(new MySqlParameter("@chaveReferencia", evento.ChaveReferencia));
#pragma warning disable CA1305
                            parameters.Add(new MySqlParameter("@dataReferencia", dataReferenciaBase.ToString("yyyy-MM-dd")));
                            parameters.Add(new MySqlParameter("@subtipoEvento", evento.SubtipoEvento));
#pragma warning restore CA1305

                            await _context.Database.ExecuteSqlRawAsync(@"
                                INSERT INTO calendario_sindical_tb (chave_referencia, tipo_evento, origem, notificar_antes, data_referencia)
                                SELECT
                                    @chaveReferencia chave_referencia,
                                    5 tipo_evento,
                                    @subtipoEvento subtipo_evento,
                                    1 origem,
                                    '120:00:00' notificar_antes,
                                    @dataReferencia data_referencia    
                                WHERE NOT EXISTS (
                                        SELECT 1 FROM calendario_sindical_tb cs
                                        WHERE cs.chave_referencia = @chaveReferencia
                                            and cs.tipo_evento = 5
                                            and cs.origem = 1
                                            and cs.data_referencia = @dataReferencia
                                      )
                            ", parameters, cancellationToken);

                            dataReferenciaBase = dataReferenciaBase.AddMonths(1);
                        }
                    }
                }
            }
        }

        private async ValueTask IncluirEventosAgenda(CancellationToken cancellationToken)
        {
            var eventosPeriodicosBase = await _context.CalendariosSindicaisUsuario.Select(csu => new EventoAgendaBaseDto
            {
                ChaveReferencia = (int)csu.Id,
                DataReferencia = csu.DataHora,
                ValidadeRecorrencia = csu.ValidadeRecorrencia,
                Frequencia = (TipoRecorrencia)csu.Recorrencia!,
                NotificarAntes = csu.NotificarAntes,
            }).ToListAsync(cancellationToken);

            if (eventosPeriodicosBase is not null)
            {
                foreach(var evento in eventosPeriodicosBase)
                {
                    if (evento.DataReferencia.HasValue)
                    {
                        if (evento.Frequencia == TipoRecorrencia.NaoRepetir)
                        {
                            await ExecutarAdicaoEventoAgendaAsync(evento, evento.DataReferencia.Value, cancellationToken);
                        }
                        else
                        {
                            var diaBase = evento.DataReferencia.Value.Day;
                            var dataReferencia = evento.DataReferencia.Value;

                            while (dataReferencia <= evento.ValidadeRecorrencia)
                            {
                                await ExecutarAdicaoEventoAgendaAsync(evento, dataReferencia, cancellationToken);
                         
                                var novaDataReferencia = EventoAgendaProximaDataFactory.Criar(evento.Frequencia, dataReferencia, diaBase);
                                
                                if (novaDataReferencia.IsFailure)
                                {
                                    throw new ArgumentException(novaDataReferencia.Error);
                                }

                                dataReferencia = novaDataReferencia.Value;
                            }
                        }
                    }
                    
                }
            }
        }

        private async ValueTask ExecutarAdicaoEventoAgendaAsync(EventoAgendaBaseDto evento, DateTime dataReferencia, CancellationToken cancellationToken)
        {
            var parameters = new List<MySqlParameter>
            {
                new MySqlParameter("@chaveReferencia", evento.ChaveReferencia),
                new MySqlParameter("@notificarAntes", evento.NotificarAntes),
                new MySqlParameter("@dataReferencia", dataReferencia.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture)),
                new MySqlParameter("@tipoEvento", TipoEventoCalendarioSindical.AgendaEventos.Id),
                new MySqlParameter("@origem", (int)OrigemEvento.Cliente),
            };

            await _context.Database.ExecuteSqlRawAsync(@"
                INSERT INTO calendario_sindical_tb (chave_referencia, tipo_evento, origem, notificar_antes, data_referencia)
                SELECT
                    @chaveReferencia chave_referencia,
                    @tipoEvento tipo_evento,
                    @origem origem,
                    @notificarAntes notificar_antes,
                    @dataReferencia data_referencia    
                WHERE NOT EXISTS (
                        SELECT 1 FROM calendario_sindical_tb cs
                        WHERE cs.chave_referencia = @chaveReferencia
                            and cs.tipo_evento = @tipoEvento
                            and cs.origem = @origem
                            and cs.data_referencia = @dataReferencia
                        )
            ", parameters, cancellationToken);
        }
    }
}
