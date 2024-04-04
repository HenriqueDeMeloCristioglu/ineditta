using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ineditta.Repository.Migrations
{
    /// <inheritdoc />
    public partial class v21 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var sqlScript = @"CREATE PROCEDURE DropForeignKeyIfItExists()
                            BEGIN
                                DECLARE fk_exists INT DEFAULT 0;

                                SELECT COUNT(*)
                                INTO fk_exists
                                FROM information_schema.KEY_COLUMN_USAGE
                                WHERE TABLE_SCHEMA = DATABASE()
                                AND TABLE_NAME = 'anuencia_usuarios'
                                AND CONSTRAINT_NAME = 'ibfk_documentos_anuencia';

                                IF fk_exists > 0 THEN
                                    SET @alterQuery = CONCAT('ALTER TABLE anuencia_usuarios DROP FOREIGN KEY ', 'ibfk_documentos_anuencia');
                                    PREPARE stmt FROM @alterQuery;
                                    EXECUTE stmt;
                                    DEALLOCATE PREPARE stmt;
                                END IF;
                            END;";

            migrationBuilder.Sql(sqlScript);

            migrationBuilder.Sql(@"CALL DropForeignKeyIfItExists();");

            migrationBuilder.Sql(@"DROP PROCEDURE IF EXISTS DropForeignKeyIfItExists;");

            migrationBuilder.AddForeignKey(
                name: "ibfk_documentos_anuencia",
                table: "anuencia_usuarios",
                column: "documentos_iddocumentos",
                principalTable: "doc_sind",
                principalColumn: "id_doc",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "ibfk_documentos_anuencia",
                table: "anuencia_usuarios");
        }
    }
}