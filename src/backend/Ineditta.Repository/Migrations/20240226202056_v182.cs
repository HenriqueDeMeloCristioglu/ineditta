using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ineditta.Repository.Migrations
{
    /// <inheritdoc />
    public partial class v182 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "data_processamento_documento",
                table: "clausula_geral",
                type: "date",
                nullable: true);

            migrationBuilder.Sql(@"DROP VIEW IF EXISTS clausulas_vw;");

            migrationBuilder.Sql(@"CREATE OR REPLACE VIEW clausulas_vw AS
                                    select
                                        cgt.id_clau AS clausula_id,
                                        cgt.tex_clau AS clausula_texto,
                                        gct.idgrupo_clausula AS grupo_clausula_id,
                                        gct.nome_grupo AS grupo_clausula_nome,
                                        ect.id_estruturaclausula AS estrutura_clausula_id,
                                        ect.nome_clausula AS estrutura_clausula_nome,
                                        dct.id_doc AS documento_id,
                                        sindp.array AS documento_sindicato_patronal,
                                        sinde.array AS documento_sindicato_laboral,
                                        dct.data_reg_mte AS data_registro,
                                        dct.data_aprovacao AS data_aprovacao,
                                        dct.database_doc AS documento_database,
                                        dct.tipo_doc_idtipo_doc AS documento_tipo_id,
                                        dct.cnae_doc AS documento_cnae,
                                        dct.abrangencia AS documento_abrangencia,
                                        dct.cliente_estabelecimento AS documento_estabelecimento,
                                        dct.validade_inicial AS documento_validade_inicial,
                                        dct.validade_final AS documento_validade_final,
                                        coalesce(cgt.liberado, 'N') AS clausula_geral_liberada,
                                        cgt.data_aprovacao AS clausula_geral_data_aprovacao,
                                        json_length(dct.sind_patronal) AS quantidade_sindicatos_patronais,
                                        json_length(dct.sind_laboral) AS quantidade_sindicatos_laborais,
                                        cgt.aprovado AS aprovado,
                                        tdoc.nome_doc AS documento_nome,
                                        dct.referencia AS documento_referencia,
                                        cgt.texto_resumido,
                                        cgt.resumivel,
                                        cgt.data_processamento_documento,
                                        cgt.consta_no_documento
                                    from doc_sind dct
                                    left join clausula_geral cgt on dct.id_doc = cgt.doc_sind_id_documento
                                    left join estrutura_clausula ect on cgt.estrutura_id_estruturaclausula = ect.id_estruturaclausula
                                    left join grupo_clausula gct on ect.grupo_clausula_idgrupo_clausula = gct.idgrupo_clausula
                                    left join tipo_doc tdoc on dct.tipo_doc_idtipo_doc = tdoc.idtipo_doc
                                    left join lateral (
                                        select
                                            json_arrayagg(json_object('id', sp.id_sindp, 'uf', sp.uf_sp, 'sigla', sp.sigla_sp, 'cnpj', sp.cnpj_sp, 'codigo', sp.codigo_sp, 'municipio', sp.municipio_sp, 'denominacao', sp.denominacao_sp)) AS array
                                        from documento_sindicato_patronal_tb dsp
                                        join sind_patr sp on sp.id_sindp = dsp.sindicato_patronal_id
                                        where dsp.documento_id = dct.id_doc
                                    ) sindp on true
                                    left join lateral (
                                        select
                                            json_arrayagg(json_object('id', se.id_sinde, 'uf', se.uf_sinde, 'sigla', se.sigla_sinde, 'cnpj', se.cnpj_sinde, 'codigo', se.codigo_sinde, 'municipio', se.municipio_sinde, 'denominacao', se.denominacao_sinde)) AS array
                                        from documento_sindicato_laboral_tb dsl
                                        join sind_emp se on se.id_sinde = dsl.sindicato_laboral_id
                                        where
                                            dsl.documento_id = dct.id_doc
                                    ) sinde on true;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "data_processamento_documento",
                table: "clausula_geral");

            migrationBuilder.Sql(@"DROP VIEW IF EXISTS clausulas_vw;");

            migrationBuilder.Sql(@"create or replace view clausulas_vw as
                                        select cgt.id_clau clausula_id,
                                            cgt.tex_clau clausula_texto,
                                            gct.idgrupo_clausula grupo_clausula_id,
                                            gct.nome_grupo grupo_clausula_nome,
                                            ect.id_estruturaclausula estrutura_clausula_id,
                                            ect.nome_clausula estrutura_clausula_nome,
                                            dct.id_doc documento_id,
                                            sindp.array documento_sindicato_patronal,
                                            sinde.array documento_sindicato_laboral,
                                            dct.data_reg_mte data_registro,
                                            dct.data_aprovacao data_aprovacao,
                                            dct.database_doc AS documento_database, 
                                            dct.tipo_doc_idtipo_doc AS documento_tipo_id,
                                            dct.cnae_doc AS documento_cnae,
                                            dct.abrangencia AS documento_abrangencia,
                                            dct.cliente_estabelecimento AS documento_estabelecimento,
                                            dct.validade_inicial AS documento_validade_inicial,
                                            dct.validade_final AS documento_validade_final,
                                            coalesce(cgt.liberado, 'N') clausula_geral_liberada,
                                            cgt.data_aprovacao clausula_geral_data_aprovacao,
                                            JSON_LENGTH(dct.sind_patronal) quantidade_sindicatos_patronais,
                                            JSON_LENGTH(dct.sind_laboral)  quantidade_sindicatos_laborais,
                                            cgt.aprovado,
                                            tdoc.nome_doc documento_nome,
                                            dct.referencia documento_referencia
                                            from doc_sind dct  
                                            left join clausula_geral cgt on dct.id_doc = cgt.doc_sind_id_documento 
                                            left join estrutura_clausula ect on cgt.estrutura_id_estruturaclausula = ect.id_estruturaclausula 
                                            left join grupo_clausula gct on ect.grupo_clausula_idgrupo_clausula = gct.idgrupo_clausula
                                            left join tipo_doc tdoc on dct.tipo_doc_idtipo_doc = tdoc.idtipo_doc
                                            LEFT JOIN LATERAL (
                                                SELECT JSON_ARRAYAGG(JSON_OBJECT('id', id_sindp, 'uf', uf_sp, 'sigla', sigla_sp, 'cnpj', cnpj_sp, 'codigo', codigo_sp, 'municipio', municipio_sp, 'denominacao', denominacao_sp)) AS array 
                                                FROM documento_sindicato_patronal_tb dsp
                                                INNER JOIN sind_patr sp ON sp.id_sindp = dsp.sindicato_patronal_id
                                                WHERE dsp.documento_id = dct.id_doc 
                                            ) sindp ON TRUE
                                            LEFT JOIN LATERAL (
                                                SELECT JSON_ARRAYAGG(JSON_OBJECT('id', id_sinde, 'uf', uf_sinde, 'sigla', sigla_sinde, 'cnpj', cnpj_sinde, 'codigo', codigo_sinde, 'municipio', municipio_sinde, 'denominacao', denominacao_sinde)) AS array 
                                                FROM documento_sindicato_laboral_tb dsl
                                                INNER JOIN sind_emp se ON se.id_sinde = dsl.sindicato_laboral_id
                                                WHERE dsl.documento_id = dct.id_doc
                                            ) sinde ON TRUE;"
            );
        }
    }
}
