using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ineditta.Repository.Migrations
{
    /// <inheritdoc />
    public partial class v159 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"ALTER TABLE clausula_geral MODIFY COLUMN responsavel_processamento int NULL;");

            migrationBuilder.AlterTable(
                name: "clausula_geral")
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.AlterColumn<int>(
                name: "sequencia",
                table: "clausula_geral_estrutura_clausula",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "nome_informacao",
                table: "clausula_geral_estrutura_clausula",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "id_info_tipo_grupo",
                table: "clausula_geral_estrutura_clausula",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "grupo_dados",
                table: "clausula_geral_estrutura_clausula",
                type: "int",
                maxLength: 2,
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "varchar(2)",
                oldMaxLength: 2,
                oldNullable: true)
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.AlterColumn<int>(
                name: "ad_tipoinformacaoadicional_cdtipoinformacaoadicional",
                table: "clausula_geral_estrutura_clausula",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "tex_clau",
                table: "clausula_geral",
                type: "longtext",
                nullable: false,
                collation: "utf8mb4_0900_ai_ci",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldCollation: "utf8mb3_general_ci")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb3");

            migrationBuilder.AlterColumn<string>(
                name: "liberado",
                table: "clausula_geral",
                type: "varchar(2)",
                maxLength: 2,
                nullable: true,
                collation: "utf8mb4_0900_ai_ci",
                oldClrType: typeof(string),
                oldType: "varchar(2)",
                oldMaxLength: 2,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "aprovado",
                table: "clausula_geral",
                type: "varchar(45)",
                maxLength: 45,
                nullable: true,
                defaultValueSql: "'nao'",
                collation: "utf8mb4_0900_ai_ci",
                oldClrType: typeof(string),
                oldType: "varchar(45)",
                oldMaxLength: 45,
                oldNullable: true,
                oldDefaultValueSql: "'nao'")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_clausula_geral_estrutura_clausula_doc_sind_id_doc",
                table: "clausula_geral_estrutura_clausula",
                column: "doc_sind_id_doc");

            migrationBuilder.CreateIndex(
                name: "IX_clausula_geral_estrutura_clausula_id_info_tipo_grupo",
                table: "clausula_geral_estrutura_clausula",
                column: "id_info_tipo_grupo");

            migrationBuilder.CreateIndex(
                name: "IX_clausula_geral_estrutura_clausula_nome_informacao",
                table: "clausula_geral_estrutura_clausula",
                column: "nome_informacao");

            migrationBuilder.CreateIndex(
                name: "IX_clausula_geral_assunto_idassunto",
                table: "clausula_geral",
                column: "assunto_idassunto");

            migrationBuilder.CreateIndex(
                name: "IX_clausula_geral_estrutura_id_estruturaclausula",
                table: "clausula_geral",
                column: "estrutura_id_estruturaclausula");

            migrationBuilder.CreateIndex(
                name: "IX_clausula_geral_responsavel_processamento",
                table: "clausula_geral",
                column: "responsavel_processamento");

            migrationBuilder.CreateIndex(
                name: "IX_clausula_geral_sinonimo_id",
                table: "clausula_geral",
                column: "sinonimo_id");

            migrationBuilder.AddForeignKey(
                name: "fk_clausula_geral_x_assunto",
                table: "clausula_geral",
                column: "assunto_idassunto",
                principalTable: "assunto",
                principalColumn: "idassunto");

            migrationBuilder.AddForeignKey(
                name: "fk_clausula_geral_x_doc_sind",
                table: "clausula_geral",
                column: "doc_sind_id_documento",
                principalTable: "doc_sind",
                principalColumn: "id_doc",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_clausula_geral_x_estrutura_clausula",
                table: "clausula_geral",
                column: "estrutura_id_estruturaclausula",
                principalTable: "estrutura_clausula",
                principalColumn: "id_estruturaclausula",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_clausula_geral_x_sinonimos",
                table: "clausula_geral",
                column: "sinonimo_id",
                principalTable: "sinonimos",
                principalColumn: "id_sinonimo");

            migrationBuilder.AddForeignKey(
                name: "fk_clausula_geral_x_usuario_adm",
                table: "clausula_geral",
                column: "responsavel_processamento",
                principalTable: "usuario_adm",
                principalColumn: "id_user");

            migrationBuilder.AddForeignKey(
                name: "fk_clausula_geral_estrutura_clausula_x_ad_tipoinformacaoadicional",
                table: "clausula_geral_estrutura_clausula",
                column: "ad_tipoinformacaoadicional_cdtipoinformacaoadicional",
                principalTable: "ad_tipoinformacaoadicional",
                principalColumn: "cdtipoinformacaoadicional",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_clausula_geral_estrutura_clausula_x_clausula_geral",
                table: "clausula_geral_estrutura_clausula",
                column: "clausula_geral_id_clau",
                principalTable: "clausula_geral",
                principalColumn: "id_clau",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_clausula_geral_estrutura_clausula_x_doc_sind",
                table: "clausula_geral_estrutura_clausula",
                column: "doc_sind_id_doc",
                principalTable: "doc_sind",
                principalColumn: "id_doc",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_clausula_geral_estrutura_clausula_x_estrutura_clausula",
                table: "clausula_geral_estrutura_clausula",
                column: "estrutura_clausula_id_estruturaclausula",
                principalTable: "estrutura_clausula",
                principalColumn: "id_estruturaclausula",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.Sql(@"ALTER TABLE clausula_geral_estrutura_clausula
                    ADD CONSTRAINT fk_clausula_geral_estrutura_clausula_x_info_grupo
                    FOREIGN KEY (id_info_tipo_grupo)
                    REFERENCES ad_tipoinformacaoadicional(cdtipoinformacaoadicional)
                    ON DELETE CASCADE;");

            migrationBuilder.AddForeignKey(
                name: "fk_clausula_geral_estrutura_clausula_x_nome_estrutura_clausula",
                table: "clausula_geral_estrutura_clausula",
                column: "nome_informacao",
                principalTable: "estrutura_clausula",
                principalColumn: "id_estruturaclausula",
                onDelete: ReferentialAction.Cascade);        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_clausula_geral_x_assunto",
                table: "clausula_geral");

            migrationBuilder.DropForeignKey(
                name: "fk_clausula_geral_x_doc_sind",
                table: "clausula_geral");

            migrationBuilder.DropForeignKey(
                name: "fk_clausula_geral_x_estrutura_clausula",
                table: "clausula_geral");

            migrationBuilder.DropForeignKey(
                name: "fk_clausula_geral_x_sinonimos",
                table: "clausula_geral");

            migrationBuilder.DropForeignKey(
                name: "fk_clausula_geral_x_usuario_adm",
                table: "clausula_geral");

            migrationBuilder.DropForeignKey(
                name: "fk_clausula_geral_estrutura_clausula_x_ad_tipoinformacaoadicional",
                table: "clausula_geral_estrutura_clausula");

            migrationBuilder.DropForeignKey(
                name: "fk_clausula_geral_estrutura_clausula_x_clausula_geral",
                table: "clausula_geral_estrutura_clausula");

            migrationBuilder.DropForeignKey(
                name: "fk_clausula_geral_estrutura_clausula_x_doc_sind",
                table: "clausula_geral_estrutura_clausula");

            migrationBuilder.DropForeignKey(
                name: "fk_clausula_geral_estrutura_clausula_x_estrutura_clausula",
                table: "clausula_geral_estrutura_clausula");

            migrationBuilder.DropForeignKey(
                name: "fk_clausula_geral_estrutura_clausula_x_informacao_adicional_grupo",
                table: "clausula_geral_estrutura_clausula");

            migrationBuilder.DropForeignKey(
                name: "fk_clausula_geral_estrutura_clausula_x_nome_estrutura_clausula",
                table: "clausula_geral_estrutura_clausula");

            migrationBuilder.AlterTable(
                name: "clausula_geral")
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_general_ci")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.AlterColumn<int>(
                name: "sequencia",
                table: "clausula_geral_estrutura_clausula",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "nome_informacao",
                table: "clausula_geral_estrutura_clausula",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "id_info_tipo_grupo",
                table: "clausula_geral_estrutura_clausula",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "grupo_dados",
                table: "clausula_geral_estrutura_clausula",
                type: "varchar(2)",
                maxLength: 2,
                nullable: true,
                collation: "utf8mb4_0900_ai_ci",
                oldClrType: typeof(int),
                oldType: "int",
                oldMaxLength: 2)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "ad_tipoinformacaoadicional_cdtipoinformacaoadicional",
                table: "clausula_geral_estrutura_clausula",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "tex_clau",
                table: "clausula_geral",
                type: "longtext",
                nullable: false,
                collation: "utf8mb3_general_ci",
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb3")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.AlterColumn<string>(
                name: "liberado",
                table: "clausula_geral",
                type: "varchar(2)",
                maxLength: 2,
                nullable: true,
                collation: "utf8mb4_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(2)",
                oldMaxLength: 2,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.AlterColumn<string>(
                name: "aprovado",
                table: "clausula_geral",
                type: "varchar(45)",
                maxLength: 45,
                nullable: true,
                defaultValueSql: "'nao'",
                collation: "utf8mb4_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(45)",
                oldMaxLength: 45,
                oldNullable: true,
                oldDefaultValueSql: "'nao'")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "utf8mb4_0900_ai_ci");
        }
    }
}
