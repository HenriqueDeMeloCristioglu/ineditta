using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ineditta.Repository.Migrations
{
    /// <inheritdoc />
    public partial class v214 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "data_alteracao",
                table: "ia_documento_sindical_tb",
                type: "DATETIME",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "data_aprovacao",
                table: "ia_documento_sindical_tb",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "data_inclusao",
                table: "ia_documento_sindical_tb",
                type: "DATETIME",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "usuario_alteracao_id",
                table: "ia_documento_sindical_tb",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "usuario_aprovador_id",
                table: "ia_documento_sindical_tb",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "usuario_inclusao_id",
                table: "ia_documento_sindical_tb",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "data_alteracao",
                table: "ia_documento_sindical_tb");

            migrationBuilder.DropColumn(
                name: "data_aprovacao",
                table: "ia_documento_sindical_tb");

            migrationBuilder.DropColumn(
                name: "data_inclusao",
                table: "ia_documento_sindical_tb");

            migrationBuilder.DropColumn(
                name: "usuario_alteracao_id",
                table: "ia_documento_sindical_tb");

            migrationBuilder.DropColumn(
                name: "usuario_aprovador_id",
                table: "ia_documento_sindical_tb");

            migrationBuilder.DropColumn(
                name: "usuario_inclusao_id",
                table: "ia_documento_sindical_tb");
        }
    }
}
