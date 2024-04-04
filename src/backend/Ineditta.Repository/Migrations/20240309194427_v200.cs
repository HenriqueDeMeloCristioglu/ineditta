using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ineditta.Repository.Migrations
{
    /// <inheritdoc />
    public partial class v200 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "ia_clausula_tb",
                newName: "id");

            migrationBuilder.AlterColumn<int>(
                name: "status",
                table: "ia_clausula_tb",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.Sql(@"INSERT INTO modulos (modulos,tipo,criar,consultar,comentar,alterar,excluir,aprovar,uri) VALUES
	            ('Documento Sindical - IA','SISAP','S','S','S','S','S','S','ia_documento_sindical');");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "id",
                table: "ia_clausula_tb",
                newName: "Id");

            migrationBuilder.AlterColumn<string>(
                name: "status",
                table: "ia_clausula_tb",
                type: "longtext",
                nullable: false,
                collation: "utf8mb4_0900_ai_ci",
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.Sql(@"delete from modulos where id_modulos = 70;");
        }
    }
}
