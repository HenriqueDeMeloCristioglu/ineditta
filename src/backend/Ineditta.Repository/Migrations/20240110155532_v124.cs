﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ineditta.Repository.Migrations
{
    /// <inheritdoc />
    public partial class v124 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "nome_arquivo",
                table: "doc_sind",
                type: "longtext",
                nullable: true,
                collation: "utf8mb4_0900_ai_ci")
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "nome_arquivo",
                table: "doc_sind");
        }
    }
}
