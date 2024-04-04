using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ineditta.Repository.Migrations
{
    /// <inheritdoc />
    public partial class v23 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"create or replace view estabelecimentos_vw as
                                    select cut.id_unidade id, cut.nome_unidade nome, cm.nome_empresa filial, cg.nome_grupoeconomico grupo, cut.cnpj_unidade cnpj, cg.nome_grupoeconomico nome_grupo_economico, cm.cliente_grupo_id_grupo_economico from cliente_unidades cut
					                left join cliente_matriz cm on cut.cliente_matriz_id_empresa = cm.id_empresa 
					                left join cliente_grupo cg on cm.cliente_grupo_id_grupo_economico = cg.id_grupo_economico");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP VIEW if exists estabelecimentos_vw");
        }
    }
}
