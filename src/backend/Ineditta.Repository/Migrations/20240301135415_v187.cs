using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ineditta.Repository.Migrations
{
    /// <inheritdoc />
    public partial class v187 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"truncate email_caixa_de_saida_tb");

            migrationBuilder.DropColumn(
                name: "data_alteracao",
                table: "email_caixa_de_saida_tb");

            migrationBuilder.DropColumn(
                name: "usuario_alteracao_id",
                table: "email_caixa_de_saida_tb");

            migrationBuilder.DropColumn(
                name: "usuario_inclusao_id",
                table: "email_caixa_de_saida_tb");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "data_inclusao",
                table: "email_caixa_de_saida_tb",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1),
                oldClrType: typeof(DateTime),
                oldType: "DATETIME",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "data_inclusao",
                table: "email_caixa_de_saida_tb",
                type: "DATETIME",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AddColumn<DateTime>(
                name: "data_alteracao",
                table: "email_caixa_de_saida_tb",
                type: "DATETIME",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "usuario_alteracao_id",
                table: "email_caixa_de_saida_tb",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "usuario_inclusao_id",
                table: "email_caixa_de_saida_tb",
                type: "int",
                nullable: true);
        }
    }
}
