using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ineditta.Repository.Migrations
{
    /// <inheritdoc />
    public partial class v146 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix002_acompanhamento_cct_localizacao_tb",
                table: "acompanhamento_cct_localizacao_tb");

            migrationBuilder.CreateIndex(
                name: "ix002_acompanhamento_cct_localizacao_tb",
                table: "acompanhamento_cct_localizacao_tb",
                columns: new[] { "acompanhamento_cct_id", "localizacao_id" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix002_acompanhamento_cct_localizacao_tb",
                table: "acompanhamento_cct_localizacao_tb");

            migrationBuilder.CreateIndex(
                name: "ix002_acompanhamento_cct_localizacao_tb",
                table: "acompanhamento_cct_localizacao_tb",
                columns: new[] { "acompanhamento_cct_id", "localizacao_id" },
                unique: true);
        }
    }
}
