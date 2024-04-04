using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ineditta.Repository.Migrations
{
    /// <inheritdoc />
    public partial class v204 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "abrangencia",
                table: "acompanhamento_cct_tb",
                newName: "anotacoes");

            migrationBuilder.CreateTable(
                name: "acompanhamento_cct_assunto_tb",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    acompanhamento_cct_id = table.Column<long>(type: "bigint", nullable: false),
                    estrutura_clausula_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_acompanhamento_cct_assunto_tb", x => x.id);
                    table.ForeignKey(
                        name: "fk_acompanhamento_cct_assunto_x_acompanhamento_cct",
                        column: x => x.acompanhamento_cct_id,
                        principalTable: "acompanhamento_cct_tb",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_acompanhamento_cct_assunto_x_estrutura_clausula",
                        column: x => x.estrutura_clausula_id,
                        principalTable: "estrutura_clausula",
                        principalColumn: "id_estruturaclausula",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateIndex(
                name: "IX_acompanhamento_cct_assunto_tb_acompanhamento_cct_id",
                table: "acompanhamento_cct_assunto_tb",
                column: "acompanhamento_cct_id");

            migrationBuilder.CreateIndex(
                name: "IX_acompanhamento_cct_assunto_tb_estrutura_clausula_id",
                table: "acompanhamento_cct_assunto_tb",
                column: "estrutura_clausula_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "acompanhamento_cct_assunto_tb");

            migrationBuilder.RenameColumn(
                name: "anotacoes",
                table: "acompanhamento_cct_tb",
                newName: "abrangencia");
        }
    }
}
