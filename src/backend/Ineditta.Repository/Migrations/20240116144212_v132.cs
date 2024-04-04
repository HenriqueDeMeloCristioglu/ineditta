using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ineditta.Repository.Migrations
{
    /// <inheritdoc />
    public partial class v132 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_acompanhanto_cct_x_acompanhamento_cct_status",
                table: "acompanhanto_cct");

            migrationBuilder.DropPrimaryKey(
                name: "PK_acompanhamento_cct_status",
                table: "acompanhamento_cct_status");

            migrationBuilder.RenameTable(
                name: "acompanhanto_cct",
                newName: "acompanhamento_cct");

            migrationBuilder.RenameTable(
                name: "acompanhamento_cct_status",
                newName: "acompanhamento_cct_status_tb");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "acompanhamento_cct_sindicato_patronal_tb",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "acompanhamento_cct_sindicato_laboral_tb",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "acompanhamento_cct_estabelecimento_tb",
                newName: "id");

            migrationBuilder.RenameIndex(
                name: "IX_acompanhanto_cct_status",
                table: "acompanhamento_cct",
                newName: "IX_acompanhamento_cct_status");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "acompanhamento_cct_status_tb",
                newName: "id");

            migrationBuilder.AlterColumn<string>(
                name: "descricao",
                table: "acompanhamento_cct_status_tb",
                type: "varchar(250)",
                maxLength: 250,
                nullable: false,
                collation: "utf8mb4_0900_ai_ci",
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.AddPrimaryKey(
                name: "PK_acompanhamento_cct_status_tb",
                table: "acompanhamento_cct_status_tb",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_acompanhanto_cct_x_acompanhamento_cct_status",
                table: "acompanhamento_cct",
                column: "status",
                principalTable: "acompanhamento_cct_status_tb",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_acompanhamento_cct_status_tb",
                table: "acompanhamento_cct_status_tb");

            migrationBuilder.RenameTable(
                name: "acompanhamento_cct_status_tb",
                newName: "acompanhamento_cct_status");

            migrationBuilder.RenameTable(
                name: "acompanhamento_cct",
                newName: "acompanhanto_cct");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "acompanhamento_cct_sindicato_patronal_tb",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "acompanhamento_cct_sindicato_laboral_tb",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "acompanhamento_cct_estabelecimento_tb",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "acompanhamento_cct_status",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_acompanhamento_cct_status",
                table: "acompanhanto_cct",
                newName: "IX_acompanhanto_cct_status");

            migrationBuilder.AlterColumn<string>(
                name: "descricao",
                table: "acompanhamento_cct_status",
                type: "longtext",
                nullable: false,
                collation: "utf8mb4_0900_ai_ci",
                oldClrType: typeof(string),
                oldType: "varchar(250)",
                oldMaxLength: 250)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.AddPrimaryKey(
                name: "PK_acompanhamento_cct_status",
                table: "acompanhamento_cct_status",
                column: "Id");
        }
    }
}
