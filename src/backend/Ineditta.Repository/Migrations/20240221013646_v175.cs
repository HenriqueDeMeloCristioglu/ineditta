using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ineditta.Repository.Migrations
{
    /// <inheritdoc />
    public partial class v175 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "permissao",
                table: "tipo_doc",
                type: "varchar(3)",
                maxLength: 3,
                nullable: true,
                defaultValueSql: "'sim'",
                collation: "utf8mb4_0900_ai_ci",
                oldClrType: typeof(string),
                oldType: "varchar(3)",
                oldMaxLength: 3)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.UpdateData(
                table: "tipo_doc",
                keyColumn: "modulo_cad",
                keyValue: null,
                column: "modulo_cad",
                value: "Geral");

            migrationBuilder.UpdateData(
                table: "tipo_doc",
                keyColumn: "modulo_cad",
                keyValue: "N",
                column: "modulo_cad",
                value: "Processado");

            migrationBuilder.UpdateData(
                table: "tipo_doc",
                keyColumn: "modulo_cad",
                keyValue: "S",
                column: "modulo_cad",
                value: "Geral");

            migrationBuilder.AlterColumn<string>(
                name: "modulo_cad",
                table: "tipo_doc",
                type: "varchar(25)",
                maxLength: 25,
                nullable: false,
                collation: "utf8mb4_0900_ai_ci",
                oldClrType: typeof(string),
                oldType: "varchar(25)",
                oldMaxLength: 25,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "utf8mb4_0900_ai_ci");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "tipo_doc",
                keyColumn: "permissao",
                keyValue: null,
                column: "permissao",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "permissao",
                table: "tipo_doc",
                type: "varchar(3)",
                maxLength: 3,
                nullable: false,
                collation: "utf8mb4_0900_ai_ci",
                oldClrType: typeof(string),
                oldType: "varchar(3)",
                oldMaxLength: 3,
                oldNullable: true,
                oldDefaultValueSql: "'sim'")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.AlterColumn<string>(
                name: "modulo_cad",
                table: "tipo_doc",
                type: "varchar(25)",
                maxLength: 25,
                nullable: true,
                collation: "utf8mb4_0900_ai_ci",
                oldClrType: typeof(string),
                oldType: "varchar(25)",
                oldMaxLength: 25)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "utf8mb4_0900_ai_ci");
        }
    }
}
