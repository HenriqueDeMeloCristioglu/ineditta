using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ineditta.Repository.Migrations
{
    /// <inheritdoc />
    public partial class v201 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "documento_estabelecimento_tb",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    documento_id = table.Column<int>(type: "int", nullable: false),
                    estabelecimento_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "documento_estabelecimento_tb_fk_1",
                        column: x => x.documento_id,
                        principalTable: "doc_sind",
                        principalColumn: "id_doc",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "documento_estabelecimento_tb_fk_2",
                        column: x => x.estabelecimento_id,
                        principalTable: "cliente_unidades",
                        principalColumn: "id_unidade",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateIndex(
                name: "IX_documento_estabelecimento_tb_documento_id",
                table: "documento_estabelecimento_tb",
                column: "documento_id");

            migrationBuilder.CreateIndex(
                name: "IX_documento_estabelecimento_tb_estabelecimento_id",
                table: "documento_estabelecimento_tb",
                column: "estabelecimento_id");

            migrationBuilder.Sql(@"
                CREATE PROCEDURE definir_relacionamento_documentos_estabelecimentos()
                BEGIN
                    INSERT INTO documento_estabelecimento_tb (documento_id, estabelecimento_id)
                    SELECT 
	                    ds.id_doc AS documento_id, 
	                    cus.unidade_id AS estabelecimento_id 
                    FROM doc_sind ds,
                    json_table(ds.cliente_estabelecimento, '$[*].u' columns (unidade_id int path '$')) as cus;
                END;       
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "documento_estabelecimento_tb");
        }
    }
}
