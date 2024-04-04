using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ineditta.Repository.Migrations
{
    /// <inheritdoc />
    public partial class v207 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "ia_clausula_tb_ibfk_3",
                table: "ia_clausula_tb");

            migrationBuilder.Sql("ALTER TABLE sinonimos DROP FOREIGN KEY sinonimos_ibfk_2;");

            migrationBuilder.DropIndex(
                name: "IX_ia_clausula_tb_documento_sindical_id",
                table: "ia_clausula_tb");

            migrationBuilder.RenameColumn(
                name: "documento_sindical_id",
                table: "ia_clausula_tb",
                newName: "ia_documento_sindical_id");

            migrationBuilder.AlterColumn<long>(
                name: "ia_documento_sindical_id",
                table: "ia_clausula_tb",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: false);

            migrationBuilder.AlterTable(
                name: "sinonimos")
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci")
                .OldAnnotation("MySql:CharSet", "utf8mb3")
                .OldAnnotation("Relational:Collation", "utf8mb3_general_ci");

            migrationBuilder.UpdateData(
                table: "sinonimos",
                keyColumn: "nome_sinonimo",
                keyValue: null,
                column: "nome_sinonimo",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "nome_sinonimo",
                table: "sinonimos",
                type: "text",
                nullable: false,
                collation: "utf8mb4_0900_ai_ci",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb3")
                .OldAnnotation("Relational:Collation", "utf8mb3_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_sinonimos_assunto_idassunto",
                table: "sinonimos",
                column: "assunto_idassunto");

            migrationBuilder.CreateIndex(
                name: "IX_ia_clausula_tb_ia_documento_sindical_id",
                table: "ia_clausula_tb",
                column: "ia_documento_sindical_id");

            migrationBuilder.AddForeignKey(
                name: "ia_clausula_tb_ibfk_3",
                table: "ia_clausula_tb",
                column: "ia_documento_sindical_id",
                principalTable: "ia_documento_sindical_tb",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_sinonimo_x_assunto",
                table: "sinonimos",
                column: "assunto_idassunto",
                principalTable: "assunto",
                principalColumn: "idassunto");

            migrationBuilder.AddForeignKey(
                name: "fk_sinonimo_x_estrutura_clausula",
                table: "sinonimos",
                column: "estrutura_clausula_id_estruturaclausula",
                principalTable: "estrutura_clausula",
                principalColumn: "id_estruturaclausula",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "ia_clausula_tb_ibfk_3",
                table: "ia_clausula_tb");

            migrationBuilder.DropForeignKey(
                name: "fk_sinonimo_x_assunto",
                table: "sinonimos");

            migrationBuilder.DropForeignKey(
                name: "fk_sinonimo_x_estrutura_clausula",
                table: "sinonimos");

            migrationBuilder.DropIndex(
                name: "IX_sinonimos_assunto_idassunto",
                table: "sinonimos");

            migrationBuilder.DropIndex(
                name: "IX_ia_clausula_tb_ia_documento_sindical_id",
                table: "ia_clausula_tb");

            migrationBuilder.AlterColumn<int>(
                name: "ia_documento_sindical_id",
                table: "ia_clausula_tb",
                type: "int",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.RenameColumn(
                name: "ia_documento_sindical_id",
                table: "ia_clausula_tb",
                newName: "documento_sindical_id");

            migrationBuilder.AlterTable(
                name: "sinonimos")
                .Annotation("MySql:CharSet", "utf8mb3")
                .Annotation("Relational:Collation", "utf8mb3_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.AlterColumn<string>(
                name: "nome_sinonimo",
                table: "sinonimos",
                type: "text",
                nullable: true,
                collation: "utf8mb3_general_ci",
                oldClrType: typeof(string),
                oldType: "text")
                .Annotation("MySql:CharSet", "utf8mb3")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateIndex(
                name: "IX_ia_clausula_tb_documento_sindical_id",
                table: "ia_clausula_tb",
                column: "documento_sindical_id");

            migrationBuilder.AddForeignKey(
                name: "ia_clausula_tb_ibfk_3",
                table: "ia_clausula_tb",
                column: "documento_sindical_id",
                principalTable: "doc_sind",
                principalColumn: "id_doc",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "sinonimos_ibfk_2",
                table: "sinonimos",
                column: "estrutura_clausula_id_estruturaclausula",
                principalTable: "estrutura_clausula",
                principalColumn: "id_estruturaclausula");
        }
    }
}
