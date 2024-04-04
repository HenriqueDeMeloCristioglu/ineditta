using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ineditta.Repository.Migrations
{
    /// <inheritdoc />
    public partial class v202 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "documento_localizacao_tb",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    documento_id = table.Column<int>(type: "int", nullable: false),
                    localizacao_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "documento_localizacao_tb_fk_1",
                        column: x => x.documento_id,
                        principalTable: "doc_sind",
                        principalColumn: "id_doc",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "documento_localizacao_tb_fk_2",
                        column: x => x.localizacao_id,
                        principalTable: "localizacao",
                        principalColumn: "id_localizacao",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateIndex(
                name: "IX_documento_localizacao_tb_documento_id",
                table: "documento_localizacao_tb",
                column: "documento_id");

            migrationBuilder.CreateIndex(
                name: "IX_documento_localizacao_tb_localizacao_id",
                table: "documento_localizacao_tb",
                column: "localizacao_id");

            migrationBuilder.Sql(@"
                CREATE PROCEDURE definir_relacionamento_documentos_localizacoes()
                BEGIN
                    INSERT INTO documento_localizacao_tb (documento_id, localizacao_id)
                    SELECT 
                        ds.id_doc AS documento_id, 
                        cus.localizacao_id AS localizacao_id 
                    FROM doc_sind ds,
                    json_table(ds.abrangencia, '$[*].id' columns (localizacao_id int path '$')) as cus;
                END;       
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "documento_localizacao_tb");
        }
    }
}
