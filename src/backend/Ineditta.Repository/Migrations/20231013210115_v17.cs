using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ineditta.Repository.Migrations
{
    /// <inheritdoc />
    public partial class v17 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"create or replace view usuarios_adms_vw as
	                                select uat.id_user id, uat.nome_usuario nome, uat.email_usuario email, uat.cargo, 
	                                uat.telefone, uat.ramal, uat.departamento, uat.id_user_superior id_superior, 
	                                uat.data_inclusao data_criacao, uat.id_grupoecon, uat.ids_fmge, uat.nivel, uart.nome_usuario nome_user_criador from usuario_adm uat
                                    left join usuario_adm uart on uat.usuario_inclusao_id = uart.id_user");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"create or replace view usuarios_adms_vw as
	                                select uat.id_user id, uat.nome_usuario nome, uat.email_usuario email, uat.cargo, 
	                                uat.telefone, uat.ramal, uat.departamento, uat.id_user_superior id_superior, 
	                                uat.data_inclusao data_criacao, uart.nome_usuario nome_user_criador from usuario_adm uat
                                    left join usuario_adm uart on uat.usuario_inclusao_id = uart.id_user");
        }
    }
}
