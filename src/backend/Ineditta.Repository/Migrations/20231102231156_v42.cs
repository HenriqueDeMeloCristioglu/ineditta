using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ineditta.Repository.Migrations
{
    /// <inheritdoc />
    public partial class v42 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "exibe_comparativo_mapa_sindical",
                table: "estrutura_clausulas_ad_tipoinformacaoadicional");

            migrationBuilder.AddColumn<sbyte>(
                name: "exibe_comparativo_mapa_sindical",
                table: "informacao_adicional_grupo",
                type: "TINYINT(0)",
                nullable: false,
                defaultValue: (sbyte)0);

            migrationBuilder.Sql(@"drop view if exists comparativo_mapa_sindical_principal_vw;");

            migrationBuilder.Sql(@"drop view if exists comparativo_mapa_sindical_item_vw;");

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

            migrationBuilder.Sql(@"CREATE OR REPLACE VIEW comparativo_mapa_sindical_item_vw AS
                                select
                                    cgt.id_clau AS clausula_id,
                                    cgect.id_clausulageral_estrutura_clausula AS clausula_geral_estrutura_id,
                                    cgt.doc_sind_id_documento AS documento_sindical_id,
                                    gct.idgrupo_clausula AS grupo_clausula_id,
                                    gct.nome_grupo AS grupo_clausula_nome,
                                    tiat.cdtipoinformacaoadicional AS informacao_adicional_id,
                                    tiat.nmtipoinformacaoadicional AS informacao_adicional_nome,
                                    cgect.data AS valor_data,
                                    cgect.numerico AS valor_numerico,
                                    cgect.texto AS valor_texto,
                                    cgect.percentual AS valor_percentual,
                                    cgect.descricao AS valor_descricao,
                                    cgect.hora AS valor_hora,
                                    cgect.combo AS valor_combo,
                                    cgect.sequencia AS sequencia,
                                    cgect.grupo_dados AS grupo_dados,
                                    dsindt.titulo_documento AS documento_titulo,
                                    dsindt.data_aprovacao AS documento_data_aprovacao,
                                    dsindt.sind_patronal AS documento_sindicatos_patronais,
                                    dsindt.sind_laboral AS documento_sindicatos_laborais,
                                    dsindt.cnae_doc AS documento_atividades_economicas,
                                    dsindt.uf AS documento_uf,
                                    dsindt.validade_inicial AS documento_validade_inicial,
                                    dsindt.validade_final AS documento_validade_final,
                                    dsindt.database_doc AS documento_database,
                                    ect.tipo_clausula AS estrutura_clausula_tipo,
                                    tiat.idtipodado AS informacao_adicional_tipo_dado,
                                    cgect.id_info_tipo_grupo informacao_adicional_grupo_id,
                                    cgect.nome_informacao clausula_grupo_informacao_adicional_nome,
                                    (case
                                        when (cgt.aprovador = 'sim') then true
                                        else false
                                    end) AS clausula_aprovada,
                                    cgt.data_aprovacao AS clausula_data_aprovacao,
                                    cgt.tex_clau AS clausula_texto,
                                    (case
                                        when (cgt.liberado = 'S') then true
                                        else false
                                    end) AS clausula_liberada,
                                    td.nome_doc AS tipo_documento_nome,
                                    dsindt.abrangencia AS documento_abrangencia,
                                    coalesce(iagt.exibe_comparativo_mapa_sindical, 0) exibe_comparativo_mapa_sindical
                                from
                                    clausula_geral cgt
                                join clausula_geral_estrutura_clausula cgect on
                                    cgt.id_clau = cgect.clausula_geral_id_clau
                                join estrutura_clausula ect on
                                    cgect.estrutura_clausula_id_estruturaclausula = ect.id_estruturaclausula
                                join grupo_clausula gct on
                                    ect.grupo_clausula_idgrupo_clausula = gct.idgrupo_clausula
                                join ad_tipoinformacaoadicional tiat on
                                    cgect.ad_tipoinformacaoadicional_cdtipoinformacaoadicional = tiat.cdtipoinformacaoadicional
                                join doc_sind dsindt on cgt.doc_sind_id_documento = dsindt.id_doc
                                join tipo_doc td on dsindt.tipo_doc_idtipo_doc = td.idtipo_doc
                                left join informacao_adicional_grupo iagt 
   	                                on cgect.id_info_tipo_grupo = iagt.ad_tipoinformacaoadicional_id 
   	                                and cgect.ad_tipoinformacaoadicional_cdtipoinformacaoadicional = iagt.informacaoadicional_no_grupo;");

            migrationBuilder.Sql(@"create index ix001_clausula_geral_estrutura_clausula on clausula_geral_estrutura_clausula (doc_sind_id_doc, id_clausulageral_estrutura_clausula, id_info_tipo_grupo, ad_tipoinformacaoadicional_cdtipoinformacaoadicional);");

            migrationBuilder.Sql("create index ix001_informacao_adicional_grupo on informacao_adicional_grupo (ad_tipoinformacaoadicional_id, informacaoadicional_no_grupo);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "exibe_comparativo_mapa_sindical",
                table: "informacao_adicional_grupo");

            migrationBuilder.AddColumn<sbyte>(
                name: "exibe_comparativo_mapa_sindical",
                table: "estrutura_clausulas_ad_tipoinformacaoadicional",
                type: "TINYINT(0)",
                nullable: false,
                defaultValue: (sbyte)0);

            migrationBuilder.Sql(@"drop view if exists comparativo_mapa_sindical_principal_vw;");

            migrationBuilder.Sql(@"drop view if exists comparativo_mapa_sindical_item_vw;");
            
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
		                                  JSON_LENGTH(dct.cliente_estabelecimento) quantidade_estabelecimentos 
   		                                from doc_sind as dct 
   		                                inner join tipo_doc tdt on dct.tipo_doc_idtipo_doc = tdt.idtipo_doc 
                                   left join lateral(select idct.id_indecon id,
   						                                idct.`data`, 
   						                                idct.dado_projetado  
   				                                     from indecon idct 
				                                     where idct.data = CONCAT(DATE_FORMAT((dct.validade_inicial - INTERVAL 1 MONTH), '%Y-%m'), '-01')
		                                     limit 1) inpc on true;");

            migrationBuilder.Sql(@"drop index ix001_clausula_geral_estrutura_clausula on clausula_geral_estrutura_clausula;");

            migrationBuilder.Sql(@"drop index ix001_informacao_adicional_grupo on informacao_adicional_grupo;");
        }
    }
}
