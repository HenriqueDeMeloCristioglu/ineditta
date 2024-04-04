using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ineditta.Repository.Migrations
{
    /// <inheritdoc />
    public partial class v176 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE estrutura_clausula DROP FOREIGN KEY estrutura_clausula_ibfk_1;");

            migrationBuilder.Sql("ALTER TABLE estrutura_clausulas_ad_tipoinformacaoadicional DROP FOREIGN KEY estrutura_clausulas_ad_tipoinformacaoadicional_ibfk_1;");

            migrationBuilder.DropColumn(
                name: "cor",
                table: "estrutura_clausula");

            migrationBuilder.DropColumn(
                name: "dadocl_ms",
                table: "estrutura_clausula");

            migrationBuilder.DropColumn(
                name: "info_adicional",
                table: "estrutura_clausula");

            migrationBuilder.DropColumn(
                name: "nome_grupo",
                table: "estrutura_clausula");

            migrationBuilder.RenameIndex(
                name: "grupo_clausula_idgrupo_clausula",
                table: "estrutura_clausula",
                newName: "IX_estrutura_clausula_grupo_clausula_idgrupo_clausula");

            migrationBuilder.AlterTable(
                name: "estrutura_clausula")
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci")
                .OldAnnotation("MySql:CharSet", "utf8mb3")
                .OldAnnotation("Relational:Collation", "utf8mb3_general_ci");

            migrationBuilder.UpdateData(
                table: "estrutura_clausula",
                keyColumn: "tipo_clausula",
                keyValue: null,
                column: "tipo_clausula",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "tipo_clausula",
                table: "estrutura_clausula",
                type: "varchar(2)",
                maxLength: 2,
                nullable: false,
                collation: "utf8mb4_0900_ai_ci",
                oldClrType: typeof(string),
                oldType: "varchar(4)",
                oldMaxLength: 4,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb3")
                .OldAnnotation("Relational:Collation", "utf8mb3_general_ci");

            migrationBuilder.UpdateData(
                table: "estrutura_clausula",
                keyColumn: "tarefa",
                keyValue: null,
                column: "tarefa",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "tarefa",
                table: "estrutura_clausula",
                type: "varchar(2)",
                maxLength: 2,
                nullable: false,
                collation: "utf8mb4_0900_ai_ci",
                oldClrType: typeof(string),
                oldType: "varchar(2)",
                oldMaxLength: 2,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb3")
                .OldAnnotation("Relational:Collation", "utf8mb3_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "nome_clausula",
                table: "estrutura_clausula",
                type: "varchar(80)",
                maxLength: 80,
                nullable: false,
                collation: "utf8mb4_0900_ai_ci",
                oldClrType: typeof(string),
                oldType: "varchar(80)",
                oldMaxLength: 80)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb3")
                .OldAnnotation("Relational:Collation", "utf8mb3_general_ci");

            migrationBuilder.AlterColumn<int>(
                name: "grupo_clausula_idgrupo_clausula",
                table: "estrutura_clausula",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "estrutura_clausula",
                keyColumn: "classe_clausula",
                keyValue: null,
                column: "classe_clausula",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "classe_clausula",
                table: "estrutura_clausula",
                type: "varchar(1)",
                maxLength: 1,
                nullable: false,
                collation: "utf8mb4_0900_ai_ci",
                oldClrType: typeof(string),
                oldType: "varchar(2)",
                oldMaxLength: 2,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb3")
                .OldAnnotation("Relational:Collation", "utf8mb3_general_ci");

            migrationBuilder.UpdateData(
                table: "estrutura_clausula",
                keyColumn: "calendario",
                keyValue: null,
                column: "calendario",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "calendario",
                table: "estrutura_clausula",
                type: "varchar(1)",
                maxLength: 1,
                nullable: false,
                collation: "utf8mb4_0900_ai_ci",
                oldClrType: typeof(string),
                oldType: "varchar(2)",
                oldMaxLength: 2,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb3")
                .OldAnnotation("Relational:Collation", "utf8mb3_general_ci");

            migrationBuilder.AddForeignKey(
                name: "fk_estrutura_clausula_x_grupo_clausula",
                table: "estrutura_clausula",
                column: "grupo_clausula_idgrupo_clausula",
                principalTable: "grupo_clausula",
                principalColumn: "idgrupo_clausula",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_estrutura_clausula_x_grupo_clausula",
                table: "estrutura_clausula");

            migrationBuilder.RenameIndex(
                name: "IX_estrutura_clausula_grupo_clausula_idgrupo_clausula",
                table: "estrutura_clausula",
                newName: "grupo_clausula_idgrupo_clausula");

            migrationBuilder.AlterTable(
                name: "estrutura_clausula")
                .Annotation("MySql:CharSet", "utf8mb3")
                .Annotation("Relational:Collation", "utf8mb3_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.AlterColumn<string>(
                name: "tipo_clausula",
                table: "estrutura_clausula",
                type: "varchar(4)",
                maxLength: 4,
                nullable: true,
                collation: "utf8mb3_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(2)",
                oldMaxLength: 2)
                .Annotation("MySql:CharSet", "utf8mb3")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.AlterColumn<string>(
                name: "tarefa",
                table: "estrutura_clausula",
                type: "varchar(2)",
                maxLength: 2,
                nullable: true,
                collation: "utf8mb3_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(2)",
                oldMaxLength: 2)
                .Annotation("MySql:CharSet", "utf8mb3")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.AlterColumn<string>(
                name: "nome_clausula",
                table: "estrutura_clausula",
                type: "varchar(80)",
                maxLength: 80,
                nullable: false,
                collation: "utf8mb3_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(80)",
                oldMaxLength: 80)
                .Annotation("MySql:CharSet", "utf8mb3")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.AlterColumn<int>(
                name: "grupo_clausula_idgrupo_clausula",
                table: "estrutura_clausula",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "classe_clausula",
                table: "estrutura_clausula",
                type: "varchar(2)",
                maxLength: 2,
                nullable: true,
                collation: "utf8mb3_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(1)",
                oldMaxLength: 1)
                .Annotation("MySql:CharSet", "utf8mb3")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.AlterColumn<string>(
                name: "calendario",
                table: "estrutura_clausula",
                type: "varchar(2)",
                maxLength: 2,
                nullable: true,
                collation: "utf8mb3_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(1)",
                oldMaxLength: 1)
                .Annotation("MySql:CharSet", "utf8mb3")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.AddColumn<string>(
                name: "cor",
                table: "estrutura_clausula",
                type: "varchar(15)",
                maxLength: 15,
                nullable: true,
                collation: "utf8mb3_general_ci")
                .Annotation("MySql:CharSet", "utf8mb3");

            migrationBuilder.AddColumn<string>(
                name: "dadocl_ms",
                table: "estrutura_clausula",
                type: "varchar(2)",
                maxLength: 2,
                nullable: true,
                collation: "utf8mb3_general_ci")
                .Annotation("MySql:CharSet", "utf8mb3");

            migrationBuilder.AddColumn<string>(
                name: "info_adicional",
                table: "estrutura_clausula",
                type: "json",
                nullable: true,
                collation: "utf8mb3_general_ci")
                .Annotation("MySql:CharSet", "utf8mb3");

            migrationBuilder.AddColumn<string>(
                name: "nome_grupo",
                table: "estrutura_clausula",
                type: "varchar(80)",
                maxLength: 80,
                nullable: true,
                collation: "utf8mb3_general_ci")
                .Annotation("MySql:CharSet", "utf8mb3");

            migrationBuilder.AddForeignKey(
                name: "ibfk_doc_assunto",
                table: "documento_assunto",
                column: "estrutura_clausula_id_estruturaclausula",
                principalTable: "estrutura_clausula",
                principalColumn: "id_estruturaclausula");

            migrationBuilder.AddForeignKey(
                name: "estrutura_clausula_ibfk_1",
                table: "estrutura_clausula",
                column: "grupo_clausula_idgrupo_clausula",
                principalTable: "grupo_clausula",
                principalColumn: "idgrupo_clausula");

            migrationBuilder.AddForeignKey(
                name: "estrutura_clausulas_ad_tipoinformacaoadicional_ibfk_1",
                table: "estrutura_clausulas_ad_tipoinformacaoadicional",
                column: "estrutura_clausula_id_estruturaclausula",
                principalTable: "estrutura_clausula",
                principalColumn: "id_estruturaclausula");
        }
    }
}
