using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ineditta.Repository.Migrations
{
    /// <inheritdoc />
    public partial class v123 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "subtipo_evento",
                table: "calendario_sindical_tb",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_calendario_sindical_tb_subtipo_evento",
                table: "calendario_sindical_tb",
                column: "subtipo_evento");

            migrationBuilder.CreateIndex(
                name: "IX_calendario_sindical_tb_tipo_evento",
                table: "calendario_sindical_tb",
                column: "tipo_evento");

            migrationBuilder.AddForeignKey(
                name: "calendario_sindical_tb_ibfk_1",
                table: "calendario_sindical_tb",
                column: "tipo_evento",
                principalTable: "tipo_evento_calendario_sindical",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "calendario_sindical_tb_ibfk_2",
                table: "calendario_sindical_tb",
                column: "subtipo_evento",
                principalTable: "subtipo_evento_calendario_sindical",
                principalColumn: "id");

            migrationBuilder.Sql(@"
                UPDATE calendario_sindical_tb cst
                LEFT JOIN clausula_geral_estrutura_clausula cgec ON cgec.id_clausulageral_estrutura_clausula = cst.chave_referencia
                LEFT JOIN ad_tipoinformacaoadicional at2 ON cgec.ad_tipoinformacaoadicional_cdtipoinformacaoadicional = at2.cdtipoinformacaoadicional 
                LEFT JOIN estrutura_clausula ec ON ec.id_estruturaclausula = cgec.estrutura_clausula_id_estruturaclausula 
                LEFT JOIN grupo_clausula gc ON gc.idgrupo_clausula = ec.grupo_clausula_idgrupo_clausula 
                SET cst.subtipo_evento = (SELECT id FROM subtipo_evento_calendario_sindical st
						                  WHERE st.nome = CONCAT(gc.nome_grupo, ' - ', at2.nmtipoinformacaoadicional)) 
                WHERE cst.tipo_evento = 5;
            ");

            migrationBuilder.Sql(@"
                DELETE FROM calendario_sindical_tb cst WHERE cst.tipo_evento = 5 AND cst.subtipo_evento IS NULL;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "calendario_sindical_tb_ibfk_1",
                table: "calendario_sindical_tb");

            migrationBuilder.DropForeignKey(
                name: "calendario_sindical_tb_ibfk_2",
                table: "calendario_sindical_tb");

            migrationBuilder.DropIndex(
                name: "IX_calendario_sindical_tb_subtipo_evento",
                table: "calendario_sindical_tb");

            migrationBuilder.DropIndex(
                name: "IX_calendario_sindical_tb_tipo_evento",
                table: "calendario_sindical_tb");

            migrationBuilder.DropColumn(
                name: "subtipo_evento",
                table: "calendario_sindical_tb");
        }
    }
}
