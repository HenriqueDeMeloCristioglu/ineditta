using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ineditta.Repository.Migrations
{
    /// <inheritdoc />
    public partial class v150 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"drop view if exists acompanhamento_cct_relatorio_vw;");

            migrationBuilder.Sql(@"update cliente_matriz set codigo_empresa = null where codigo_empresa = '-';");

            migrationBuilder.Sql(@"CREATE OR REPLACE VIEW acompanhamento_cct_relatorio_vw AS
                                    select
                                        cct.id AS id,
                                        tdt.nome_doc AS nome_documento,
                                        cct.data_base AS data_base,
                                        fc.fase_negociacao AS fase,
                                        cct.observacoes_gerais AS observacoes_gerais,
                                        ConvertMonthYear_ENtoPT(ir.periodo_data) AS periodo_anterior,
                                        ir.indicador AS indicador,
                                        ir.dado_real AS dado_real,
                                        cct.cnaes_ids AS cnaes_ids,
                                        cct.fase_id AS fase_id,
                                        date_format((ir.periodo_data + interval 1 month), '%b/%Y') AS ir_periodo,
                                        cnaet.atividades_economicas AS atividades_economicas,
                                        acet.estabelecimentos AS estabelecimentos,
                                        sindicator_laborais.sindicatos AS sindicatos_laborais,
                                        sindicatos_patronais.sindicatos AS sindicatos_patronais,
                                        localizacoes.ufs ufs,
                                        localizacoes.municipios municipios,
                                        cct.data_processamento
                                    from acompanhamento_cct_tb cct
                                    left join fase_cct fc on cct.fase_id = fc.id_fase
                                    left join tipo_doc tdt on cct.tipo_documento_id = tdt.idtipo_doc
                                    left join indecon_real ir on ir.periodo_data is not null
                                        and cct.data_base = ConvertMonthYear_ENtoPT(date_format((ir.periodo_data + interval 1 month), '%b/%Y'))
                                        and ir.indicador = 'INPC'
                                    left join lateral (
                                        select
                                            group_concat(cnaet.descricao_subclasse separator ', ') AS atividades_economicas
                                        from
                                            classe_cnae cnaet
                                        where
                                            json_contains(cct.cnaes_ids,
                                            concat('[""', cast(cnaet.id_cnae as char charset utf8mb4), '""]'))
                                    ) cnaet on true
                                    left join lateral (
                                        select
	                                        json_arrayagg(
	                                            json_object('nome',
		                                            cut.nome_unidade,
		                                            'cnpj',
		                                            cut.cnpj_unidade,
		                                            'grupoEconomicoId',
		                                            cut.cliente_grupo_id_grupo_economico,
		                                            'codigoSindicatoCliente',
		                                            cut.cod_sindcliente,
		                                            'codigoEmpresa',
		                                            cm.codigo_empresa,
		                                            'nomeEmpresa',
		                                            cm.nome_empresa
	                                            )
	                                        ) AS estabelecimentos
                                        from acompanhamento_cct_estabelecimento_tb acet
                                        join cliente_unidades cut on acet.estabelecimento_id = cut.id_unidade
                                        join cliente_matriz cm on acet.empresa_id = cm.id_empresa   
                                        where acet.acompanhamento_cct_id = cct.id
                                    ) acet on true
                                    left join lateral (
                                        select
                                            json_arrayagg(
	                                            json_object(
		                                            'sigla',
		                                            se.sigla_sinde,
		                                            'cnpj',
		                                            se.cnpj_sinde,
		                                            'uf',
		                                            se.uf_sinde,
		                                            'nome',
		                                            se.denominacao_sinde
	                                            )
                                            ) AS sindicatos
                                        from acompanhamento_cct_sindicato_laboral_tb acspt
                                        join sind_emp se on acspt.sindicato_id = se.id_sinde
                                        where acspt.acompanhamento_cct_id = cct.id
                                    ) sindicator_laborais on true
                                    left join lateral (
                                        select
                                            json_arrayagg(
        	                                    json_object('sigla',
		                                            sp.sigla_sp,
		                                            'cnpj',
		                                            sp.cnpj_sp,
		                                            'uf',
		                                            sp.uf_sp,
		                                            'nome',
		                                            sp.denominacao_sp
	                                            )
                                            ) AS sindicatos
                                        from acompanhamento_cct_sindicato_patronal_tb acsl
                                        join sind_patr sp on acsl.sindicato_id = sp.id_sindp
                                        where
                                            acsl.id = cct.id
                                    ) sindicatos_patronais on true
                                    left join lateral (
	                                    select json_arrayagg(l.uf) as ufs, json_arrayagg(l.municipio) as municipios  from acompanhamento_cct_localizacao_tb aclt
	                                    inner join localizacao l on aclt.localizacao_id = l.id_localizacao
	                                    where aclt.acompanhamento_cct_id  = cct.id
                                    ) as localizacoes on true;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"drop view if exists acompanhamento_cct_relatorio_vw;");

            migrationBuilder.Sql(@"CREATE OR REPLACE VIEW acompanhamento_cct_relatorio_vw AS
                    select
                        cct.id,
                        tdt.nome_doc AS nome_documento,
                        cct.data_base AS data_base,
                        fc.fase_negociacao AS fase,
                        cct.observacoes_gerais AS observacoes_gerais,
                        date_format(cct.data_alteracao, '%d/%m/%Y') AS ultima_atualizacao,
                        ConvertMonthYear_ENtoPT(ir.periodo_data) AS periodo_anterior,
                        ir.indicador,
                        ir.dado_real,
                        cct.cnaes_ids,
                        cct.fase_id,
                        date_format((ir.periodo_data + interval 1 month), '%b/%Y') AS ir_periodo,
                        cnaet.atividades_economicas AS atividades_economicas,
                        acet.estabelecimentos AS estabelecimentos,
                        sindicator_laborais.sindicatos AS sindicatos_laborais,
                        sindicatos_patronais.sindicatos AS sindicatos_patronais
                    from acompanhamento_cct_tb cct
                    left join fase_cct fc on cct.fase_id = fc.id_fase
                    left join tipo_doc tdt on cct.tipo_documento_id = tdt.idtipo_doc
                    left join indecon_real ir on ir.periodo_data is not null
                        and cct.data_base = ConvertMonthYear_ENtoPT(date_format((ir.periodo_data + interval 1 month), '%b/%Y'))
	                    and ir.indicador = 'INPC'
                    left join lateral (
                        select group_concat(cnaet.descricao_subclasse separator ', ') AS atividades_economicas from classe_cnae cnaet
                        where json_contains(cct.cnaes_ids, concat('[""', cast(cnaet.id_cnae as char charset utf8mb4), '""]'))
                    ) cnaet on true
                    left join lateral (
                        select json_arrayagg(
    	                    json_object('nome',
                            cut.nome_unidade,
                            'cnpj',
                            cut.cnpj_unidade,
                            'grupoEconomicoId',
                            cut.cliente_grupo_id_grupo_economico,
                            'codigoSindicatoCliente',
                            cut.cod_sindcliente)
	                    ) AS estabelecimentos
	                    from acompanhamento_cct_estabelecimento_tb acet
	                    join cliente_unidades cut on acet.estabelecimento_id = cut.id_unidade
	                    where acet.acompanhamento_cct_id = cct.id
                     ) acet on true
                    left join lateral (
                        select json_arrayagg(
                        json_object('sigla',
                            se.sigla_sinde,
                            'cnpj',
                            se.cnpj_sinde,
                            'uf',
                            se.uf_sinde)
	                    ) AS sindicatos
                        from acompanhamento_cct_sindicato_laboral_tb acspt
                        inner join sind_emp se on acspt.sindicato_id = se.id_sinde
                        where acspt.acompanhamento_cct_id = cct.id
                    ) sindicator_laborais on true
                    left join lateral (
                        select json_arrayagg(
                        json_object(
    	                    'sigla',
                            sp.sigla_sp,
                            'cnpj',
                            sp.cnpj_sp,
                            'uf',
                            sp.uf_sp
		                    )
	                    ) AS sindicatos
                        from acompanhamento_cct_sindicato_patronal_tb acsl
                        inner join sind_patr sp on acsl.sindicato_id = sp.id_sindp
                        where acsl.id = cct.id
                    ) sindicatos_patronais on true;");
        }
    }
}
