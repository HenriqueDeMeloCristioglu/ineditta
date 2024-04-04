using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ineditta.Repository.Migrations
{
    /// <inheritdoc />
    public partial class v203 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "etiqueta_id",
                table: "acompanhamento_cct_tb",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "acompanhamento_cct_etiqueta_tb",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    descricao = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_acompanhamento_cct_etiqueta_tb", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");
#pragma warning restore S125 // Sections of code should not be commented out

            migrationBuilder.CreateIndex(
                name: "IX_acompanhamento_cct_tb_etiqueta_id",
                table: "acompanhamento_cct_tb",
                column: "etiqueta_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_acompanhanto_cct_x_acompanhamento_cct_etiqueta",
                table: "acompanhamento_cct_tb");

            migrationBuilder.DropForeignKey(
                name: "ia_clausula_tb_ibfk_1",
                table: "ia_clausula_tb");

            migrationBuilder.DropTable(
                name: "acompanhamento_cct_etiqueta_tb");

            migrationBuilder.DropIndex(
                name: "IX_acompanhamento_cct_tb_etiqueta_id",
                table: "acompanhamento_cct_tb");

            migrationBuilder.DropColumn(
                name: "etiqueta_id",
                table: "acompanhamento_cct_tb");

            migrationBuilder.AlterColumn<int>(
                name: "consta_no_documento",
                table: "clausula_geral",
                type: "int",
                nullable: false,
                defaultValue: 1,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "ia_clausula_tb_ibfk_1",
                table: "ia_clausula_tb",
                column: "estrutura_clausula_id",
                principalTable: "EstruturaClausula",
                principalColumn: "TempId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "sinonimos_ibfk_2",
                table: "sinonimos",
                column: "estrutura_clausula_id_estruturaclausula",
                principalTable: "EstruturaClausula",
                principalColumn: "TempId1");
        }
    }
}
