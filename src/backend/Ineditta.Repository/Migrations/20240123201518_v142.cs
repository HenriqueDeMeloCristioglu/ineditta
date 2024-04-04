using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ineditta.Repository.Migrations
{
    /// <inheritdoc />
    public partial class v142 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "sindicato_laboral_id",
                table: "acompanhamento_cct_tb");

            migrationBuilder.DropColumn(
                name: "sindicato_patronal_id",
                table: "acompanhamento_cct_tb");

            migrationBuilder.CreateTable(
                name: "acompanhamento_cct_localizacao_tb",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    acompanhamento_cct_id = table.Column<long>(type: "bigint", nullable: false),
                    localizacao_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "fk_acompanhamento_cct_localizacao_x_acompanhamento_cct",
                        column: x => x.acompanhamento_cct_id,
                        principalTable: "acompanhamento_cct_tb",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_acompanhamento_cct_localizacao_x_localizacao",
                        column: x => x.localizacao_id,
                        principalTable: "localizacao",
                        principalColumn: "id_localizacao",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateIndex(
                name: "IX_acompanhamento_cct_localizacao_tb_localizacao_id",
                table: "acompanhamento_cct_localizacao_tb",
                column: "localizacao_id");

            migrationBuilder.CreateIndex(
                name: "ix001_acompanhamento_cct_localizacao_tb",
                table: "acompanhamento_cct_localizacao_tb",
                column: "acompanhamento_cct_id");

            migrationBuilder.CreateIndex(
                name: "ix002_acompanhamento_cct_localizacao_tb",
                table: "acompanhamento_cct_localizacao_tb",
                columns: new[] { "acompanhamento_cct_id", "localizacao_id" },
                unique: true);

            migrationBuilder.Sql(@"create or replace view base_territorial_sindicato_laboral_localizacao as
                                SELECT l.id_localizacao id, se.sigla_sinde sigla, l.uf, l.municipio, l.pais, bt.sind_empregados_id_sinde1 sindicato_laboral_id from base_territorialsindemp bt
                                inner join sind_emp se ON bt.sind_empregados_id_sinde1 = se.id_sinde
                                inner join localizacao l ON bt.localizacao_id_localizacao1 = l.id_localizacao");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "acompanhamento_cct_localizacao_tb");

            migrationBuilder.AddColumn<int>(
                name: "sindicato_laboral_id",
                table: "acompanhamento_cct_tb",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "sindicato_patronal_id",
                table: "acompanhamento_cct_tb",
                type: "int",
                nullable: true);

            migrationBuilder.Sql(@"drop view if exists view base_territorial_sindicato_laboral_localizacao");
        }
    }
}
