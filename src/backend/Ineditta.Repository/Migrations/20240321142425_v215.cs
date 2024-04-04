using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ineditta.Repository.Migrations
{
    /// <inheritdoc />
    public partial class v215 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "clausula_grupo_economico_tb");

            migrationBuilder.CreateTable(
                name: "estrutura_clausula_grupo_economico_tb",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    grupo_economico_id = table.Column<int>(type: "int", nullable: false),
                    estrutura_clausula_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_estrutura_clausula_grupo_economico_tb", x => x.id);
                    table.ForeignKey(
                        name: "fk_estrutura_clausula_grupo_economico_x_cliente_grupo",
                        column: x => x.grupo_economico_id,
                        principalTable: "cliente_grupo",
                        principalColumn: "id_grupo_economico",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_estrutura_clausula_x_estrutura_clausula_grupo_economico",
                        column: x => x.estrutura_clausula_id,
                        principalTable: "estrutura_clausula",
                        principalColumn: "id_estruturaclausula",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateIndex(
                name: "IX_estrutura_clausula_grupo_economico_tb_estrutura_clausula_id",
                table: "estrutura_clausula_grupo_economico_tb",
                column: "estrutura_clausula_id");

            migrationBuilder.CreateIndex(
                name: "IX_estrutura_clausula_grupo_economico_tb_grupo_economico_id",
                table: "estrutura_clausula_grupo_economico_tb",
                column: "grupo_economico_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "estrutura_clausula_grupo_economico_tb");

            migrationBuilder.CreateTable(
                name: "clausula_grupo_economico_tb",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    clausula_geral_id = table.Column<int>(type: "int", nullable: true),
                    grupo_economico_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_clausula_grupo_economico_tb", x => x.id);
                    table.ForeignKey(
                        name: "fk_clausula_geral_x_clausula_geral_grupo_economico",
                        column: x => x.clausula_geral_id,
                        principalTable: "clausula_geral",
                        principalColumn: "id_clau",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_clausula_grupo_economico_x_cliente_grupo",
                        column: x => x.grupo_economico_id,
                        principalTable: "cliente_grupo",
                        principalColumn: "id_grupo_economico",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateIndex(
                name: "IX_clausula_grupo_economico_tb_clausula_geral_id",
                table: "clausula_grupo_economico_tb",
                column: "clausula_geral_id");

            migrationBuilder.CreateIndex(
                name: "IX_clausula_grupo_economico_tb_grupo_economico_id",
                table: "clausula_grupo_economico_tb",
                column: "grupo_economico_id");
        }
    }
}
