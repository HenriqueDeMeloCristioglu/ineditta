using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ineditta.Repository.Migrations
{
    /// <inheritdoc />
    public partial class v171 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"UPDATE clausula_geral set aprovado = 'sim' where aprovado = ""'sim'"";");
            migrationBuilder.Sql(@"UPDATE clausula_geral set aprovado = 'nao' where aprovado = ""'nao'"";");

            migrationBuilder.Sql(@"UPDATE clausula_geral set liberado = 'S' where liberado = ""'S'"";");

            migrationBuilder.Sql(@"ALTER TABLE clausula_geral MODIFY COLUMN aprovado VARCHAR(45) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT 'nao';");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"ALTER TABLE clausula_geral MODIFY COLUMN aprovado VARCHAR(45) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT ""'nao'"";");
        }
    }
}
