using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ineditta.Repository.Migrations
{
    /// <inheritdoc />
    public partial class v135 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                                            ) sinde ON TRUE"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP VIEW IF EXISTS clausulas_vw;");
            migrationBuilder.Sql(@"create or replace view clausulas_vw as
                                        select cgt.id_clau clausula_id,
                                            cgt.tex_clau clausula_texto,
                                            gct.idgrupo_clausula grupo_clausula_id,
                                            gct.nome_grupo grupo_clausula_nome,
                                            ect.id_estruturaclausula estrutura_clausula_id,
                                            ect.nome_clausula estrutura_clausula_nome,
                                            dct.id_doc documento_id,
                                            dct.sind_patronal documento_sindicato_patronal,
                                            dct.sind_laboral documento_sindicato_laboral,
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
                                            JSON_LENGTH(dct.sind_patronal) quantidade_sindicatos_patronais,
                                            JSON_LENGTH(dct.sind_laboral)  quantidade_sindicatos_laborais,
                                            cgt.aprovado,
                                            tdoc.nome_doc documento_nome,
                                            dct.referencia documento_referencia
                                            from doc_sind dct  
                                                left join clausula_geral cgt on dct.id_doc = cgt.doc_sind_id_documento 
                                                left join estrutura_clausula ect on cgt.estrutura_id_estruturaclausula = ect.id_estruturaclausula 
                                                left join grupo_clausula gct on ect.grupo_clausula_idgrupo_clausula = gct.idgrupo_clausula
                                                left join tipo_doc tdoc on dct.tipo_doc_idtipo_doc = tdoc.idtipo_doc");
        }
    }
}
