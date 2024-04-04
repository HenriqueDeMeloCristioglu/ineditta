using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ineditta.Repository.Migrations
{
    /// <inheritdoc />
    public partial class v147 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "cliente_unidade_sindicato_patronal_tb",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    cliente_unidade_id = table.Column<int>(type: "int", nullable: false),
                    sindicato_patronal_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "cliente_unidade_sindicato_patronal_ibfk_2",
                        column: x => x.sindicato_patronal_id,
                        principalTable: "sind_patr",
                        principalColumn: "id_sindp",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "cliente_unidade_sinedicato_patronal_ibfk_1",
                        column: x => x.cliente_unidade_id,
                        principalTable: "cliente_unidades",
                        principalColumn: "id_unidade",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateIndex(
                name: "IX_cliente_unidade_sindicato_patronal_tb_cliente_unidade_id",
                table: "cliente_unidade_sindicato_patronal_tb",
                column: "cliente_unidade_id");

            migrationBuilder.CreateIndex(
                name: "IX_cliente_unidade_sindicato_patronal_tb_sindicato_patronal_id",
                table: "cliente_unidade_sindicato_patronal_tb",
                column: "sindicato_patronal_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "cliente_unidade_sindicato_patronal_tb");
        }
    }
}
