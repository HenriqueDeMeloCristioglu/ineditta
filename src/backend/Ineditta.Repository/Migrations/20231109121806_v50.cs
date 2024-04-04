using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ineditta.Repository.Migrations
{
    /// <inheritdoc />
    public partial class v50 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "percentual",
                table: "clausula_geral_estrutura_clausula",
                type: "decimal(65,30)",
                nullable: true,
                oldClrType: typeof(float),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "numerico",
                table: "clausula_geral_estrutura_clausula",
                type: "decimal(65,30)",
                nullable: true,
                oldClrType: typeof(float),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.Sql(@"ALTER TABLE clausula_geral_estrutura_clausula MODIFY combo TEXT;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<float>(
                name: "percentual",
                table: "clausula_geral_estrutura_clausula",
                type: "float",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(65,30)",
                oldNullable: true);

            migrationBuilder.AlterColumn<float>(
                name: "numerico",
                table: "clausula_geral_estrutura_clausula",
                type: "float",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(65,30)",
                oldNullable: true);

            migrationBuilder.Sql(@"ALTER TABLE clausula_geral_estrutura_clausula MODIFY combo VARCHAR(500);");
        }
    }
}
