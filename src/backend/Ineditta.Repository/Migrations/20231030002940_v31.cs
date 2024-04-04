using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ineditta.Repository.Migrations
{
    /// <inheritdoc />
    public partial class v31 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"drop view if exists mapa_sindical_vw;");

            migrationBuilder.Sql(@"CREATE OR REPLACE
                                VIEW mapa_sindical_vw AS
	                                select
	                                cgt.id_clau AS clausula_id,
	                                cgt.tex_clau AS clausula_texto,
	                                gct.idgrupo_clausula AS grupo_clausula_id,
	                                gct.nome_grupo AS grupo_clausula_nome,
	                                ect.id_estruturaclausula AS estrutura_clausula_id,
	                                ect.nome_clausula AS estrutura_clausula_nome,
	                                dct.id_doc AS documento_id,
	                                dct.sind_patronal AS documento_sindicato_patronal,
	                                dct.sind_laboral AS documento_sindicato_laboral,
	                                dct.data_reg_mte AS data_registro,
	                                dct.data_aprovacao AS documento_data_aprovacao,
	                                dct.database_doc AS documento_database,
	                                dct.tipo_doc_idtipo_doc AS documento_tipo_id,
	                                dct.cnae_doc AS documento_cnae,
	                                dct.abrangencia AS documento_abrangencia,
	                                dct.cliente_estabelecimento AS documento_estabelecimento,
	                                dct.validade_inicial AS documento_validade_inicial,
	                                dct.validade_final AS documento_validade_final,
	                                COALESCE(cgt.liberado, 'N') AS clausula_geral_liberada,
	                                dct.uf AS documento_uf,
	                                dct.descricao_documento AS documento_descricao,
	                                dct.titulo_documento AS documento_titulo,
	                                JSON_LENGTH(dct.sind_patronal) AS quantidade_sindicatos_patronais,
	                                JSON_LENGTH(dct.sind_laboral) AS quantidade_sindicatos_laborais,
	                                cgt.aprovado AS aprovado,
	                                cgt.numero_clausula clausula_geral_numero,
	                                coalesce(ecat.quantidade, 0) estrutura_clausula_quantidade_campos_adicionais,
	                                coalesce(cgec.quantidade, 0) clausula_geral_quantidade_campos_adicionais,
	                                cgt.data_aprovacao clausula_geral_data_aprovacao
                                FROM 
	                                    doc_sind dct
                                INNER JOIN clausula_geral cgt ON
	                                dct.id_doc = cgt.doc_sind_id_documento
                                INNER JOIN 
	                                    estrutura_clausula ect ON
	                                cgt.estrutura_id_estruturaclausula = ect.id_estruturaclausula
                                INNER JOIN 
	                                    grupo_clausula gct ON
	                                ect.grupo_clausula_idgrupo_clausula = gct.idgrupo_clausula
                                LEFT JOIN LATERAL(
	                                select
		                                COUNT(1) quantidade
	                                from
		                                estrutura_clausulas_ad_tipoinformacaoadicional ecat
	                                where
		                                ecat.estrutura_clausula_id_estruturaclausula = ect.id_estruturaclausula
	                                ) ecat on
	                                true
                                LEFT JOIN LATERAL(
	                                select
		                                COUNT(1) quantidade
	                                from
		                                clausula_geral_estrutura_clausula cgec
	                                where
		                                cgec.clausula_geral_id_clau = cgt.id_clau 
                                        and cgec.ad_tipoinformacaoadicional_cdtipoinformacaoadicional <> 170
	                                ) cgec on
	                                true;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"drop view if exists mapa_sindical_vw;");

            migrationBuilder.Sql(@"CREATE OR REPLACE
                                VIEW mapa_sindical_vw AS
	                                select
	                                cgt.id_clau AS clausula_id,
	                                cgt.tex_clau AS clausula_texto,
	                                gct.idgrupo_clausula AS grupo_clausula_id,
	                                gct.nome_grupo AS grupo_clausula_nome,
	                                ect.id_estruturaclausula AS estrutura_clausula_id,
	                                ect.nome_clausula AS estrutura_clausula_nome,
	                                dct.id_doc AS documento_id,
	                                dct.sind_patronal AS documento_sindicato_patronal,
	                                dct.sind_laboral AS documento_sindicato_laboral,
	                                dct.data_reg_mte AS data_registro,
	                                dct.data_aprovacao AS documento_data_aprovacao,
	                                dct.database_doc AS documento_database,
	                                dct.tipo_doc_idtipo_doc AS documento_tipo_id,
	                                dct.cnae_doc AS documento_cnae,
	                                dct.abrangencia AS documento_abrangencia,
	                                dct.cliente_estabelecimento AS documento_estabelecimento,
	                                dct.validade_inicial AS documento_validade_inicial,
	                                dct.validade_final AS documento_validade_final,
	                                COALESCE(cgt.liberado, 'N') AS clausula_geral_liberada,
	                                dct.uf AS documento_uf,
	                                dct.descricao_documento AS documento_descricao,
	                                dct.titulo_documento AS documento_titulo,
	                                JSON_LENGTH(dct.sind_patronal) AS quantidade_sindicatos_patronais,
	                                JSON_LENGTH(dct.sind_laboral) AS quantidade_sindicatos_laborais,
	                                cgt.aprovado AS aprovado,
	                                cgt.numero_clausula clausula_geral_numero,
	                                coalesce(ecat.quantidade, 0) estrutura_clausula_quantidade_campos_adicionais,
	                                coalesce(cgec.quantidade, 0) clausula_geral_quantidade_campos_adicionais,
	                                cgt.data_aprovacao clausula_geral_data_aprovacao
                                FROM 
	                                    doc_sind dct
                                INNER JOIN clausula_geral cgt ON
	                                dct.id_doc = cgt.doc_sind_id_documento
                                INNER JOIN 
	                                    estrutura_clausula ect ON
	                                cgt.estrutura_id_estruturaclausula = ect.id_estruturaclausula
                                INNER JOIN 
	                                    grupo_clausula gct ON
	                                ect.grupo_clausula_idgrupo_clausula = gct.idgrupo_clausula
                                LEFT JOIN LATERAL(
	                                select
		                                COUNT(1) quantidade
	                                from
		                                estrutura_clausulas_ad_tipoinformacaoadicional ecat
	                                where
		                                ecat.estrutura_clausula_id_estruturaclausula = ect.id_estruturaclausula
	                                ) ecat on
	                                true
                                LEFT JOIN LATERAL(
	                                select
		                                COUNT(1) quantidade
	                                from
		                                clausula_geral_estrutura_clausula cgec
	                                where
		                                cgec.clausula_geral_id_clau = cgt.id_clau 
	                                ) cgec on
	                                true;");
        }
    }
}
