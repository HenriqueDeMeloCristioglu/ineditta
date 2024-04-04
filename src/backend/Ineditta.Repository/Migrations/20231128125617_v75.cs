using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ineditta.Repository.Migrations
{
    /// <inheritdoc />
    public partial class v75 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"drop view if exists comparativo_mapa_sindical_principal_vw;");

            migrationBuilder.Sql(@"CREATE OR REPLACE VIEW comparativo_mapa_sindical_principal_vw AS
                                select
                                    dct.id_doc AS documento_id,
                                    dct.sind_patronal AS sindicatos_patronais,
                                    dct.sind_laboral AS sindicatos_laborais,
                                    dct.cnae_doc AS cnaes,
                                    dct.cliente_estabelecimento AS estabelecimentos,
                                    dct.abrangencia AS abrangencia,
                                    dct.validade_inicial AS validade_inicial,
                                    dct.validade_final AS validade_final,
                                    dct.database_doc AS data_base,
                                    dct.uf AS uf,
                                    dct.data_aprovacao AS data_aprovacao,
                                    coalesce(inpc.dado_real, 0) AS indice_projetado,
                                    inpc.id AS inpc_id,
                                    tdt.nome_doc AS documento_nome,
                                    tdt.idtipo_doc AS tipo_documento_id,
                                    json_length(dct.cliente_estabelecimento) AS quantidade_estabelecimentos,
                                    coalesce(cgect.quantidade, 0) AS quantidade_clausulas_comparativo,
                                    dct.data_upload AS data_upload,
                                    dct.descricao_documento AS descricao_documento
                                from
                                    (((doc_sind dct
                                join tipo_doc tdt on
                                    ((dct.tipo_doc_idtipo_doc = tdt.idtipo_doc)))
                                left join lateral (
                                    select
                                        idct.id AS id,
                                        idct.periodo_data AS periodo_data,
                                        idct.dado_real AS dado_real
                                    from
                                        indecon_real idct
                                    where
                                        ((idct.periodo_data = concat(date_format((dct.validade_inicial - interval 1 month), '%Y-%m'), '-01'))
                                            and (idct.indicador = 'INPC'))
                                    limit 1) inpc on
                                    (true))
                                left join lateral (
                                    select
                                        count(1) AS quantidade
                                    from comparativo_mapa_sindical_item_vw cmsiv where dct.id_doc = cmsiv.documento_sindical_id and cmsiv.exibe_comparativo_mapa_sindical = 1) cgect on
                                    (true));");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"drop view if exists comparativo_mapa_sindical_principal_vw;");

            migrationBuilder.Sql(@"create or replace view comparativo_mapa_sindical_principal_vw as
                                   select dct.id_doc documento_id,
		                                  dct.sind_patronal sindicatos_patronais,
		                                  dct.sind_laboral sindicatos_laborais,
		                                  dct.cnae_doc cnaes,
		                                  dct.cliente_estabelecimento estabelecimentos,
		                                  dct.abrangencia abrangencia,
		                                  dct.validade_inicial validade_inicial,
		                                  dct.validade_final validade_final,
		                                  dct.database_doc data_base,
		                                  dct.uf uf,
		                                  dct.data_aprovacao data_aprovacao,
		                                  coalesce(inpc.dado_projetado, 0) indice_projetado,
		                                  inpc.id inpc_id,
		                                  tdt.nome_doc documento_nome,
		                                  tdt.idtipo_doc tipo_documento_id, 
		                                  JSON_LENGTH(dct.cliente_estabelecimento) quantidade_estabelecimentos,
		                                  coalesce(cgect.quantidade, 0) quantidade_clausulas_comparativo,
		                                  dct.data_upload,
		                                  dct.descricao_documento
   		                                from doc_sind as dct 
   		                                inner join tipo_doc tdt on dct.tipo_doc_idtipo_doc = tdt.idtipo_doc 
   		                                left join lateral(select idct.id_indecon id,
   						                                idct.`data`, 
   						                                idct.dado_projetado  
   				                                     from indecon idct 
				                                     where idct.data = CONCAT(DATE_FORMAT((dct.validade_inicial - INTERVAL 1 MONTH), '%Y-%m'), '-01')
		                                     limit 1) inpc on true
		                                left join lateral (select count(1) quantidade 
						                                   from clausula_geral_estrutura_clausula cgect 
						                                   inner join clausula_geral cgt on cgect.id_clausulageral_estrutura_clausula = cgt.id_clau 
						                                   inner join informacao_adicional_grupo iagt 
						   	                                on cgect.id_info_tipo_grupo = iagt.ad_tipoinformacaoadicional_id 
						   	                                and cgect.ad_tipoinformacaoadicional_cdtipoinformacaoadicional = iagt.informacaoadicional_no_grupo 
						   	                                and iagt.exibe_comparativo_mapa_sindical = 1
						   	                                and cgect.ad_tipoinformacaoadicional_cdtipoinformacaoadicional <> 170
						   	                                and cgt.liberado = 'S'
						   	                                where dct.id_doc = cgect.doc_sind_id_doc) cgect on true;");
        }
    }
}
