using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ineditta.Repository.Migrations
{
    /// <inheritdoc />
    public partial class v49 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "notificacao_evento_tb",
                newName: "calendario_sindical_notificacao_tb");

            migrationBuilder.RenameTable(
                name: "evento_tb",
                newName: "calendario_sindical_tb");

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_inclusao",
                table: "calendario_sindical_tb",
                type: "DATETIME",
                nullable: true,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "timestamp",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<bool>(
                name: "ativo",
                table: "calendario_sindical_tb",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "data_alteracao",
                table: "calendario_sindical_tb",
                type: "DATETIME",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "data_referencia",
                table: "calendario_sindical_tb",
                type: "date",
                nullable: false);

            migrationBuilder.AddColumn<int>(
                name: "usuario_alteracao_id",
                table: "calendario_sindical_tb",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "usuario_inclusao_id",
                table: "calendario_sindical_tb",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ativo",
                table: "calendario_sindical_tb");

            migrationBuilder.DropColumn(
                name: "data_alteracao",
                table: "calendario_sindical_tb");

            migrationBuilder.DropColumn(
                name: "data_referencia",
                table: "calendario_sindical_tb");

            migrationBuilder.DropColumn(
                name: "usuario_alteracao_id",
                table: "calendario_sindical_tb");

            migrationBuilder.DropColumn(
                name: "usuario_inclusao_id",
                table: "calendario_sindical_tb");

            migrationBuilder.RenameTable(
                name: "calendario_sindical_tb",
                newName: "evento_tb");

            migrationBuilder.RenameTable(
                name: "calendario_sindical_notificacao_tb",
                newName: "notificacao_evento_tb");

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_inclusao",
                table: "evento_tb",
                type: "timestamp",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "DATETIME",
                oldNullable: true,
                oldDefaultValueSql: "CURRENT_TIMESTAMP");
        }
    }
}
