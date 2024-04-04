using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ineditta.Repository.Migrations
{
    /// <inheritdoc />
    public partial class v122 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"UPDATE clausula_geral cg
                                SET cg.sinonimo_id = (
                                    SELECT sn.id_sinonimo FROM sinonimos sn
                                    WHERE cg.assunto_idassunto = sn.assunto_idassunto
                                    LIMIT 1
                                );");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"UPDATE clausula_geral cg SET cg.sinonimo_id = null;");
        }
    }
}
