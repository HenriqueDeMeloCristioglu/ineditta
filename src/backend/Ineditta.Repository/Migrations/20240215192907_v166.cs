using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ineditta.Repository.Migrations
{
    /// <inheritdoc />
    public partial class v166 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "resumivel",
                table: "clausula_geral",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "resumo_clausulas_tb",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    clausula_id = table.Column<int>(type: "int", nullable: false),
                    texto = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_resumo_clausulas_tb", x => x.id);
                    table.ForeignKey(
                        name: "fk_resumo_clausulas_x_clausula_geral",
                        column: x => x.clausula_id,
                        principalTable: "clausula_geral",
                        principalColumn: "id_clau",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateIndex(
                name: "IX_resumo_clausulas_tb_clausula_id",
                table: "resumo_clausulas_tb",
                column: "clausula_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "resumo_clausulas_tb");

            migrationBuilder.DropColumn(
                name: "resumivel",
                table: "clausula_geral");
        }
    }
}
