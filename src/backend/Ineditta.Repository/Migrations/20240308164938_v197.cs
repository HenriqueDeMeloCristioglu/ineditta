using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ineditta.Repository.Migrations
{
    /// <inheritdoc />
    public partial class v197 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "data_inclusao",
                table: "cliente_unidades",
                type: "DATETIME",
                nullable: true,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "timestamp",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<DateTime>(
                name: "data_alteracao",
                table: "cliente_unidades",
                type: "DATETIME",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "usuario_alteracao_id",
                table: "cliente_unidades",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "usuario_inclusao_id",
                table: "cliente_unidades",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "data_alteracao",
                table: "cliente_unidades");

            migrationBuilder.DropColumn(
                name: "usuario_alteracao_id",
                table: "cliente_unidades");

            migrationBuilder.DropColumn(
                name: "usuario_inclusao_id",
                table: "cliente_unidades");

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_inclusao",
                table: "cliente_unidades",
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
