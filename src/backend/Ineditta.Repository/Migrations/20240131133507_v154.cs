using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ineditta.Repository.Migrations
{
    /// <inheritdoc />
    public partial class v154 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                                        localizacoes.ufs AS ufs,
                                        localizacoes.municipios AS municipios,
                                        cct.data_processamento AS data_processamento
                                    from acompanhamento_cct_tb cct
                                    left join fase_cct fc on cct.fase_id = fc.id_fase
                                    left join tipo_doc tdt on cct.tipo_documento_id = tdt.idtipo_doc
                                    left join indecon_real ir on ir.periodo_data is not null
                                        and cct.data_base = ConvertMonthYear_ENtoPT(date_format((ir.periodo_data + interval 1 month), '%b/%Y'))
                                        and ir.indicador = 'INPC'
                                    left join lateral (
                                        select group_concat(cnaet.descricao_subclasse separator ', ') AS atividades_economicas from classe_cnae cnaet
                                        where json_contains(cct.cnaes_ids, concat('[""', cast(cnaet.id_cnae as char charset utf8mb4), '""]'))) cnaet on true
                                    left join lateral (
                                        select
                                            json_arrayagg(json_object('nome',
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
                                            cm.nome_empresa)
                                        ) AS estabelecimentos
                                        from acompanhamento_cct_estabelecimento_tb acet
                                        join cliente_unidades cut on acet.estabelecimento_id = cut.id_unidade
                                        join cliente_matriz cm on acet.empresa_id = cm.id_empresa
                                        where acet.acompanhamento_cct_id = cct.id
                                    ) acet on true
                                    left join lateral (
	                                    select json_arrayagg(
                                            json_object(
                                                'sigla', COALESCE(se.sigla_sinde, ''),
                                                'cnpj', COALESCE(se.cnpj_sinde, ''),
                                                'uf', COALESCE(se.uf_sinde, ''),
                                                'nome', COALESCE(se.denominacao_sinde, '')
                                            )
                                        ) AS sindicatos
                                        from acompanhamento_cct_sindicato_laboral_tb acspt
                                        join sind_emp se on acspt.sindicato_id = se.id_sinde
                                        where acspt.acompanhamento_cct_id = cct.id
                                    ) sindicator_laborais on true
                                    left join lateral (
                                        SELECT JSON_ARRAYAGG(
	                                        JSON_OBJECT(
	                                            'sigla', COALESCE(sp.sigla_sp, ''),
	                                            'cnpj', COALESCE(sp.cnpj_sp, ''),
	                                            'uf', COALESCE(sp.uf_sp, ''),
	                                            'nome', COALESCE(sp.denominacao_sp, '')
	                                        )
	                                    ) AS sindicatos
	                                    FROM acompanhamento_cct_sindicato_patronal_tb acsl
	                                    INNER JOIN sind_patr sp ON acsl.sindicato_id = sp.id_sindp
	                                    WHERE acsl.acompanhamento_cct_id = cct.id
                                    ) sindicatos_patronais on true
                                    left join lateral (
                                        select
                                            json_arrayagg(l.uf) AS ufs,
                                            json_arrayagg(l.municipio) AS municipios
                                        from acompanhamento_cct_localizacao_tb aclt
                                        join localizacao l on aclt.localizacao_id = l.id_localizacao
                                        where aclt.acompanhamento_cct_id = cct.id
                                    ) localizacoes on true;");

            migrationBuilder.Sql(@"CREATE OR REPLACE VIEW acompanhamento_cct_inclusao_vw AS
                                    select
                                        acct.id AS id,
                                        acct.data_inicial AS data_inicial,
                                        acct.data_final AS data_final,
                                        acct.data_alteracao AS data_alteracao,
                                        stcct.descricao AS status,
                                        ua.nome_usuario AS nome_usuario,
                                        fcct.fase_negociacao AS fase,
                                        td.nome_doc AS nome_documento,
                                        acct.proxima_ligacao AS proxima_ligacao,
                                        acct.data_base AS data_base,
                                        sindicatos_patronais.siglas AS sigla_sindicato_patronal,
                                        sindicatos_patronais.ufs AS uf_sindicato_patronal,
                                        sindicatos_laborais.siglas AS sigla_sindicato_empregado,
                                        sindicatos_laborais.ufs AS uf_sindicato_empregado,
                                        cnaes.descricao_subclasse AS descricao_sub_classe
                                    from acompanhamento_cct_tb acct
                                    left join usuario_adm ua on acct.usuario_responsavel_id = ua.id_user
                                    left join fase_cct fcct on acct.fase_id = fcct.id_fase
                                    left join acompanhamento_cct_status_tb stcct on acct.status = stcct.id
                                    left join tipo_doc td on acct.tipo_documento_id = td.idtipo_doc
                                    left join lateral (
                                        select
                                            group_concat(se.id_sinde separator ', ') AS ids,
                                            group_concat(se.sigla_sinde separator ', ') AS siglas,
                                            group_concat(se.cnpj_sinde separator ', ') AS cnpjs,
                                            group_concat(se.uf_sinde separator ', ') AS ufs,
                                            group_concat(se.codigo_sinde separator ', ') AS codigos
                                        from acompanhamento_cct_sindicato_laboral_tb acslt
                                        join sind_emp se on acslt.sindicato_id = se.id_sinde
                                        where acslt.acompanhamento_cct_id = acct.id
                                    ) sindicatos_laborais on true
                                    left join lateral (
                                        select
                                            group_concat(sp.id_sindp separator ', ') AS ids,
                                            group_concat(sp.sigla_sp separator ', ') AS siglas,
                                            group_concat(sp.cnpj_sp separator ', ') AS cnpjs,
                                            group_concat(sp.uf_sp separator ', ') AS ufs,
                                            group_concat(sp.codigo_sp separator ', ') AS codigos
                                        from acompanhamento_cct_sindicato_patronal_tb acspt
                                        join sind_patr sp on acspt.sindicato_id = sp.id_sindp
                                        where acspt.acompanhamento_cct_id = acct.id
                                    ) sindicatos_patronais on true
                                    left join lateral (
	                                    select GROUP_CONCAT(ccnae.descricao_subclasse separator ', ') as descricao_subclasse  from classe_cnae ccnae 
	                                    WHERE json_contains(acct.cnaes_ids, json_array(cast(ccnae.id_cnae as char)))
                                    ) cnaes on true
                                    order by
                                        acct.proxima_ligacao desc,
                                        acct.status;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
                                        localizacoes.ufs AS ufs,
                                        localizacoes.municipios AS municipios,
                                        cct.data_processamento AS data_processamento
                                    from acompanhamento_cct_tb cct
                                    left join fase_cct fc on cct.fase_id = fc.id_fase
                                    left join tipo_doc tdt on cct.tipo_documento_id = tdt.idtipo_doc
                                    left join indecon_real ir on ir.periodo_data is not null
                                        and cct.data_base = ConvertMonthYear_ENtoPT(date_format((ir.periodo_data + interval 1 month), '%b/%Y'))
                                        and ir.indicador = 'INPC'
                                    left join lateral (
                                        select group_concat(cnaet.descricao_subclasse separator ', ') AS atividades_economicas from classe_cnae cnaet
                                        where json_contains(cct.cnaes_ids, concat('[""', cast(cnaet.id_cnae as char charset utf8mb4), '""]'))) cnaet on true
                                    left join lateral (
                                        select
                                            json_arrayagg(json_object('nome',
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
                                            cm.nome_empresa)
                                        ) AS estabelecimentos
                                        from acompanhamento_cct_estabelecimento_tb acet
                                        join cliente_unidades cut on acet.estabelecimento_id = cut.id_unidade
                                        join cliente_matriz cm on acet.empresa_id = cm.id_empresa
                                        where acet.acompanhamento_cct_id = cct.id
                                    ) acet on true
                                    left join lateral (
	                                    select json_arrayagg(
                                            json_object(
                                                'sigla', COALESCE(se.sigla_sinde, ''),
                                                'cnpj', COALESCE(se.cnpj_sinde, ''),
                                                'uf', COALESCE(se.uf_sinde, ''),
                                                'nome', COALESCE(se.denominacao_sinde, '')
                                            )
                                        ) AS sindicatos
                                        from acompanhamento_cct_sindicato_laboral_tb acspt
                                        join sind_emp se on acspt.sindicato_id = se.id_sinde
                                        where acspt.acompanhamento_cct_id = cct.id
                                    ) sindicator_laborais on true
                                    left join lateral (
                                        SELECT JSON_ARRAYAGG(
	                                        JSON_OBJECT(
	                                            'sigla', COALESCE(sp.sigla_sp, ''),
	                                            'cnpj', COALESCE(sp.cnpj_sp, ''),
	                                            'uf', COALESCE(sp.uf_sp, ''),
	                                            'nome', COALESCE(sp.denominacao_sp, '')
	                                        )
	                                    ) AS sindicatos
	                                    FROM acompanhamento_cct_sindicato_patronal_tb acsl
	                                    INNER JOIN sind_patr sp ON acsl.sindicato_id = sp.id_sindp
	                                    WHERE acsl.id = cct.id
                                    ) sindicatos_patronais on true
                                    left join lateral (
                                        select
                                            json_arrayagg(l.uf) AS ufs,
                                            json_arrayagg(l.municipio) AS municipios
                                        from acompanhamento_cct_localizacao_tb aclt
                                        join localizacao l on aclt.localizacao_id = l.id_localizacao
                                        where aclt.acompanhamento_cct_id = cct.id
                                    ) localizacoes on true;");


            migrationBuilder.Sql(@"CREATE OR REPLACE VIEW acompanhamento_cct_inclusao_vw AS
                                select
                                    acct.id AS id,
                                    acct.data_inicial AS data_inicial,
                                    acct.data_final AS data_final,
                                    acct.data_alteracao AS data_alteracao,
                                    stcct.descricao AS status,
                                    ua.nome_usuario AS nome_usuario,
                                    fcct.fase_negociacao AS fase,
                                    td.nome_doc AS nome_documento,
                                    acct.proxima_ligacao AS proxima_ligacao,
                                    acct.data_base AS data_base,
                                    sindicatos_patronais.siglas AS sigla_sindicato_patronal,
                                    sindicatos_patronais.ufs AS uf_sindicato_patronal,
                                    sindicatos_laborais.siglas AS sigla_sindicato_empregado,
                                    sindicatos_laborais.ufs AS uf_sindicato_empregado,
                                    ccnae.descricao_subclasse AS descricao_sub_classe
                                from acompanhamento_cct_tb acct
                                left join usuario_adm ua on acct.usuario_responsavel_id = ua.id_user
                                left join fase_cct fcct on acct.fase_id = fcct.id_fase
                                left join acompanhamento_cct_status_tb stcct on acct.status = stcct.id
                                left join tipo_doc td on acct.tipo_documento_id = td.idtipo_doc
                                left join classe_cnae ccnae on json_contains(acct.cnaes_ids, json_array(cast(ccnae.id_cnae as char charset utf8mb4)))
                                left join lateral (
                                    select
                                        group_concat(se.id_sinde separator ', ') AS ids,
                                        group_concat(se.sigla_sinde separator ', ') AS siglas,
                                        group_concat(se.cnpj_sinde separator ', ') AS cnpjs,
                                        group_concat(se.uf_sinde separator ', ') AS ufs,
                                        group_concat(se.codigo_sinde separator ', ') AS codigos
                                    from acompanhamento_cct_sindicato_laboral_tb acslt
                                    join sind_emp se on acslt.sindicato_id = se.id_sinde
                                    where acslt.acompanhamento_cct_id = acct.id
                                ) sindicatos_laborais on true
                                left join lateral (
                                    select
                                        group_concat(sp.id_sindp separator ', ') AS ids,
                                        group_concat(sp.sigla_sp separator ', ') AS siglas,
                                        group_concat(sp.cnpj_sp separator ', ') AS cnpjs,
                                        group_concat(sp.uf_sp separator ', ') AS ufs,
                                        group_concat(sp.codigo_sp separator ', ') AS codigos
                                    from acompanhamento_cct_sindicato_patronal_tb acspt
                                    join sind_patr sp on acspt.sindicato_id = sp.id_sindp
                                    where acspt.acompanhamento_cct_id = acct.id
                                ) sindicatos_patronais on true
                                order by
                                    acct.proxima_ligacao desc,
                                    acct.status;");
        }
    }
}
