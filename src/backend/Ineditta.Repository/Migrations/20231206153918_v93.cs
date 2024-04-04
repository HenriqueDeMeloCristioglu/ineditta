using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ineditta.Repository.Migrations
{
    /// <inheritdoc />
    public partial class v93 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE OR REPLACE
                ALGORITHM = UNDEFINED VIEW `documento_sindicato_estabelecimentos_vw` AS
                select
                    dct.id_doc AS documento_id,
                    gt.grupo_economico_id AS grupo_economico_id,
                    cgt.nome_grupoeconomico AS grupo_economico_nome,
                    gt.matriz_id AS matriz_id,
                    cmt.nome_empresa AS matriz_nome,
                    gt.estabelecimento_id AS estabelecimento_id,
                    cut.nome_unidade AS estabelecimento_nome,
                    cut.cod_sindcliente AS estabelecimento_codigo_sindicato_laboral,
                    cut.cod_sindpatrocliente AS estabelecimento_codigo_sindicato_patronal,
                    dct.titulo_documento AS documento_titulo,
                    tdt.nome_doc AS documento_nome,
                    dct.versao_documento AS documento_versao,
                    dct.database_doc AS documento_database,
                    dct.validade_inicial AS documento_vigencia_inicial,
                    dct.validade_final AS documento_vigencia_final,
                    cgt.logo_grupo AS grupo_economico_logo_url,
                    cut.codigo_unidade AS estabelecimento_codigo,
                    dct.data_assinatura AS documento_data_assinatura,
                    dct.caminho_arquivo AS documento_caminho_arquivo,
                    dct.data_reg_mte AS documento_data_registro_mte,
                    sempt.sindicatos AS estabelecimento_sindicatos_laborais,
                    spt.sindicatos AS estabelecimento_sindicatos_patronais,
                    dctspt.sindicatos as documento_sindicatos_patronais,
                    dctset.sindicatos as documento_sindicatos_laborais
                from
                    doc_sind dct
                join tipo_doc tdt on
                    dct.tipo_doc_idtipo_doc = tdt.idtipo_doc
                join json_table(dct.cliente_estabelecimento,
                    '$[*]' columns (grupo_economico_id int path '$.g',
                    matriz_id int path '$.m',
                    estabelecimento_id int path '$.u')) gt
                join cliente_unidades cut on
                    gt.estabelecimento_id = cut.id_unidade
                join cliente_grupo cgt on
                    gt.grupo_economico_id = cgt.id_grupo_economico
                join cliente_matriz cmt on
                    gt.matriz_id = cmt.id_empresa
                left join lateral (
                    select
                        json_arrayagg(json_object('id',
                        sempt.id_sinde,
                        'sigla',
                        sempt.sigla_sinde,
                        'cnpj',
                        sempt.cnpj_sinde,
                        'codigo',
                        sempt.codigo_sinde,
                        'razaoSocial',
                        sempt.razaosocial_sinde)) AS sindicatos
                    from
                        sind_emp sempt
                    where
                        exists(
                        select
                            1
                        from
                            (localizacao lt
                        join base_territorialsindemp btset on
                            (((lt.id_localizacao = btset.localizacao_id_localizacao1)
                                and json_contains(cut.cnae_unidade,
                                concat('{""id"":', btset.classe_cnae_idclasse_cnae, '}'))
                                    and (sempt.id_sinde = btset.sind_empregados_id_sinde1))))
                        where
                            json_contains(dct.sind_laboral,
                            concat('{""id"":', cast(sempt.id_sinde as char charset utf8mb4), '}'))
                                and cut.localizacao_id_localizacao = lt.id_localizacao)) sempt on
                    true
                left join lateral (
                    select
                        json_arrayagg(json_object('id',
                        spt.id_sindp,
                        'sigla',
                        spt.sigla_sp,
                        'cnpj',
                        spt.cnpj_sp,
                        'codigo',
                        spt.codigo_sp,
                        'razaoSocial',
                        spt.razaosocial_sp)) AS sindicatos
                    from
                        sind_patr spt
                    where
                        exists(
                        select
                            1
                        from
                            localizacao lt
                        join base_territorialsindpatro btspt on
                            lt.id_localizacao = btspt.localizacao_id_localizacao1
                                and json_contains(cut.cnae_unidade,
                                concat('{""id"":', btspt.classe_cnae_idclasse_cnae, '}'))
                                    and spt.id_sindp = btspt.sind_patronal_id_sindp
                        where
                            json_contains(dct.sind_patronal,
                            concat('{""id"":', cast(spt.id_sindp as char charset utf8mb4), '}'))
                                and cut.localizacao_id_localizacao = lt.id_localizacao)) spt on
                    (true)
                left join lateral (
                 select 
                   json_arrayagg(json_object('id',
                        dctspt.id_sindp,
                        'sigla',
                        dctspt.sigla_sp,
                        'cnpj',
                        dctspt.cnpj_sp,
                        'codigo',
                        dctspt.codigo_sp,
                        'razaoSocial',
                        dctspt.razaosocial_sp)) AS sindicatos
                 from sind_patr dctspt,
 	                JSON_TABLE(dct.sind_patronal, '$[*].id' columns ( id int path '$')) gt
 	                where dctspt.id_sindp = gt.id
                ) dctspt on true
                left join lateral (
                 select 
                   json_arrayagg(json_object('id',
                        dctset.id_sinde,
                        'sigla',
                        dctset.sigla_sinde,
                        'cnpj',
                        dctset.cnpj_sinde,
                        'codigo',
                        dctset.codigo_sinde,
                        'razaoSocial',
                        dctset.razaosocial_sinde)) AS sindicatos
                 from sind_emp dctset,
 	                JSON_TABLE(dct.sind_laboral , '$[*].id' columns ( id int path '$')) gt
 	                where dctset.id_sinde = gt.id
                ) dctset on true");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"create or replace view documento_sindicato_estabelecimentos_vw as
                                    select dct.id_doc documento_id, gt.grupo_economico_id, cgt.nome_grupoeconomico grupo_economico_nome, gt.matriz_id, cmt.nome_empresa matriz_nome, gt.estabelecimento_id, 
                                    cut.nome_unidade estabelecimento_nome, cut.cod_sindcliente estabelecimento_codigo_sindicato_laboral,
                                    cut.cod_sindpatrocliente estabelecimento_codigo_sindicato_patronal, dct.titulo_documento documento_titulo, tdt.nome_doc documento_nome,
                                    dct.versao_documento documento_versao, dct.database_doc documento_database, dct.validade_inicial documento_vigencia_inicial, dct.validade_final
                                    documento_vigencia_final, cgt.logo_grupo grupo_economico_logo_url, cut.codigo_unidade estabelecimento_codigo,
                                    dct.data_assinatura documento_data_assinatura, dct.caminho_arquivo documento_caminho_arquivo, dct.data_reg_mte documento_data_registro_mte,
                                    dct.sind_laboral documento_sindicatos_laborais,
                                    sempt.sindicatos as estabelecimento_sindicatos_laborais, 
                                    dct.sind_patronal documento_sindicatos_patronais,
                                    spt.sindicatos as estabelecimento_sindicatos_patronais
                                    from doc_sind dct
                                    inner join tipo_doc tdt on dct.tipo_doc_idtipo_doc = tdt.idtipo_doc,
                                    json_table(dct.cliente_estabelecimento,  '$[*]' columns (
                                            grupo_economico_id INT PATH '$.g',
                                            matriz_id int path '$.m',
                                            estabelecimento_id int path '$.u'
                                           )) as gt
                                    join cliente_unidades cut on gt.estabelecimento_id = cut.id_unidade 
                                    join cliente_grupo cgt on gt.grupo_economico_id = cgt.id_grupo_economico 
                                    join cliente_matriz cmt on gt.matriz_id = cmt.id_empresa 
                                    left join lateral (select JSON_ARRAYAGG(
	                                    JSON_OBJECT(
		                                    'id', sempt.id_sinde, 
		                                    'sigla', sempt.sigla_sinde, 
		                                    'cnpj', sempt.cnpj_sinde,
		                                    'codigo', sempt.codigo_sinde, 
		                                    'razaoSocial', sempt.razaosocial_sinde)) as sindicatos
		                                    from sind_emp sempt 
                                            where exists (select 1 from localizacao lt 
                                                 inner join base_territorialsindemp btset on lt.id_localizacao = btset.localizacao_id_localizacao1 AND JSON_CONTAINS(cut.cnae_unidade, CONCAT('{""id"":', btset.classe_cnae_idclasse_cnae,'}'))
                                                 and sempt.id_sinde = btset.sind_empregados_id_sinde1
                                                 where JSON_CONTAINS(dct.sind_laboral, CONCAT('{""id"":', CAST(sempt.id_sinde as char), '}'))
                                                 and cut.localizacao_id_localizacao = lt.id_localizacao
                                                 )
	                                    ) sempt on true
                                    left join lateral (select JSON_ARRAYAGG(
					                                    JSON_OBJECT(
						                                    'id', spt.id_sindp, 
						                                    'sigla', spt.sigla_sp, 
						                                    'cnpj', spt.cnpj_sp ,
						                                    'codigo', spt.codigo_sp , 
						                                    'razaoSocial', spt.razaosocial_sp)) as sindicatos
						                                    from sind_patr spt 
				                                            where exists(select 1 from localizacao lt
				                                                         inner join base_territorialsindpatro btspt on lt.id_localizacao = btspt.localizacao_id_localizacao1 AND JSON_CONTAINS(cut.cnae_unidade, CONCAT('{""id"":', btspt.classe_cnae_idclasse_cnae, '}'))
				                                                         and spt.id_sindp = btspt.sind_patronal_id_sindp
				                                                         where JSON_CONTAINS(dct.sind_patronal , CONCAT('{""id"":', CAST(spt.id_sindp as char), '}'))
				                                                         and cut.localizacao_id_localizacao = lt.id_localizacao
						                                    )       
					                                    ) spt on true;");
        }
    }
}
