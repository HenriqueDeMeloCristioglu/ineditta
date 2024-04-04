using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ineditta.Repository.Migrations
{
    /// <inheritdoc />
    public partial class v32 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
                                            cgt.aprovado
                                            from doc_sind dct  
                                            left join clausula_geral cgt on dct.id_doc = cgt.doc_sind_id_documento 
                                            left join estrutura_clausula ect on cgt.estrutura_id_estruturaclausula = ect.id_estruturaclausula 
                                            left join grupo_clausula gct on ect.grupo_clausula_idgrupo_clausula = gct.idgrupo_clausula");
        }
    }
}
