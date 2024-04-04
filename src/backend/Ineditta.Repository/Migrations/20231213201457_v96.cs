using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ineditta.Repository.Migrations
{
    /// <inheritdoc />
    public partial class v96 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"UPDATE clausula_geral_estrutura_clausula SET percentual = 0 WHERE percentual is null;");

            migrationBuilder.Sql(@"UPDATE clausula_geral_estrutura_clausula SET numerico = 0 WHERE numerico is null;");

            migrationBuilder.AlterColumn<decimal>(
                name: "percentual",
                table: "clausula_geral_estrutura_clausula",
                type: "decimal(18,2)",
                nullable: true,
                defaultValue: 0.00m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "numerico",
                table: "clausula_geral_estrutura_clausula",
                type: "decimal(18,2)",
                nullable: true,
                defaultValue: 0.00m,
                oldClrType: typeof(decimal),
                oldType: "decimal(65,30)",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "percentual",
                table: "clausula_geral_estrutura_clausula",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true,
                oldDefaultValue: 0.00m);

            migrationBuilder.AlterColumn<decimal>(
                name: "numerico",
                table: "clausula_geral_estrutura_clausula",
                type: "decimal(65,30)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true,
                oldDefaultValue: 0.00m);
        }
    }
}
