using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ineditta.Repository.Migrations
{
    /// <inheritdoc />
    public partial class v38 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<sbyte>(
                name: "exibe_comparativo_mapa_sindical",
                table: "estrutura_clausulas_ad_tipoinformacaoadicional",
                type: "TINYINT(0)",
                nullable: false,
                defaultValue: (sbyte)0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "exibe_comparativo_mapa_sindical",
                table: "estrutura_clausulas_ad_tipoinformacaoadicional");
        }
    }
}
