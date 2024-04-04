using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ineditta.Repository.Migrations
{
    /// <inheritdoc />
    public partial class v121 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tipo_evento_calendario_sindical",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    nome = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "subtipo_evento_calendario_sindical",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    nome = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    tipo_evento_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "subtipo_evento_ibfk_1",
                        column: x => x.tipo_evento_id,
                        principalTable: "tipo_evento_calendario_sindical",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "usuario_tipo_evento_calendario_sindical",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    usuario_id = table.Column<long>(type: "int", nullable: false),
                    tipo_evento_id = table.Column<int>(type: "int", nullable: false),
                    subtipo_evento_id = table.Column<int>(type: "int", nullable: true),
                    notificar_email = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    notificar_whatsapp = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    notificar_antes = table.Column<TimeSpan>(type: "time(6)", nullable: false, defaultValueSql: "'120:00:00'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "usuario_tipo_evento_calendario_sindical_ibfk_1",
                        column: x => x.usuario_id,
                        principalTable: "usuario_adm",
                        principalColumn: "id_user",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "usuario_tipo_evento_calendario_sindical_ibfk_2",
                        column: x => x.tipo_evento_id,
                        principalTable: "tipo_evento_calendario_sindical",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "usuario_tipo_evento_calendario_sindical_ibfk_3",
                        column: x => x.subtipo_evento_id,
                        principalTable: "subtipo_evento_calendario_sindical",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateIndex(
                name: "IX_subtipo_evento_calendario_sindical_tipo_evento_id",
                table: "subtipo_evento_calendario_sindical",
                column: "tipo_evento_id");

            migrationBuilder.CreateIndex(
                name: "IX_usuario_tipo_evento_calendario_sindical_subtipo_evento_id",
                table: "usuario_tipo_evento_calendario_sindical",
                column: "subtipo_evento_id");

            migrationBuilder.CreateIndex(
                name: "IX_usuario_tipo_evento_calendario_sindical_tipo_evento_id",
                table: "usuario_tipo_evento_calendario_sindical",
                column: "tipo_evento_id");

            migrationBuilder.CreateIndex(
                name: "IX_usuario_tipo_evento_calendario_sindical_usuario_id",
                table: "usuario_tipo_evento_calendario_sindical",
                column: "usuario_id");

            migrationBuilder.Sql(@"
                INSERT INTO tipo_evento_calendario_sindical (id, nome) VALUES 
                (1, 'Vencimento de Documento'),
                (2, 'Vencimento Mandato Sindical Laboral'),
                (3, 'Vencimento Mandato Sindical Patronal'),
                (4, 'Trintidio'),
                (5, 'Eventos de Cláusulas'),
                (6, 'Agenda de Eventos'),
                (7, 'Assembléia Patronal'),
                (8, 'Reunião entre as partes');
            ");

            migrationBuilder.Sql(@"
                INSERT INTO subtipo_evento_calendario_sindical (nome, tipo_evento_id)
                SELECT 
	                CONCAT(gc.nome_grupo, ' - ', at2.nmtipoinformacaoadicional) nome,
	                (SELECT id FROM tipo_evento_calendario_sindical WHERE nome = 'Eventos de Cláusulas') tipo_evento_id
                FROM grupo_clausula gc
                JOIN ad_tipoinformacaoadicional at2 ON at2.cdtipoinformacaoadicional IN (24,27,28,29)
                WHERE gc.idgrupo_clausula IN
	                (SELECT 
		                ec.grupo_clausula_idgrupo_clausula 
	                FROM estrutura_clausula ec 
	                WHERE ec.calendario = 'S');
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "usuario_tipo_evento_calendario_sindical");

            migrationBuilder.DropTable(
                name: "subtipo_evento_calendario_sindical");

            migrationBuilder.DropTable(
                name: "tipo_evento_calendario_sindical");
        }
    }
}
