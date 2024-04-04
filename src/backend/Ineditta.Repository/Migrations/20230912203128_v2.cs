using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ineditta.Repository.Migrations
{
    /// <inheritdoc />
    public partial class v2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"drop view if exists documento_sindical_vw;");

            migrationBuilder.Sql(@"create or replace view documento_sindical_vw as
                                    select dst.id_doc, tdt.nome_doc, dst.data_upload, dst.validade_inicial, dst.validade_final,
                                     JSON_EXTRACT(dst.sind_laboral, '$[*].sigla') as sindicatos_laborais_siglas,
                                     JSON_EXTRACT(dst.sind_patronal , '$[*].sigla') as sindicatos_patronais_siglas, 
                                     JSON_EXTRACT(dst.sind_laboral, '$[*].municipio') sindicatos_laborais_municipios,
                                     dst.modulo,
                                     dst.sind_laboral,
                                     dst.sind_patronal,
                                     dst.cliente_estabelecimento,
                                     dst.referencia estrutura_clausulas_ids,
                                     dst.caminho_arquivo,
                                     dst.database_doc,
                                     dst.abrangencia,
                                     dst.cnae_doc,
                                     dst.descricao_documento,
                                     tdt.tipo_doc tipo_documento,
                                     dst.anuencia,
                                     dst.doc_restrito,
                                     dst.data_aprovacao
                                    from doc_sind dst
                                    left join tipo_doc tdt on dst.tipo_doc_idtipo_doc = tdt.idtipo_doc 
                                    left join estrutura_clausula as ect on JSON_CONTAINS(dst.referencia, CONCAT('""',ect.id_estruturaclausula, '""'),'$');");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"drop view if exists documento_sindical_vw;");

            migrationBuilder.Sql(@"create or replace view documento_sindical_vw as
                                select dst.id_doc, tdt.nome_doc, dst.data_upload, dst.validade_inicial, dst.validade_final,
                                 JSON_EXTRACT(dst.sind_laboral, '$[*].sigla') as sindicatos_laborais_siglas,
                                 JSON_EXTRACT(dst.sind_patronal , '$[*].sigla') as sindicatos_patronais_siglas, 
                                 dst.modulo,
                                 dst.sind_laboral,
                                 dst.sind_patronal,
                                 dst.cliente_estabelecimento,
                                 dst.referencia estrutura_clausulas_ids,
                                 dst.caminho_arquivo,
                                 dst.database_doc,
                                 dst.abrangencia,
                                 dst.cnae_doc,
                                 dst.descricao_documento,
                                 tdt.tipo_doc tipo_documento,
                                 dst.anuencia,
                                 dst.doc_restrito,
                                 dst.data_aprovacao
                                from doc_sind dst
                                left join tipo_doc tdt on dst.tipo_doc_idtipo_doc = tdt.idtipo_doc 
                                left join estrutura_clausula as ect on JSON_CONTAINS(dst.referencia, CONCAT('\""',ect.id_estruturaclausula, '\""'),'$');");
        }
    }
}
