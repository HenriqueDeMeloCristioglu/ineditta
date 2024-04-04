using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ineditta.Repository.Migrations
{
    /// <inheritdoc />
    public partial class v76 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"create index ix001_indecon_real on indecon_real (indicador, periodo_data);");

            migrationBuilder.Sql(@"create index ix001_cliente_unidades on cliente_unidades (localizacao_id_localizacao, cliente_grupo_id_grupo_economico, id_unidade);");

            migrationBuilder.Sql(@"create index ix002_base_territorialsindpatro on base_territorialsindpatro (sind_patronal_id_sindp, localizacao_id_localizacao1, classe_cnae_idclasse_cnae);");

            migrationBuilder.Sql(@"create index ix001_base_territorialsindemp on base_territorialsindemp (sind_empregados_id_sinde1, localizacao_id_localizacao1, classe_cnae_idclasse_cnae);");

            migrationBuilder.Sql(@"create index ix001_usuario_adm on usuario_adm (email_usuario);");

            migrationBuilder.Sql(@"create index ix001_cnae_emp on cnae_emp (cliente_unidades_id_unidade, classe_cnae_idclasse_cnae);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"drop index if exists ix001_indecon_real on indecon_real;");

            migrationBuilder.Sql(@"drop index if exists ix001_cliente_unidades on cliente_unidades;");

            migrationBuilder.Sql(@"drop index if exists ix002_base_territorialsindpatro on base_territorialsindpatro;");

            migrationBuilder.Sql(@"drop index if exists ix001_base_territorialsindemp on base_territorialsindemp;");

            migrationBuilder.Sql(@"drop index if exists ix001_usuario_adm on usuario_adm;");

            migrationBuilder.Sql(@"drop index if exists ix001_cnae_emp on cnae_emp;");
        }
    }
}
