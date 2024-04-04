using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ineditta.Repository.Migrations
{
    /// <inheritdoc />
    public partial class v125 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP VIEW IF EXISTS tipos_subtipos_evento_calendario_sindical_vw;");

            migrationBuilder.Sql(@"
                CREATE OR REPLACE VIEW tipos_subtipos_evento_calendario_sindical_vw AS
                SELECT *, null tipo_associado FROM tipo_evento_calendario_sindical tecs
                WHERE NOT EXISTS (
	                SELECT 1 FROM subtipo_evento_calendario_sindical secs
	                WHERE secs.tipo_evento_id = tecs.id
                )
                UNION ALL
                SELECT 
	                secs2.id, CONCAT(tecs2.nome, ' - ', secs2.nome) nome, tipo_evento_id tipo_associado 
                FROM subtipo_evento_calendario_sindical secs2
                JOIN tipo_evento_calendario_sindical tecs2 ON tecs2.id = secs2.tipo_evento_id;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP VIEW IF EXISTS tipos_subtipos_evento_calendario_sindical_vw;");
        }
    }
}
