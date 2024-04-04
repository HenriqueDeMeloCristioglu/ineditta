using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ineditta.Repository.Migrations
{
    /// <inheritdoc />
    public partial class v59 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "informacao_adicional_cliente_tb",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    grupo_economico_id = table.Column<int>(type: "int", nullable: false),
                    documento_sindical_id = table.Column<int>(type: "int", nullable: false),
                    informacoes_adicionais = table.Column<string>(type: "json", nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    data_alteracao = table.Column<DateTime>(type: "DATETIME", nullable: true),
                    data_inclusao = table.Column<DateTime>(type: "DATETIME", nullable: true),
                    usuario_alteracao_id = table.Column<int>(type: "int", nullable: true),
                    usuario_inclusao_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "fk_informacao_adicional_cliente_tb_x_cliente_grupo",
                        column: x => x.grupo_economico_id,
                        principalTable: "cliente_grupo",
                        principalColumn: "id_grupo_economico",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_informacao_adicional_cliente_tb_x_docsind",
                        column: x => x.documento_sindical_id,
                        principalTable: "doc_sind",
                        principalColumn: "id_doc",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateIndex(
                name: "IX_informacao_adicional_cliente_tb_grupo_economico_id",
                table: "informacao_adicional_cliente_tb",
                column: "grupo_economico_id");

            migrationBuilder.CreateIndex(
                name: "uk001_informacao_adicional_cliente_tb",
                table: "informacao_adicional_cliente_tb",
                columns: new[] { "documento_sindical_id", "grupo_economico_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "informacao_adicional_cliente_tb");
        }
    }
}
