using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ineditta.Repository.Migrations
{
    /// <inheritdoc />
    public partial class v89 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"drop view if exists documento_sindicato_estabelecimentos_vw;");
        }
    }
}
