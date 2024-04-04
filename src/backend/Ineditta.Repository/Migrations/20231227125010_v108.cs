using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ineditta.Repository.Migrations
{
    /// <inheritdoc />
    public partial class v108 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "tipo_doc",
                keyColumn: "processado",
                keyValue: null,
                column: "processado",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "processado",
                table: "tipo_doc",
                type: "varchar(3)",
                maxLength: 3,
                nullable: false,
                collation: "utf8mb3_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(3)",
                oldMaxLength: 3,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb3")
                .OldAnnotation("MySql:CharSet", "utf8mb3")
                .OldAnnotation("Relational:Collation", "utf8mb3_general_ci");

            migrationBuilder.UpdateData(
                table: "tipo_doc",
                keyColumn: "permissao",
                keyValue: null,
                column: "permissao",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "permissao",
                table: "tipo_doc",
                type: "varchar(3)",
                maxLength: 3,
                nullable: false,
                collation: "utf8mb3_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(3)",
                oldMaxLength: 3,
                oldNullable: true,
                oldDefaultValueSql: "'não'")
                .Annotation("MySql:CharSet", "utf8mb3")
                .OldAnnotation("MySql:CharSet", "utf8mb3")
                .OldAnnotation("Relational:Collation", "utf8mb3_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "uri",
                table: "modulos",
                type: "varchar(45)",
                maxLength: 45,
                nullable: true,
                collation: "utf8mb3_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(45)",
                oldMaxLength: 45)
                .Annotation("MySql:CharSet", "utf8mb3")
                .OldAnnotation("MySql:CharSet", "utf8mb3")
                .OldAnnotation("Relational:Collation", "utf8mb3_general_ci");

            migrationBuilder.UpdateData(
                table: "modulos",
                keyColumn: "tipo",
                keyValue: null,
                column: "tipo",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "tipo",
                table: "modulos",
                type: "varchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValueSql: "'Comercial'",
                collation: "utf8mb3_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(20)",
                oldMaxLength: 20,
                oldNullable: true,
                oldDefaultValueSql: "'Comercial'")
                .Annotation("MySql:CharSet", "utf8mb3")
                .OldAnnotation("MySql:CharSet", "utf8mb3")
                .OldAnnotation("Relational:Collation", "utf8mb3_general_ci");

            migrationBuilder.UpdateData(
                table: "modulos",
                keyColumn: "excluir",
                keyValue: null,
                column: "excluir",
                value: "N");

            migrationBuilder.AlterColumn<string>(
                name: "excluir",
                table: "modulos",
                type: "varchar(2)",
                maxLength: 2,
                nullable: false,
                collation: "utf8mb3_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(2)",
                oldMaxLength: 2,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb3")
                .OldAnnotation("MySql:CharSet", "utf8mb3")
                .OldAnnotation("Relational:Collation", "utf8mb3_general_ci");

            migrationBuilder.UpdateData(
                table: "modulos",
                keyColumn: "criar",
                keyValue: null,
                column: "criar",
                value: "N");

            migrationBuilder.AlterColumn<string>(
                name: "criar",
                table: "modulos",
                type: "varchar(2)",
                maxLength: 2,
                nullable: false,
                collation: "utf8mb3_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(2)",
                oldMaxLength: 2,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb3")
                .OldAnnotation("MySql:CharSet", "utf8mb3")
                .OldAnnotation("Relational:Collation", "utf8mb3_general_ci");

            migrationBuilder.UpdateData(
                table: "modulos",
                keyColumn: "consultar",
                keyValue: null,
                column: "consultar",
                value: "N");

            migrationBuilder.AlterColumn<string>(
                name: "consultar",
                table: "modulos",
                type: "varchar(2)",
                maxLength: 2,
                nullable: false,
                collation: "utf8mb3_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(2)",
                oldMaxLength: 2,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb3")
                .OldAnnotation("MySql:CharSet", "utf8mb3")
                .OldAnnotation("Relational:Collation", "utf8mb3_general_ci");

            migrationBuilder.UpdateData(
                table: "modulos",
                keyColumn: "comentar",
                keyValue: null,
                column: "comentar",
                value: "N");

            migrationBuilder.AlterColumn<string>(
                name: "comentar",
                table: "modulos",
                type: "varchar(2)",
                maxLength: 2,
                nullable: false,
                collation: "utf8mb3_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(2)",
                oldMaxLength: 2,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb3")
                .OldAnnotation("MySql:CharSet", "utf8mb3")
                .OldAnnotation("Relational:Collation", "utf8mb3_general_ci");

            migrationBuilder.UpdateData(
                table: "modulos",
                keyColumn: "aprovar",
                keyValue: null,
                column: "aprovar",
                value: "N");

            migrationBuilder.AlterColumn<string>(
                name: "aprovar",
                table: "modulos",
                type: "varchar(2)",
                maxLength: 2,
                nullable: false,
                collation: "utf8mb3_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(2)",
                oldMaxLength: 2,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb3")
                .OldAnnotation("MySql:CharSet", "utf8mb3")
                .OldAnnotation("Relational:Collation", "utf8mb3_general_ci");

            migrationBuilder.UpdateData(
                table: "modulos",
                keyColumn: "alterar",
                keyValue: null,
                column: "alterar",
                value: "N");

            migrationBuilder.AlterColumn<string>(
                name: "alterar",
                table: "modulos",
                type: "varchar(2)",
                maxLength: 2,
                nullable: false,
                collation: "utf8mb3_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(2)",
                oldMaxLength: 2,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb3")
                .OldAnnotation("MySql:CharSet", "utf8mb3")
                .OldAnnotation("Relational:Collation", "utf8mb3_general_ci");

            migrationBuilder.Sql(@"DROP TABLE IF EXISTS tipo_documento_cliente_matriz;");

            migrationBuilder.CreateTable(
                name: "tipo_documento_cliente_matriz",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    tipo_documento_id = table.Column<int>(type: "int", nullable: false),
                    cliente_matriz_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "tipo_documento_cliente_matriz_ibfk_2",
                        column: x => x.tipo_documento_id,
                        principalTable: "tipo_doc",
                        principalColumn: "idtipo_doc",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "tipo_documento_matriz_cliente_ibfk_1",
                        column: x => x.cliente_matriz_id,
                        principalTable: "cliente_matriz",
                        principalColumn: "id_empresa",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb3")
                .Annotation("Relational:Collation", "utf8mb3_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_tipo_documento_cliente_matriz_cliente_matriz_id",
                table: "tipo_documento_cliente_matriz",
                column: "cliente_matriz_id");

            migrationBuilder.CreateIndex(
                name: "IX_tipo_documento_cliente_matriz_tipo_documento_id",
                table: "tipo_documento_cliente_matriz",
                column: "tipo_documento_id");

            migrationBuilder.Sql(@"
                INSERT INTO tipo_documento_cliente_matriz (tipo_documento_id, cliente_matriz_id)
                SELECT 
	                5 AS tipo_documento_id,
	                cm.id_empresa AS cliente_matriz_id
                FROM cliente_matriz cm
                WHERE cm.tip_doc LIKE '%ACT%'; 

                INSERT INTO tipo_documento_cliente_matriz (tipo_documento_id, cliente_matriz_id)
                SELECT 
	                6 AS tipo_documento_id,
	                cm.id_empresa AS cliente_matriz_id
                FROM cliente_matriz cm
                WHERE cm.tip_doc LIKE '%CCT%';
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tipo_documento_cliente_matriz");

            migrationBuilder.AlterColumn<string>(
                name: "processado",
                table: "tipo_doc",
                type: "varchar(3)",
                maxLength: 3,
                nullable: true,
                collation: "utf8mb3_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(3)",
                oldMaxLength: 3)
                .Annotation("MySql:CharSet", "utf8mb3")
                .OldAnnotation("MySql:CharSet", "utf8mb3")
                .OldAnnotation("Relational:Collation", "utf8mb3_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "permissao",
                table: "tipo_doc",
                type: "varchar(3)",
                maxLength: 3,
                nullable: true,
                defaultValueSql: "'não'",
                collation: "utf8mb3_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(3)",
                oldMaxLength: 3)
                .Annotation("MySql:CharSet", "utf8mb3")
                .OldAnnotation("MySql:CharSet", "utf8mb3")
                .OldAnnotation("Relational:Collation", "utf8mb3_general_ci");

            migrationBuilder.UpdateData(
                table: "modulos",
                keyColumn: "uri",
                keyValue: null,
                column: "uri",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "uri",
                table: "modulos",
                type: "varchar(45)",
                maxLength: 45,
                nullable: false,
                collation: "utf8mb3_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(45)",
                oldMaxLength: 45,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb3")
                .OldAnnotation("MySql:CharSet", "utf8mb3")
                .OldAnnotation("Relational:Collation", "utf8mb3_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "tipo",
                table: "modulos",
                type: "varchar(20)",
                maxLength: 20,
                nullable: true,
                defaultValueSql: "'Comercial'",
                collation: "utf8mb3_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(20)",
                oldMaxLength: 20,
                oldDefaultValueSql: "'Comercial'")
                .Annotation("MySql:CharSet", "utf8mb3")
                .OldAnnotation("MySql:CharSet", "utf8mb3")
                .OldAnnotation("Relational:Collation", "utf8mb3_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "excluir",
                table: "modulos",
                type: "varchar(2)",
                maxLength: 2,
                nullable: true,
                collation: "utf8mb3_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(2)",
                oldMaxLength: 2)
                .Annotation("MySql:CharSet", "utf8mb3")
                .OldAnnotation("MySql:CharSet", "utf8mb3")
                .OldAnnotation("Relational:Collation", "utf8mb3_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "criar",
                table: "modulos",
                type: "varchar(2)",
                maxLength: 2,
                nullable: true,
                collation: "utf8mb3_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(2)",
                oldMaxLength: 2)
                .Annotation("MySql:CharSet", "utf8mb3")
                .OldAnnotation("MySql:CharSet", "utf8mb3")
                .OldAnnotation("Relational:Collation", "utf8mb3_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "consultar",
                table: "modulos",
                type: "varchar(2)",
                maxLength: 2,
                nullable: true,
                collation: "utf8mb3_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(2)",
                oldMaxLength: 2)
                .Annotation("MySql:CharSet", "utf8mb3")
                .OldAnnotation("MySql:CharSet", "utf8mb3")
                .OldAnnotation("Relational:Collation", "utf8mb3_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "comentar",
                table: "modulos",
                type: "varchar(2)",
                maxLength: 2,
                nullable: true,
                collation: "utf8mb3_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(2)",
                oldMaxLength: 2)
                .Annotation("MySql:CharSet", "utf8mb3")
                .OldAnnotation("MySql:CharSet", "utf8mb3")
                .OldAnnotation("Relational:Collation", "utf8mb3_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "aprovar",
                table: "modulos",
                type: "varchar(2)",
                maxLength: 2,
                nullable: true,
                collation: "utf8mb3_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(2)",
                oldMaxLength: 2)
                .Annotation("MySql:CharSet", "utf8mb3")
                .OldAnnotation("MySql:CharSet", "utf8mb3")
                .OldAnnotation("Relational:Collation", "utf8mb3_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "alterar",
                table: "modulos",
                type: "varchar(2)",
                maxLength: 2,
                nullable: true,
                collation: "utf8mb3_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(2)",
                oldMaxLength: 2)
                .Annotation("MySql:CharSet", "utf8mb3")
                .OldAnnotation("MySql:CharSet", "utf8mb3")
                .OldAnnotation("Relational:Collation", "utf8mb3_general_ci");
        }
    }
}
