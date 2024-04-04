using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ineditta.Repository.Migrations
{
    /// <inheritdoc />
    public partial class v134 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "usuario_adm_id_usuario",
                table: "acompanhamento_cct",
                newName: "usuario_responsavel_id");

            migrationBuilder.RenameColumn(
                name: "tipo_doc_idtipo_doc",
                table: "acompanhamento_cct",
                newName: "tipo_documento_id");

            migrationBuilder.RenameColumn(
                name: "sind_patr_id_sindp",
                table: "acompanhamento_cct",
                newName: "sindicato_patronal_id");

            migrationBuilder.RenameColumn(
                name: "sind_emp_id_sinde",
                table: "acompanhamento_cct",
                newName: "sindicato_laboral_id");

            migrationBuilder.RenameColumn(
                name: "ids_cnaes",
                table: "acompanhamento_cct",
                newName: "cnaes_ids");

            migrationBuilder.RenameColumn(
                name: "idacompanhanto_cct",
                table: "acompanhamento_cct",
                newName: "id");

            migrationBuilder.Sql(@"create or replace view acompanhamento_cct_inclusao_vw as
                                SELECT
                                    acct.id,
                                    acct.data_inicial,
                                    acct.data_final,
                                    acct.data_alteracao,
                                    stcct.descricao status,
                                    ua.nome_usuario,
                                    fcct.fase_negociacao fase,
                                    td.nome_doc nome_documento,
                                    acct.proxima_ligacao,
                                    acct.data_base,
                                    sindp.sigla_sp sigla_sindicato_patronal,
                                    sindp.uf_sp uf_sindicato_patronal,
                                    sindemp.sigla_sinde sigla_sindicato_empregado,
                                    sindemp.uf_sinde uf_sindicato_empregado,
                                    ccnae.descricao_subclasse descricao_sub_classe,
                                    acct.sindicato_laboral_id
                                FROM
                                    acompanhamento_cct acct
                                    LEFT JOIN usuario_adm ua ON acct.usuario_responsavel_id = ua.id_user
                                    LEFT JOIN fase_cct fcct ON acct.fase_id = fcct.id_fase
                                    LEFT JOIN acompanhamento_cct_status_tb stcct ON acct.status = stcct.id
                                    LEFT JOIN tipo_doc td ON acct.tipo_documento_id = td.idtipo_doc
                                    LEFT JOIN sind_patr sindp ON acct.sindicato_patronal_id = sindp.id_sindp
                                    LEFT JOIN sind_emp sindemp ON acct.sindicato_laboral_id = sindemp.id_sinde
                                    LEFT JOIN classe_cnae ccnae ON JSON_CONTAINS(acct.cnaes_ids, json_array(cast(ccnae.id_cnae as char)))
                                ORDER BY
                                    acct.proxima_ligacao DESC,
                                    acct.status;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "usuario_responsavel_id",
                table: "acompanhamento_cct",
                newName: "usuario_adm_id_usuario");

            migrationBuilder.RenameColumn(
                name: "tipo_documento_id",
                table: "acompanhamento_cct",
                newName: "tipo_doc_idtipo_doc");

            migrationBuilder.RenameColumn(
                name: "sindicato_patronal_id",
                table: "acompanhamento_cct",
                newName: "sind_patr_id_sindp");

            migrationBuilder.RenameColumn(
                name: "sindicato_laboral_id",
                table: "acompanhamento_cct",
                newName: "sind_emp_id_sinde");

            migrationBuilder.RenameColumn(
                name: "cnaes_ids",
                table: "acompanhamento_cct",
                newName: "ids_cnaes");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "acompanhamento_cct",
                newName: "idacompanhanto_cct");

            migrationBuilder.Sql(@"DROP VIEW if exists acompanhamento_cct_inclusao_vw;");
        }
    }
}
