using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ineditta.Repository.Migrations
{
    /// <inheritdoc />
    public partial class v100 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "note_cliente");

            migrationBuilder.CreateTable(
                name: "tipo_etiqueta_tb",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    nome = table.Column<string>(type: "text", nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "etiqueta_tb",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    nome = table.Column<string>(type: "text", nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    tipo_etiqueta_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "fk_etiqueta_x_tipo_etiqueta",
                        column: x => x.tipo_etiqueta_id,
                        principalTable: "tipo_etiqueta_tb",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "comentarios_tb",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    tipo = table.Column<int>(type: "int", nullable: false),
                    valor = table.Column<string>(type: "text", nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    tipo_notificacao = table.Column<int>(type: "int", nullable: false),
                    referencia_id = table.Column<int>(type: "int", nullable: false),
                    data_validade = table.Column<DateOnly>(type: "date", nullable: true),
                    tipo_usuario_destino = table.Column<int>(type: "int", nullable: false),
                    usuario_destino_id = table.Column<int>(type: "int", nullable: false),
                    etiqueta = table.Column<long>(type: "bigint", maxLength: 70, nullable: false),
                    data_alteracao = table.Column<DateTime>(type: "DATETIME", nullable: true),
                    data_inclusao = table.Column<DateTime>(type: "DATETIME", nullable: true),
                    usuario_alteracao_id = table.Column<int>(type: "int", nullable: true),
                    usuario_inclusao_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "fk_etiqueta_x_comentario",
                        column: x => x.etiqueta,
                        principalTable: "etiqueta_tb",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateIndex(
                name: "IX_comentarios_tb_etiqueta",
                table: "comentarios_tb",
                column: "etiqueta");

            migrationBuilder.CreateIndex(
                name: "IX_etiqueta_tb_tipo_etiqueta_id",
                table: "etiqueta_tb",
                column: "tipo_etiqueta_id");

            migrationBuilder.Sql(@"INSERT INTO tipo_etiqueta_tb (nome) VALUES ('Sindicatos');");
            migrationBuilder.Sql(@"INSERT INTO tipo_etiqueta_tb (nome) VALUES ('Cláusulas')");
            migrationBuilder.Sql(@"INSERT INTO tipo_etiqueta_tb (nome) VALUES ('Sindicatos e Cláusulas')");

            migrationBuilder.Sql(@"INSERT INTO etiqueta_tb (nome, tipo_etiqueta_id) VALUES ('Antecipação de reajuste', 2);");
            migrationBuilder.Sql(@"INSERT INTO etiqueta_tb (nome, tipo_etiqueta_id) VALUES ('Ofensora', 2);");
            migrationBuilder.Sql(@"INSERT INTO etiqueta_tb (nome, tipo_etiqueta_id) VALUES ('Relevância', 1);");
            migrationBuilder.Sql(@"INSERT INTO etiqueta_tb (nome, tipo_etiqueta_id) VALUES ('Sindicato problemático', 1);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "comentarios_tb");

            migrationBuilder.DropTable(
                name: "etiqueta_tb");

            migrationBuilder.DropTable(
                name: "tipo_etiqueta_tb");

            migrationBuilder.CreateTable(
                name: "note_cliente",
                columns: table => new
                {
                    id_notecliente = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    comentario = table.Column<string>(type: "text", nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    data_final = table.Column<DateOnly>(type: "date", nullable: true),
                    data_registro = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    etiqueta = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    id_tipo_comentario = table.Column<int>(type: "int", nullable: false),
                    id_tipo_usuario_destino = table.Column<int>(type: "int", nullable: false),
                    tipo_comentario = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    tipo_notificacao = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    tipo_usuario_destino = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    usuario_adm_id_user = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id_notecliente);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");
        }
    }
}
