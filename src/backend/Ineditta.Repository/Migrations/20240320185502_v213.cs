using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ineditta.Repository.Migrations
{
    /// <inheritdoc />
    public partial class v213 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "clausula_grupo_economico_tb",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    grupo_economico_id = table.Column<int>(type: "int", nullable: false),
                    clausula_geral_id = table.Column<int>(type: "int", nullable: true)
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "clausula_grupo_economico_tb");
        }
    }
}
