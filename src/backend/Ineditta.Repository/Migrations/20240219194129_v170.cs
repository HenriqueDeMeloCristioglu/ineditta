using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ineditta.Repository.Migrations
{
    /// <inheritdoc />
    public partial class v170 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "status_code",
                table: "email_storage_manager_tb");

            migrationBuilder.AddColumn<string>(
                name: "message_id",
                table: "email_storage_manager_tb",
                type: "longtext",
                nullable: false,
                collation: "utf8mb4_0900_ai_ci")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.RenameColumn(
                name: "email",
                table: "email_storage_manager_tb",
                newName: "to");

            migrationBuilder.AddColumn<string>(
                name: "from",
                table: "email_storage_manager_tb",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                collation: "utf8mb4_0900_ai_ci")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "Enviado",
                table: "email_storage_manager_tb",
                type: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "from",
                table: "email_storage_manager_tb");

            migrationBuilder.RenameColumn(
                name: "to",
                table: "email_storage_manager_tb",
                newName: "email");

            migrationBuilder.DropColumn(
                name: "message_id",
                table: "email_storage_manager_tb");

            migrationBuilder.AddColumn<int>(
                name: "status_code",
                table: "email_storage_manager_tb",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.DropColumn(
                name: "Enviado",
                table: "email_storage_manager_tb");
        }
    }
}
