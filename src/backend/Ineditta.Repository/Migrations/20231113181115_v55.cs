using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable
#pragma warning disable
namespace Ineditta.Repository.Migrations
{
    /// <inheritdoc />
    public partial class v56 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "recorrencia",
                table: "calendario_sindical_usuario_tb",
                type: "int",
                nullable: true,
                defaultValue: 1,
                oldClrType: typeof(TimeSpan),
                oldType: "time(6)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_referencia",
                table: "calendario_sindical_tb",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<TimeSpan>(
                name: "recorrencia",
                table: "calendario_sindical_usuario_tb",
                type: "time(6)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldDefaultValue: 1);

            migrationBuilder.AlterColumn<DateOnly>(
                name: "data_referencia",
                table: "calendario_sindical_tb",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");
        }
    }
}
