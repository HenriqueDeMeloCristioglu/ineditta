using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ineditta.Repository.Migrations
{
    /// <inheritdoc />
    public partial class v195 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "data_alteracao",
                table: "sind_patr",
                type: "DATETIME",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "data_inclusao",
                table: "sind_patr",
                type: "DATETIME",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "usuario_alteracao_id",
                table: "sind_patr",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "usuario_inclusao_id",
                table: "sind_patr",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "data_alteracao",
                table: "sind_emp",
                type: "DATETIME",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "data_inclusao",
                table: "sind_emp",
                type: "DATETIME",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "usuario_alteracao_id",
                table: "sind_emp",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "usuario_inclusao_id",
                table: "sind_emp",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "data_alteracao",
                table: "base_territorialsindpatro",
                type: "DATETIME",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "data_inclusao",
                table: "base_territorialsindpatro",
                type: "DATETIME",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "usuario_alteracao_id",
                table: "base_territorialsindpatro",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "usuario_inclusao_id",
                table: "base_territorialsindpatro",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "data_alteracao",
                table: "base_territorialsindemp",
                type: "DATETIME",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "data_inclusao",
                table: "base_territorialsindemp",
                type: "DATETIME",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "usuario_alteracao_id",
                table: "base_territorialsindemp",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "usuario_inclusao_id",
                table: "base_territorialsindemp",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "data_alteracao",
                table: "sind_patr");

            migrationBuilder.DropColumn(
                name: "data_inclusao",
                table: "sind_patr");

            migrationBuilder.DropColumn(
                name: "usuario_alteracao_id",
                table: "sind_patr");

            migrationBuilder.DropColumn(
                name: "usuario_inclusao_id",
                table: "sind_patr");

            migrationBuilder.DropColumn(
                name: "data_alteracao",
                table: "sind_emp");

            migrationBuilder.DropColumn(
                name: "data_inclusao",
                table: "sind_emp");

            migrationBuilder.DropColumn(
                name: "usuario_alteracao_id",
                table: "sind_emp");

            migrationBuilder.DropColumn(
                name: "usuario_inclusao_id",
                table: "sind_emp");

            migrationBuilder.DropColumn(
                name: "data_alteracao",
                table: "base_territorialsindpatro");

            migrationBuilder.DropColumn(
                name: "data_inclusao",
                table: "base_territorialsindpatro");

            migrationBuilder.DropColumn(
                name: "usuario_alteracao_id",
                table: "base_territorialsindpatro");

            migrationBuilder.DropColumn(
                name: "usuario_inclusao_id",
                table: "base_territorialsindpatro");

            migrationBuilder.DropColumn(
                name: "data_alteracao",
                table: "base_territorialsindemp");

            migrationBuilder.DropColumn(
                name: "data_inclusao",
                table: "base_territorialsindemp");

            migrationBuilder.DropColumn(
                name: "usuario_alteracao_id",
                table: "base_territorialsindemp");

            migrationBuilder.DropColumn(
                name: "usuario_inclusao_id",
                table: "base_territorialsindemp");
        }
    }
}
