using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ineditta.Repository.Migrations
{
    /// <inheritdoc />
    public partial class v73 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"drop view if exists documentos_sindicais_sisap_vw;");
            migrationBuilder.Sql(@"
                            create or replace view documentos_sindicais_sisap_vw as
                            select
                                ds.id_doc AS Id,
                                td.nome_doc AS NomeDocumento,
                                ds.cnae_doc AS CnaeDocs,
                                ds.data_aprovacao AS DataAprovacao,
                                ds.validade_inicial AS ValidadeInicial,
                                ds.validade_final AS ValidadeFinal,
                                ds.sind_laboral AS NomeSindicatoLaboral,
                                ds.sind_patronal AS NomeSindicatoPatronal,
                                uam.nome_usuario AS NomeUsuarioAprovador,
                                ds.data_assinatura AS DataAssinatura,
                                ds.data_sla AS DataSla,
                                cnt.cnae_subclasse_codigos as CnaeSubclasseCodigos
                            from doc_sind ds
                            left join tipo_doc td on td.idtipo_doc = ds.tipo_doc_idtipo_doc
                            left join usuario_adm uam on uam.id_user = ds.usuario_aprovador
                            left join lateral
                            (
                                select
                                    json_arrayagg(cc.subclasse_cnae) cnae_subclasse_codigos
                                from
                                    classe_cnae cc
                                where
                                    json_contains(ds.cnae_doc, concat('{""id"":', cc.id_cnae, '}'))
                            ) as cnt on true
                            where ds.modulo = 'SISAP';
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
                                ds.data_sla DataSla,
                                (SELECT JSON_ARRAYAGG(cc.subclasse_cnae) 
                                 FROM classe_cnae cc
                                 WHERE JSON_CONTAINS(`ds`.`cnae_doc`, CONCAT('{""id"":',cc.id_cnae,'}'))) AS `CnaeSubclasseCodigos`
                            FROM
                                doc_sind ds
                            LEFT JOIN tipo_doc td ON td.idtipo_doc = ds.tipo_doc_idtipo_doc
                            LEFT JOIN usuario_adm uam ON uam.id_user = ds.usuario_aprovador
                            WHERE ds.modulo = 'SISAP';
                         ");
        }
    }
}
