using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ineditta.Repository.Migrations
{
    /// <inheritdoc />
    public partial class v198 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_documento_sindical_tb",
                table: "documento_sindical_tb");

            migrationBuilder.RenameTable(
                name: "documento_sindical_tb",
                newName: "ia_documento_sindical_tb");

            migrationBuilder.RenameIndex(
                name: "IX_documento_sindical_tb_documento_referencia_id",
                table: "ia_documento_sindical_tb",
                newName: "IX_ia_documento_sindical_tb_documento_referencia_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ia_documento_sindical_tb",
                table: "ia_documento_sindical_tb",
                column: "id");

            migrationBuilder.CreateTable(
                name: "ia_clausula_tb",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    texto = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    documento_sindical_id = table.Column<int>(type: "int", nullable: false),
                    estrutura_clausula_id = table.Column<int>(type: "int", nullable: false),
                    data_processamento = table.Column<DateOnly>(type: "date", nullable: false),
                    numero = table.Column<int>(type: "int", nullable: false),
                    sinonimo_id = table.Column<int>(type: "int", nullable: false),
                    status = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "ia_clausula_tb_ibfk_1",
                        column: x => x.estrutura_clausula_id,
                        principalTable: "estrutura_clausula",
                        principalColumn: "id_estruturaclausula",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "ia_clausula_tb_ibfk_2",
                        column: x => x.sinonimo_id,
                        principalTable: "sinonimos",
                        principalColumn: "id_sinonimo",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "ia_clausula_tb_ibfk_3",
                        column: x => x.documento_sindical_id,
                        principalTable: "doc_sind",
                        principalColumn: "id_doc",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateIndex(
                name: "IX_ia_clausula_tb_documento_sindical_id",
                table: "ia_clausula_tb",
                column: "documento_sindical_id");

            migrationBuilder.CreateIndex(
                name: "IX_ia_clausula_tb_estrutura_clausula_id",
                table: "ia_clausula_tb",
                column: "estrutura_clausula_id");

            migrationBuilder.CreateIndex(
                name: "IX_ia_clausula_tb_sinonimo_id",
                table: "ia_clausula_tb",
                column: "sinonimo_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ia_clausula_tb");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ia_documento_sindical_tb",
                table: "ia_documento_sindical_tb");

            migrationBuilder.RenameTable(
                name: "ia_documento_sindical_tb",
                newName: "documento_sindical_tb");

            migrationBuilder.RenameIndex(
                name: "IX_ia_documento_sindical_tb_documento_referencia_id",
                table: "documento_sindical_tb",
                newName: "IX_documento_sindical_tb_documento_referencia_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_documento_sindical_tb",
                table: "documento_sindical_tb",
                column: "id");
        }
    }
}
