using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ineditta.Repository.Migrations
{
    /// <inheritdoc />
    public partial class v129 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "documento_sindicato_laboral_tb",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    documento_id = table.Column<int>(type: "int", nullable: false),
                    sindicato_laboral_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "documento_sindicato_laboral_tb_ibfk_1",
                        column: x => x.documento_id,
                        principalTable: "doc_sind",
                        principalColumn: "id_doc",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "documento_sindicato_laboral_tb_ibfk_2",
                        column: x => x.sindicato_laboral_id,
                        principalTable: "sind_emp",
                        principalColumn: "id_sinde",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "documento_sindicato_patronal_tb",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    documento_id = table.Column<int>(type: "int", nullable: false),
                    sindicato_patronal_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "documento_sindicato_patronal_tb_ibfk_1",
                        column: x => x.documento_id,
                        principalTable: "doc_sind",
                        principalColumn: "id_doc",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "documento_sindicato_patronal_tb_ibfk_2",
                        column: x => x.sindicato_patronal_id,
                        principalTable: "sind_patr",
                        principalColumn: "id_sindp",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateIndex(
                name: "IX_documento_sindicato_laboral_tb_documento_id",
                table: "documento_sindicato_laboral_tb",
                column: "documento_id");

            migrationBuilder.CreateIndex(
                name: "IX_documento_sindicato_laboral_tb_sindicato_laboral_id",
                table: "documento_sindicato_laboral_tb",
                column: "sindicato_laboral_id");

            migrationBuilder.CreateIndex(
                name: "IX_documento_sindicato_patronal_tb_documento_id",
                table: "documento_sindicato_patronal_tb",
                column: "documento_id");

            migrationBuilder.CreateIndex(
                name: "IX_documento_sindicato_patronal_tb_sindicato_patronal_id",
                table: "documento_sindicato_patronal_tb",
                column: "sindicato_patronal_id");

            migrationBuilder.Sql(@"DROP PROCEDURE IF EXISTS inserir_documento_sindicatos;");

            migrationBuilder.Sql(@"
                CREATE PROCEDURE inserir_documento_sindicatos()
                BEGIN
                    INSERT INTO documento_sindicato_patronal_tb (documento_id, sindicato_patronal_id)
                    SELECT 
                        ds.id_doc documento_id,
                        jt.id sindicato_patronal_id
                    FROM 
                        doc_sind ds,
                        JSON_TABLE(ds.sind_patronal, '$[*].id' COLUMNS (id INT PATH '$')) AS jt;

                    INSERT INTO documento_sindicato_laboral_tb (documento_id, sindicato_laboral_id)
                    SELECT 
                        ds.id_doc documento_id,
                        jt.id sindicato_laboral_id
                    FROM 
                        doc_sind ds,
                        JSON_TABLE(ds.sind_laboral, '$[*].id' COLUMNS (id INT PATH '$')) AS jt;
                END;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "documento_sindicato_laboral_tb");

            migrationBuilder.DropTable(
                name: "documento_sindicato_patronal_tb");

            migrationBuilder.Sql(@"DROP PROCEDURE IF EXISTS inserir_documento_sindicatos;");
        }
    }
}
