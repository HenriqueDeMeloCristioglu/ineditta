using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ineditta.Repository.Migrations
{
    /// <inheritdoc />
    public partial class v149 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP VIEW IF EXISTS organizacao_patronal_vw;");
            migrationBuilder.Sql(@"CREATE OR REPLACE VIEW organizacao_patronal_vw AS
                SELECT  
    	            sp.id_sindp id, 
    	            sp.municipio_sp municipio,
    	            sp.sigla_sp sigla,
    	            sp.cnpj_sp cnpj,
    	            a.nome nome_confederacao,
    	            a2.nome nome_federacao,
                    NULL associado
                FROM sind_patr sp 
                LEFT JOIN associacao a ON sp.confederacao_id_associacao = a.id_associacao 
                LEFT JOIN associacao a2 ON sp.federacao_id_associacao = a.id_associacao;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP VIEW IF EXISTS organizacao_patronal_vw;");
        }
    }
}
