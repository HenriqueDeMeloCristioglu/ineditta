using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ineditta.Repository.Migrations
{
    /// <inheritdoc />
    public partial class v69 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"drop view if exists documentos_sindicais_sisap_vw;");
            migrationBuilder.Sql(@"
                            create or replace view documentos_sindicais_sisap_vw as
                            SELECT
                                ds.id_doc Id,
                                td.nome_doc NomeDocumento,
                                ds.cnae_doc CnaeDocs,
                                ds.data_aprovacao DataAprovacao,
                                ds.validade_inicial ValidadeInicial,
                                ds.validade_final ValidadeFinal,
                                ds.sind_laboral NomeSindicatoLaboral,
                                ds.sind_patronal NomeSindicatoPatronal,
                                uam.nome_usuario NomeUsuarioAprovador,
                                ds.data_assinatura DataAssinatura,
                                (SELECT JSON_ARRAYAGG(cc.subclasse_cnae) 
                                 FROM classe_cnae cc
                                 WHERE JSON_CONTAINS(`ds`.`cnae_doc`, CONCAT('{""id"":',cc.id_cnae,'}'))) AS `CnaeSubclasseCodigos`
                            FROM
                                doc_sind ds
                            LEFT JOIN tipo_doc td ON td.idtipo_doc = ds.tipo_doc_idtipo_doc
                            LEFT JOIN usuario_adm uam ON uam.id_user = ds.usuario_aprovador
                            WHERE ds.modulo = 'SISAP'
                         ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"drop view if exists documentos_sindicais_sisap_vw;");
            migrationBuilder.Sql(@"
                            create or replace view documentos_sindicais_sisap_vw as
                            SELECT
                                ds.id_doc Id,
                                td.nome_doc NomeDocumento,
                                ds.cnae_doc CnaeDocs,
                                ds.data_aprovacao DataAprovacao,
                                ds.validade_inicial ValidadeInicial,
                                ds.validade_final ValidadeFinal,
                                ds.sind_laboral NomeSindicatoLaboral,
                                ds.sind_patronal NomeSindicatoPatronal,
                                uam.nome_usuario NomeUsuarioAprovador,
                                ds.data_assinatura DataAssinatura,
                                (SELECT JSON_ARRAYAGG(cc.subclasse_cnae) 
                                 FROM classe_cnae cc
                                 WHERE JSON_CONTAINS(`ds`.`cnae_doc`, CONCAT('{""id"":',cc.id_cnae,'}'))) AS `CnaeSubclasseCodigos`
                            FROM
                                doc_sind ds
                            LEFT JOIN tipo_doc td ON td.idtipo_doc = ds.tipo_doc_idtipo_doc
                            LEFT JOIN usuario_adm uam ON uam.id_user = ds.usuario_responsavel
                            WHERE ds.modulo = 'SISAP'
                         ");
        }
    }
}
