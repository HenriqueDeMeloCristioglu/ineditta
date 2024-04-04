using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ineditta.Repository.Migrations
{
    /// <inheritdoc />
    public partial class v136 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "acompanhamento_cct",
                newName: "acompanhamento_cct_tb");

            migrationBuilder.RenameIndex(
                name: "IX_acompanhamento_cct_status",
                table: "acompanhamento_cct_tb",
                newName: "IX_acompanhamento_cct_tb_status");

            migrationBuilder.Sql(@"DROP VIEW if exists acompanhamento_cct_inclusao_vw;");

            migrationBuilder.Sql(@"create or replace view acompanhamento_cct_inclusao_vw as
                                    SELECT
                                        acct.id,
                                        acct.data_inicial,
                                        acct.data_final,
                                        acct.data_alteracao,
                                        stcct.descricao status,
                                        ua.nome_usuario,
                                        fcct.fase_negociacao fase,
                                        td.nome_doc nome_documento,
                                        acct.proxima_ligacao,
                                        acct.data_base,
                                        sindicatos_patronais.siglas sigla_sindicato_patronal,
                                        sindicatos_patronais.ufs uf_sindicato_patronal,
                                        sindicatos_laborais.siglas sigla_sindicato_empregado,
                                        sindicatos_laborais.ufs uf_sindicato_empregado,
                                        ccnae.descricao_subclasse descricao_sub_classe
                                    FROM
                                        acompanhamento_cct_tb acct
                                        LEFT JOIN usuario_adm ua ON acct.usuario_responsavel_id = ua.id_user
                                        LEFT JOIN fase_cct fcct ON acct.fase_id = fcct.id_fase
                                        LEFT JOIN acompanhamento_cct_status_tb stcct ON acct.status = stcct.id
                                        LEFT JOIN tipo_doc td ON acct.tipo_documento_id = td.idtipo_doc
                                        LEFT JOIN classe_cnae ccnae ON JSON_CONTAINS(acct.cnaes_ids, json_array(cast(ccnae.id_cnae as char)))
                                        left join lateral (
                                            select
                                                GROUP_CONCAT(se.id_sinde separator "", "") ids,
                                                GROUP_CONCAT(se.sigla_sinde separator "", "") siglas,
                                                GROUP_CONCAT(se.cnpj_sinde separator "", "") cnpjs,
                                                GROUP_CONCAT(se.uf_sinde separator "", "") ufs,
                                                GROUP_CONCAT(se.codigo_sinde separator "", "") codigos
                                            from acompanhamento_cct_sindicato_laboral_tb acslt
                                            inner join sind_emp se on acslt.sindicato_id = se.id_sinde
                                            where acslt.acompanhamento_cct_id = acct.id
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
                                            where acspt.acompanhamento_cct_id = acct.id
                                        ) sindicatos_patronais on true
                                    ORDER BY
                                        acct.proxima_ligacao DESC,
                                        acct.status;");

            migrationBuilder.Sql(@"DROP VIEW if exists acompanhamento_cct_vw;");

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
                                ) sindicatos_patronais on true;
            ");

            migrationBuilder.Sql(@"DROP VIEW if exists acompanhamento_cct_relatorio_vw;");

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

            migrationBuilder.Sql(@"DROP PROCEDURE inserir_estabelecimentos_sindicatos_acompanhamento;");

            migrationBuilder.Sql(@"CREATE PROCEDURE inserir_estabelecimentos_acompanhamento()
                                BEGIN
                                    truncate acompanhamento_cct_estabelecimento_tb;

                                    -- Carga Estabelecimentos Laborais Nova
                                    insert into acompanhamento_cct_estabelecimento_tb(acompanhamento_cct_id, grupo_economico_id, empresa_id, estabelecimento_id)
                                    select distinct cct.id acompanhamento_cct_id, cut.cliente_grupo_id_grupo_economico grupo_economico_id, cut.cliente_matriz_id_empresa empresa_id, cut.id_unidade estabelecimento_id 
                                    from acompanhamento_cct_tb cct
                                    inner join lateral (
                                        select sempt.* from acompanhamento_cct_sindicato_laboral_tb acsl
                                        inner join sind_emp sempt on sempt.id_sinde = acsl.sindicato_id
                                        WHERE acsl.sindicato_id = sempt.id_sinde
                                        AND acsl.acompanhamento_cct_id = cct.id
                                    ) as sempt on true
                                    inner join base_territorialsindemp btset on sempt.id_sinde = btset.sind_empregados_id_sinde1
                                    inner join localizacao lt on btset.localizacao_id_localizacao1 = lt.id_localizacao
                                    inner join cliente_unidades cut on cut.localizacao_id_localizacao = lt.id_localizacao 
                                    inner join cnae_emp cet on cut.id_unidade = cet.cliente_unidades_id_unidade and btset.classe_cnae_idclasse_cnae = cet.classe_cnae_idclasse_cnae
                                    where not exists(select 1 from acompanhamento_cct_estabelecimento_tb acet
                                                    where cct.id = acet.acompanhamento_cct_id
                                                    and cut.cliente_grupo_id_grupo_economico = acet.grupo_economico_id
                                                    and cut.cliente_matriz_id_empresa = acet.empresa_id
                                                    and cut.id_unidade = acet.estabelecimento_id)
                                    and exists(
   		                                    select * from acompanhamento_cct_sindicato_patronal_tb acsp 
                                            inner join base_territorialsindpatro btspt on acsp.sindicato_id = btspt.sind_patronal_id_sindp
                                            inner join cnae_emp cet2 on cut.id_unidade = cet2.cliente_unidades_id_unidade and btspt.classe_cnae_idclasse_cnae = cet2.classe_cnae_idclasse_cnae
                                            where acsp.acompanhamento_cct_id = cct.id
                                            and cut.localizacao_id_localizacao = btspt.localizacao_id_localizacao1)
                                    and cut.cliente_grupo_id_grupo_economico is not null
                                    and cut.cliente_matriz_id_empresa is not null;

                                    -- Carga Estabelecimentos Grupo Economicos
                                    insert into acompanhamento_cct_estabelecimento_tb(acompanhamento_cct_id, grupo_economico_id, empresa_id, estabelecimento_id)
                                    select distinct cct.id acompanhamento_cct_id, cut.cliente_grupo_id_grupo_economico grupo_economico_id, cut.cliente_matriz_id_empresa empresa_id, cut.id_unidade estabelecimento_id 
                                    from acompanhamento_cct_tb cct 
                                    inner join cliente_unidades cut on JSON_CONTAINS(cct.grupos_economicos_ids, concat('[', cut.cliente_grupo_id_grupo_economico, ']'))
                                    where not exists(select 1 from acompanhamento_cct_estabelecimento_tb acet
                                                    where cct.id = acet.acompanhamento_cct_id
                                                    and cut.cliente_grupo_id_grupo_economico = acet.grupo_economico_id
                                                    and cut.cliente_matriz_id_empresa = acet.empresa_id
                                                    and cut.id_unidade = acet.estabelecimento_id);

                                    -- Carga Estabelecimentos Matrizes
                                    insert into acompanhamento_cct_estabelecimento_tb(acompanhamento_cct_id, grupo_economico_id, empresa_id, estabelecimento_id)
                                    select distinct cct.id id, cut.cliente_grupo_id_grupo_economico grupo_economico_id, cut.cliente_matriz_id_empresa empresa_id, cut.id_unidade estabelecimento_id 
                                    from acompanhamento_cct_tb cct 
                                    inner join cliente_unidades cut on JSON_CONTAINS(cct.empresas_ids, concat('[', cut.cliente_matriz_id_empresa, ']'))
                                    where not exists(select 1 from acompanhamento_cct_estabelecimento_tb acet
                                                    where cct.id = acet.acompanhamento_cct_id
                                                    and cut.cliente_grupo_id_grupo_economico = acet.grupo_economico_id
                                                    and cut.cliente_matriz_id_empresa = acet.empresa_id
                                                    and cut.id_unidade = acet.estabelecimento_id);
                                END;");

            migrationBuilder.Sql(@"CALL inserir_estabelecimentos_acompanhamento();");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "acompanhamento_cct_tb",
                newName: "acompanhamento_cct");

            migrationBuilder.RenameIndex(
                name: "IX_acompanhamento_cct_tb_status",
                table: "acompanhamento_cct",
                newName: "IX_acompanhamento_cct_status");

            migrationBuilder.Sql(@"DROP VIEW if exists acompanhamento_cct_inclusao_vw;");

            migrationBuilder.Sql(@"create or replace view acompanhamento_cct_inclusao_vw as
                                SELECT
                                    acct.id,
                                    acct.data_inicial,
                                    acct.data_final,
                                    acct.data_alteracao,
                                    stcct.descricao status,
                                    ua.nome_usuario,
                                    fcct.fase_negociacao fase,
                                    td.nome_doc nome_documento,
                                    acct.proxima_ligacao,
                                    acct.data_base,
                                    sindp.sigla_sp sigla_sindicato_patronal,
                                    sindp.uf_sp uf_sindicato_patronal,
                                    sindemp.sigla_sinde sigla_sindicato_empregado,
                                    sindemp.uf_sinde uf_sindicato_empregado,
                                    ccnae.descricao_subclasse descricao_sub_classe,
                                    acct.sindicato_laboral_id
                                FROM
                                    acompanhamento_cct acct
                                    LEFT JOIN usuario_adm ua ON acct.usuario_responsavel_id = ua.id_user
                                    LEFT JOIN fase_cct fcct ON acct.fase_id = fcct.id_fase
                                    LEFT JOIN acompanhamento_cct_status_tb stcct ON acct.status = stcct.id
                                    LEFT JOIN tipo_doc td ON acct.tipo_documento_id = td.idtipo_doc
                                    LEFT JOIN sind_patr sindp ON acct.sindicato_patronal_id = sindp.id_sindp
                                    LEFT JOIN sind_emp sindemp ON acct.sindicato_laboral_id = sindemp.id_sinde
                                    LEFT JOIN classe_cnae ccnae ON JSON_CONTAINS(acct.cnaes_ids, json_array(cast(ccnae.id_cnae as char)))
                                ORDER BY
                                    acct.proxima_ligacao DESC,
                                    acct.status;");

            migrationBuilder.Sql(@"DROP VIEW if exists acompanhamento_cct_vw;");

            migrationBuilder.Sql(@"CREATE OR REPLACE
                                ALGORITHM = UNDEFINED VIEW `acompanhamento_cct_vw` AS
                                select
                                    `cct`.`idacompanhanto_cct` AS `id`,
                                    `sempt`.`codigo_sinde` AS `codigo_sindicato_laboral`,
                                    `sempt`.`sigla_sinde` AS `sigla_sindicato_laboral`,
                                    `sempt`.`cnpj_sinde` AS `cnpj_sindicato_laboral`,
                                    `spt`.`codigo_sp` AS `codigo_sindicato_patronal`,
                                    `spt`.`sigla_sp` AS `sigla_sindicato_patronal`,
                                    `spt`.`cnpj_sp` AS `cnpj_sindicato_patronal`,
                                    `spt`.`id_sindp` AS `id_sindicato_patronal`,
                                    `spt`.`uf_sp` AS `uf_sindicato_patronal`,
                                    `sempt`.`id_sinde` AS `id_sindicato_laboral`,
                                    `sempt`.`uf_sinde` AS `uf_sindicato_laboral`,
                                    `tdt`.`nome_doc` AS `nome_documento`,
                                    `cct`.`data_base` AS `data_base`,
                                    `cct`.`fase` AS `fase`,
                                    `cct`.`observacoes_gerais` AS `observacoes_gerais`,
                                    date_format(`cct`.`ultima_atualizacao`, '%d/%m/%Y') AS `ultima_atualizacao`,
                                    `ConvertMonthYear_ENtoPT`(`ir`.`periodo_data`) AS `periodo_anterior`,
                                    `ir`.`indicador` AS `indicador`,
                                    `ir`.`dado_real` AS `dado_real`,
                                    (convert(date_format((`ir`.`periodo_data` + interval 1 month), '%b/%Y')
                                        using utf8mb4) collate utf8mb4_0900_ai_ci) AS `ir_periodo`,
                                    `cnaet`.`atividades_economicas` AS `atividades_economicas`,
                                    `cct`.`ids_cnaes` AS `ids_cnaes`
                                from
                                    (((((`acompanhanto_cct` `cct`
                                left join `sind_emp` `sempt` on
                                    ((`cct`.`sind_emp_id_sinde` = `sempt`.`id_sinde`)))
                                left join `sind_patr` `spt` on
                                    ((`cct`.`sind_patr_id_sindp` = `spt`.`id_sindp`)))
                                left join `tipo_doc` `tdt` on
                                    ((`cct`.`tipo_doc_idtipo_doc` = `tdt`.`idtipo_doc`)))
                                left join `indecon_real` `ir` on
                                    (((`ir`.`periodo_data` is not null)
                                        and (`cct`.`data_base` = `ConvertMonthYear_ENtoPT`(date_format((`ir`.`periodo_data` + interval 1 month), '%b/%Y')))
                                            and (`ir`.`indicador` = 'INPC'))))
                                left join lateral (
                                    select
                                        group_concat(`cnaet`.`descricao_subclasse` separator ', ') AS `atividades_economicas`
                                    from
                                        `classe_cnae` `cnaet`
                                    where
                                        json_contains(`cct`.`ids_cnaes`,
                                        concat('[""', cast(`cnaet`.`id_cnae` as char charset utf8mb4), '""]'))) `cnaet` on
                                    (true));");

            migrationBuilder.Sql(@"DROP VIEW IF EXIST acompanhamento_cct_relatorio_vw;");

            migrationBuilder.Sql(@"CREATE OR REPLACE VIEW acompanhamento_cct_relatorio_vw AS
                    select
                        cct.idacompanhanto_cct AS id,
                        sempt.id_sinde AS id_sindicato_laboral,
                        sempt.sigla_sinde AS sigla_sindicato_laboral,
                        sempt.cnpj_sinde AS cnpj_sindicato_laboral,
                        sempt.codigo_sinde AS codigo_sindicato_laboral,
                        sempt.uf_sinde AS uf_sindicato_laboral,
                        spt.id_sindp AS id_sindicato_patronal,
                        spt.sigla_sp AS sigla_sindicato_patronal,
                        spt.cnpj_sp AS cnpj_sindicato_patronal,
                        spt.codigo_sp AS codigo_sindicato_patronal,
                        spt.uf_sp AS uf_sindicato_patronal,
                        tdt.nome_doc AS nome_documento,
                        cct.data_base AS data_base,
                        cct.fase AS fase,
                        cct.observacoes_gerais AS observacoes_gerais,
                        date_format(cct.ultima_atualizacao, '%d/%m/%Y') AS ultima_atualizacao,
                        ConvertMonthYear_ENtoPT(ir.periodo_data) AS periodo_anterior,
                        ir.indicador AS indicador,
                        ir.dado_real AS dado_real,
                        cct.ids_cnaes AS ids_cnaes,
                        date_format((ir.periodo_data + interval 1 month), '%b/%Y') AS ir_periodo,
                        cnaet.atividades_economicas AS atividades_economicas,
                        acet.estabelecimentos AS estabelecimentos,
                        sind_ads_laborais.sindicatos AS sindicatos_laborais_adicionais,
                        sind_ads_patronais.sindicatos AS sindicatos_patronais_adicionais
                    from acompanhanto_cct cct
                    left join sind_emp sempt on cct.sind_emp_id_sinde = sempt.id_sinde
                    left join sind_patr spt on cct.sind_patr_id_sindp = spt.id_sindp
                    left join tipo_doc tdt on cct.tipo_doc_idtipo_doc = tdt.idtipo_doc
                    left join indecon_real ir on ir.periodo_data is not null
                        and cct.data_base = ConvertMonthYear_ENtoPT(date_format((ir.periodo_data + interval 1 month), '%b/%Y'))
	                    and ir.indicador = 'INPC'
                    left join lateral (
                        select group_concat(cnaet.descricao_subclasse separator ', ') AS atividades_economicas from classe_cnae cnaet
                        where json_contains(cct.ids_cnaes, concat('[""', cast(cnaet.id_cnae as char charset utf8mb4), '""]'))
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
	                    where acet.acompanhamento_cct_id = cct.idacompanhanto_cct
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
                        from sind_emp se
                        join acompanhamento_cct_sindicato_laboral_tb acspt on acspt.acompanhamento_cct_id = cct.idacompanhanto_cct
                            and acspt.sindicato_id = se.id_sinde
		                    and sempt.id_sinde <> acspt.sindicato_id
                    ) sind_ads_laborais on true
                    left join lateral (
                        select json_arrayagg(
                        json_object('sigla',
                            sp.sigla_sp,
                            'cnpj',
                            sp.cnpj_sp,
                            'uf',
                            sp.uf_sp)
	                    ) AS sindicatos
                        from sind_patr sp
                        join acompanhamento_cct_sindicato_patronal_tb acspt on acspt.acompanhamento_cct_id = cct.idacompanhanto_cct
                            and acspt.sindicato_id = sp.id_sindp
                            and spt.id_sindp <> acspt.sindicato_id
                    ) sind_ads_patronais on true;
");

            migrationBuilder.Sql(@"DROP PROCEDURE inserir_estabelecimentos_acompanhamento;");

            migrationBuilder.Sql(@"CREATE PROCEDURE inserir_estabelecimentos_sindicatos_acompanhamento()
                BEGIN
                    truncate acompanhamento_cct_estabelecimento_tb;
                    truncate acompanhamento_cct_sindicato_laboral_tb;
                    truncate acompanhamento_cct_sindicato_patronal_tb;

   	                -- Carga Estabelecimentos Sindicatos Laborais Simples
                    insert into acompanhamento_cct_estabelecimento_tb(acompanhamento_cct_id, grupo_economico_id, empresa_id, estabelecimento_id)
                    select distinct cct.idacompanhanto_cct acompanhamento_cct_id, cut.cliente_grupo_id_grupo_economico grupo_economico_id, cut.cliente_matriz_id_empresa empresa_id, cut.id_unidade estabelecimento_id 
                    from acompanhanto_cct cct 
                    inner join sind_emp sempt on cct.sind_emp_id_sinde = sempt.id_sinde
                    inner join base_territorialsindemp btset on sempt.id_sinde = btset.sind_empregados_id_sinde1
                    inner join localizacao lt on btset.localizacao_id_localizacao1 = lt.id_localizacao
                    inner join cliente_unidades cut on cut.localizacao_id_localizacao = lt.id_localizacao 
				                                                and json_contains(cut.cnae_unidade, concat('{""id"":', btset.classe_cnae_idclasse_cnae, '}'))
                    and not exists(select 1 from acompanhamento_cct_estabelecimento_tb acet
                                   where cct.idacompanhanto_cct = acet.acompanhamento_cct_id
                                   and cut.cliente_grupo_id_grupo_economico = acet.grupo_economico_id
                                   and cut.cliente_matriz_id_empresa = acet.empresa_id
                                   and cut.id_unidade = acet.estabelecimento_id);

	                -- Carga Estabelecimentos Sindicatos Laborais Adicionais
	                insert into acompanhamento_cct_estabelecimento_tb(acompanhamento_cct_id, grupo_economico_id, empresa_id, estabelecimento_id)
                    select distinct cct.idacompanhanto_cct acompanhamento_cct_id, cut.cliente_grupo_id_grupo_economico grupo_economico_id, cut.cliente_matriz_id_empresa empresa_id, cut.id_unidade estabelecimento_id 
                    from acompanhanto_cct cct 
                    inner join sind_emp sempt on cct.sind_emp_id_sinde = json_contains(cct.ids_sindemp_adicionais, concat('[""', sempt.id_sinde, '""]'))
                    inner join base_territorialsindemp btset on sempt.id_sinde = btset.sind_empregados_id_sinde1
                    inner join localizacao lt on btset.localizacao_id_localizacao1 = lt.id_localizacao
                    inner join cliente_unidades cut on cut.localizacao_id_localizacao = lt.id_localizacao 
				                                                and json_contains(cut.cnae_unidade, concat('{""id"":', btset.classe_cnae_idclasse_cnae, '}'))
                    and not exists(select 1 from acompanhamento_cct_estabelecimento_tb acet
                                   where cct.idacompanhanto_cct = acet.acompanhamento_cct_id
                                   and cut.cliente_grupo_id_grupo_economico = acet.grupo_economico_id
                                   and cut.cliente_matriz_id_empresa = acet.empresa_id
                                   and cut.id_unidade = acet.estabelecimento_id);
			
	                -- Carga Estabelecimentos Sindicatos Patronais Simples
                    insert into acompanhamento_cct_estabelecimento_tb(acompanhamento_cct_id, grupo_economico_id, empresa_id, estabelecimento_id)
                    select distinct cct.idacompanhanto_cct acompanhamento_cct_id, cut.cliente_grupo_id_grupo_economico grupo_economico_id, cut.cliente_matriz_id_empresa empresa_id, cut.id_unidade estabelecimento_id 
                    from acompanhanto_cct cct 
                    inner join sind_patr sptr on cct.sind_patr_id_sindp = sptr.id_sindp
                    inner join base_territorialsindpatro btspt on sptr.id_sindp = btspt.sind_patronal_id_sindp
                    inner join localizacao lt on btspt.localizacao_id_localizacao1 = lt.id_localizacao
                    inner join cliente_unidades cut on cut.localizacao_id_localizacao = lt.id_localizacao 
												                and json_contains(cut.cnae_unidade, concat('{""id"":', btspt.classe_cnae_idclasse_cnae, '}'))
                    and not exists(select 1 from acompanhamento_cct_estabelecimento_tb acet
                                   where cct.idacompanhanto_cct = acet.acompanhamento_cct_id
                                   and cut.cliente_grupo_id_grupo_economico = acet.grupo_economico_id
                                   and cut.cliente_matriz_id_empresa = acet.empresa_id
                                   and cut.id_unidade = acet.estabelecimento_id);
	
	                -- Carga Estabelecimentos Sindicatos Patronais Adicionais
                    insert into acompanhamento_cct_estabelecimento_tb(acompanhamento_cct_id, grupo_economico_id, empresa_id, estabelecimento_id)
                    select distinct cct.idacompanhanto_cct acompanhamento_cct_id, cut.cliente_grupo_id_grupo_economico grupo_economico_id, cut.cliente_matriz_id_empresa empresa_id, cut.id_unidade estabelecimento_id 
                    from acompanhanto_cct cct 
                    inner join sind_patr sptr on cct.sind_patr_id_sindp = json_contains(cct.ids_sindpatr_adicionais, concat('[""', sptr.id_sindp, '""]'))
                    inner join base_territorialsindpatro btspt on sptr.id_sindp = btspt.sind_patronal_id_sindp
                    inner join localizacao lt on btspt.localizacao_id_localizacao1 = lt.id_localizacao
                    inner join cliente_unidades cut on cut.localizacao_id_localizacao = lt.id_localizacao 
												                and json_contains(cut.cnae_unidade, concat('{""id"":', btspt.classe_cnae_idclasse_cnae, '}'))
                    and not exists(select 1 from acompanhamento_cct_estabelecimento_tb acet
                                   where cct.idacompanhanto_cct = acet.acompanhamento_cct_id
                                   and cut.cliente_grupo_id_grupo_economico = acet.grupo_economico_id
                                   and cut.cliente_matriz_id_empresa = acet.empresa_id
                                   and cut.id_unidade = acet.estabelecimento_id);

	                -- Carga Estabelecimentos Grupo Economico
                    insert into acompanhamento_cct_estabelecimento_tb(acompanhamento_cct_id, grupo_economico_id, empresa_id, estabelecimento_id)
                    select distinct cct.idacompanhanto_cct acompanhamento_cct_id, cut.cliente_grupo_id_grupo_economico grupo_economico_id, cut.cliente_matriz_id_empresa empresa_id, cut.id_unidade estabelecimento_id 
                    from acompanhanto_cct cct 
                    inner join cliente_unidades cut on JSON_CONTAINS(cct.grupos_economicos_ids, concat('[', cut.cliente_grupo_id_grupo_economico, ']'))
                    where not exists(select 1 from acompanhamento_cct_estabelecimento_tb acet
                                   where cct.idacompanhanto_cct = acet.acompanhamento_cct_id
                                   and cut.cliente_grupo_id_grupo_economico = acet.grupo_economico_id
                                   and cut.cliente_matriz_id_empresa = acet.empresa_id
                                   and cut.id_unidade = acet.estabelecimento_id);
  
	                -- Carga Estabelecimentos Matriz
                    insert into acompanhamento_cct_estabelecimento_tb(acompanhamento_cct_id, grupo_economico_id, empresa_id, estabelecimento_id)
                    select distinct cct.idacompanhanto_cct acompanhamento_cct_id, cut.cliente_grupo_id_grupo_economico grupo_economico_id, cut.cliente_matriz_id_empresa empresa_id, cut.id_unidade estabelecimento_id 
                    from acompanhanto_cct cct 
                    inner join cliente_unidades cut on JSON_CONTAINS(cct.empresas_ids, concat('[', cut.cliente_matriz_id_empresa, ']'))
                    where not exists(select 1 from acompanhamento_cct_estabelecimento_tb acet
                                   where cct.idacompanhanto_cct = acet.acompanhamento_cct_id
                                   and cut.cliente_grupo_id_grupo_economico = acet.grupo_economico_id
                                   and cut.cliente_matriz_id_empresa = acet.empresa_id
                                   and cut.id_unidade = acet.estabelecimento_id);	

	                -- Carga Sindicatos Laborais
                    insert into acompanhamento_cct_sindicato_laboral_tb(acompanhamento_cct_id, sindicato_id)
                    select distinct cct.idacompanhanto_cct acompanhamento_cct_id, sptr.id_sinde sindicato_id
                    from acompanhanto_cct cct 
                    inner join sind_emp sptr on cct.sind_emp_id_sinde = sptr.id_sinde or json_contains(cct.ids_sindemp_adicionais, concat('[""', sptr.id_sinde, '""]'));

	                -- Carga Sindicatos Patronais
                    insert into acompanhamento_cct_sindicato_patronal_tb(acompanhamento_cct_id, sindicato_id)
                    select distinct cct.idacompanhanto_cct acompanhamento_cct_id, sptr.id_sindp sindicato_id
                    from acompanhanto_cct cct 
                    inner join sind_patr sptr on cct.sind_patr_id_sindp = sptr.id_sindp or json_contains(cct.ids_sindpatr_adicionais, concat('[""', sptr.id_sindp, '""]'));	  
                END;");
        }
    }
}
