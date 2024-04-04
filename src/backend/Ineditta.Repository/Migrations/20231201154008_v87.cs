using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ineditta.Repository.Migrations
{
    /// <inheritdoc />
    public partial class v87 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "orientacao",
                table: "informacao_adicional_cliente_tb",
                type: "LONGTEXT",
                nullable: true,
                collation: "utf8mb4_0900_ai_ci")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "outras_informacoes",
                table: "informacao_adicional_cliente_tb",
                type: "LONGTEXT",
                nullable: true,
                collation: "utf8mb4_0900_ai_ci")
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "orientacao",
                table: "informacao_adicional_cliente_tb");

            migrationBuilder.DropColumn(
                name: "outras_informacoes",
                table: "informacao_adicional_cliente_tb");
        }
    }
}
