using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ineditta.Repository.Migrations
{
    /// <inheritdoc />
    public partial class v193 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "data_alteracao",
                table: "doc_sind",
                type: "DATETIME",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "data_inclusao",
                table: "doc_sind",
                type: "DATETIME",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "usuario_alteracao_id",
                table: "doc_sind",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "usuario_inclusao_id",
                table: "doc_sind",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "data_alteracao",
                table: "doc_sind");

            migrationBuilder.DropColumn(
                name: "data_inclusao",
                table: "doc_sind");

            migrationBuilder.DropColumn(
                name: "usuario_alteracao_id",
                table: "doc_sind");

            migrationBuilder.DropColumn(
                name: "usuario_inclusao_id",
                table: "doc_sind");
        }
    }
}
