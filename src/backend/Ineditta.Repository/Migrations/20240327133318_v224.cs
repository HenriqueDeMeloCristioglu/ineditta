using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ineditta.Repository.Migrations
{
    /// <inheritdoc />
    public partial class v224 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "atividade_economica_id",
                table: "documento_atividade_economica_tb",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_documento_atividade_economica_tb_atividade_economica_id",
                table: "documento_atividade_economica_tb",
                column: "atividade_economica_id");

            migrationBuilder.AddForeignKey(
                name: "fk_documento_atividade_economica_x_classes_cnae",
                table: "documento_atividade_economica_tb",
                column: "atividade_economica_id",
                principalTable: "classe_cnae",
                principalColumn: "id_cnae",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.Sql(@"create procedure inserir_cnaes_documentos()
                BEGIN
	                TRUNCATE documento_atividade_economica_tb;
	
	                insert into documento_atividade_economica_tb (documento_id, atividade_economica_id)
	                SELECT ds.id_doc documento_id, atv.value atividade_economica_id from doc_sind ds,
	                json_table(ds.cnae_doc, '$[*].id' columns (value int path '$')) as atv;
                END;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_documento_atividade_economica_x_classes_cnae",
                table: "documento_atividade_economica_tb");

            migrationBuilder.DropIndex(
                name: "IX_documento_atividade_economica_tb_atividade_economica_id",
                table: "documento_atividade_economica_tb");

            migrationBuilder.DropColumn(
                name: "atividade_economica_id",
                table: "documento_atividade_economica_tb");
        }
    }
}
