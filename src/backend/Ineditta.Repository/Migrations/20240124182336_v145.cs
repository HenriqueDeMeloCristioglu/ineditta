using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ineditta.Repository.Migrations
{
    /// <inheritdoc />
    public partial class v145 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP VIEW IF EXISTS acompanhamento_cct_vw;");

            migrationBuilder.Sql(@"CREATE OR REPLACE VIEW acompanhamento_cct_vw AS
                                    select
                                        cct.id AS id,
                                        tdt.nome_doc AS nome_documento,
                                        cct.data_base AS data_base,
                                        fc.fase_negociacao AS fase,
                                        fc.id_fase AS fase_id,
                                        cct.observacoes_gerais AS observacoes_gerais,
                                        cct.data_processamento,
                                        ConvertMonthYear_ENtoPT(ir.periodo_data) AS periodo_anterior,
                                        ir.indicador AS indicador,
                                        ir.dado_real AS dado_real,
                                        (convert(date_format((ir.periodo_data + interval 1 month), '%b/%Y')
                                            using utf8mb4) collate utf8mb4_0900_ai_ci) AS ir_periodo,
                                        cnaet.atividades_economicas AS atividades_economicas,
                                        cct.cnaes_ids AS ids_cnaes,
                                        sindicatos_laborais.ids sindicatos_laborais_ids,
                                        sindicatos_laborais.cnpjs sindicatos_laborais_cnpjs,
                                        sindicatos_laborais.ufs sindicatos_laborais_ufs,
                                        sindicatos_laborais.siglas sindicatos_laborais_siglas,
                                        sindicatos_laborais.codigos sindicatos_laborais_codigos,
                                        sindicatos_patronais.ids sindicatos_patronais_ids,
                                        sindicatos_patronais.cnpjs sindicatos_patronais_cnpjs,
                                        sindicatos_patronais.ufs sindicatos_patronais_ufs,
                                        sindicatos_patronais.siglas sindicatos_patronais_siglas,
                                        sindicatos_patronais.codigos sindicatos_patronais_codigos
                                    from acompanhamento_cct_tb cct
                                    left join tipo_doc tdt on cct.tipo_documento_id = tdt.idtipo_doc
                                    left join fase_cct fc on cct.fase_id = fc.id_fase
                                    left join indecon_real ir on ir.periodo_data is not null
                                        and cct.data_base = ConvertMonthYear_ENtoPT(date_format((ir.periodo_data + interval 1 month), '%b/%Y'))
                                        and ir.indicador = 'INPC'
                                    left join lateral (
                                        select group_concat(cnaet.descricao_subclasse separator ', ') AS atividades_economicas
                                        from classe_cnae cnaet
                                        where json_contains(cct.cnaes_ids, json_array(cast(cnaet.id_cnae as char charset utf8mb4)))) cnaet on true
                                    left join lateral (
                                        select
                                            GROUP_CONCAT(se.id_sinde separator "", "") ids,
                                            GROUP_CONCAT(se.sigla_sinde separator "", "") siglas,
                                            GROUP_CONCAT(se.cnpj_sinde separator "", "") cnpjs,
                                            GROUP_CONCAT(se.uf_sinde separator "", "") ufs,
                                            GROUP_CONCAT(se.codigo_sinde separator "", "") codigos
                                        from acompanhamento_cct_sindicato_laboral_tb acslt
                                        inner join sind_emp se on acslt.sindicato_id = se.id_sinde
                                        where acslt.acompanhamento_cct_id = cct.id
                                    ) sindicatos_laborais on true
                                    left join lateral (
                                        select
                                            GROUP_CONCAT(sp.id_sindp separator "", "") ids,
                                            GROUP_CONCAT(sp.sigla_sp separator "", "") siglas,
                                            GROUP_CONCAT(sp.cnpj_sp separator "", "") cnpjs,
                                            GROUP_CONCAT(sp.uf_sp separator "", "") ufs,
                                            GROUP_CONCAT(sp.codigo_sp separator "", "") codigos
                                        from acompanhamento_cct_sindicato_patronal_tb acspt
                                        inner join sind_patr sp on acspt.sindicato_id = sp.id_sindp
                                        where acspt.acompanhamento_cct_id = cct.id
                                    ) sindicatos_patronais on true;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP VIEW IF EXISTS acompanhamento_cct_vw;");

            migrationBuilder.Sql(@"CREATE OR REPLACE VIEW acompanhamento_cct_vw AS
                                select
                                    cct.id AS id,
                                    tdt.nome_doc AS nome_documento,
                                    cct.data_base AS data_base,
                                    fc.fase_negociacao AS fase,
                                    fc.id_fase AS fase_id,
                                    cct.observacoes_gerais AS observacoes_gerais,
                                    date_format(cct.data_alteracao, '%d/%m/%Y') AS ultima_atualizacao,
                                    ConvertMonthYear_ENtoPT(ir.periodo_data) AS periodo_anterior,
                                    ir.indicador AS indicador,
                                    ir.dado_real AS dado_real,
                                    (convert(date_format((ir.periodo_data + interval 1 month), '%b/%Y')
                                        using utf8mb4) collate utf8mb4_0900_ai_ci) AS ir_periodo,
                                    cnaet.atividades_economicas AS atividades_economicas,
                                    cct.cnaes_ids AS ids_cnaes,
                                    sindicatos_laborais.ids sindicatos_laborais_ids,
                                    sindicatos_laborais.cnpjs sindicatos_laborais_cnpjs,
                                    sindicatos_laborais.ufs sindicatos_laborais_ufs,
                                    sindicatos_laborais.siglas sindicatos_laborais_siglas,
                                    sindicatos_laborais.codigos sindicatos_laborais_codigos,
                                    sindicatos_patronais.ids sindicatos_patronais_ids,
                                    sindicatos_patronais.cnpjs sindicatos_patronais_cnpjs,
                                    sindicatos_patronais.ufs sindicatos_patronais_ufs,
                                    sindicatos_patronais.siglas sindicatos_patronais_siglas,
                                    sindicatos_patronais.codigos sindicatos_patronais_codigos
                                from acompanhamento_cct_tb cct
                                left join tipo_doc tdt on cct.tipo_documento_id = tdt.idtipo_doc
                                left join fase_cct fc on cct.fase_id = fc.id_fase
                                left join indecon_real ir on ir.periodo_data is not null
                                    and cct.data_base = ConvertMonthYear_ENtoPT(date_format((ir.periodo_data + interval 1 month), '%b/%Y'))
                                    and ir.indicador = 'INPC'
                                left join lateral (
                                    select group_concat(cnaet.descricao_subclasse separator ', ') AS atividades_economicas
                                    from classe_cnae cnaet
                                    where json_contains(cct.cnaes_ids, json_array(cast(cnaet.id_cnae as char charset utf8mb4)))) cnaet on true
                                left join lateral (
	                                select
		                                GROUP_CONCAT(se.id_sinde separator "", "") ids,
		                                GROUP_CONCAT(se.sigla_sinde separator "", "") siglas,
		                                GROUP_CONCAT(se.cnpj_sinde separator "", "") cnpjs,
		                                GROUP_CONCAT(se.uf_sinde separator "", "") ufs,
		                                GROUP_CONCAT(se.codigo_sinde separator "", "") codigos
	                                from acompanhamento_cct_sindicato_laboral_tb acslt
	                                inner join sind_emp se on acslt.sindicato_id = se.id_sinde
	                                where acslt.acompanhamento_cct_id = cct.id
                                ) sindicatos_laborais on true
                                left join lateral (
	                                select
		                                GROUP_CONCAT(sp.id_sindp separator "", "") ids,
		                                GROUP_CONCAT(sp.sigla_sp separator "", "") siglas,
		                                GROUP_CONCAT(sp.cnpj_sp separator "", "") cnpjs,
		                                GROUP_CONCAT(sp.uf_sp separator "", "") ufs,
		                                GROUP_CONCAT(sp.codigo_sp separator "", "") codigos
	                                from acompanhamento_cct_sindicato_patronal_tb acspt
	                                inner join sind_patr sp on acspt.sindicato_id = sp.id_sindp
	                                where acspt.acompanhamento_cct_id = cct.id
                                ) sindicatos_patronais on true;");
        }
    }
}
