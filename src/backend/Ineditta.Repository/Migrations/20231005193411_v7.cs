using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ineditta.Repository.Migrations
{
    /// <inheritdoc />
    public partial class v7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"create or replace view comentario_sindicato_vw as 
                select nt.id_notecliente notificacao_id, nt.tipo_comentario notificacao_tipo_comentario, nt.comentario notificacao_comentario,
                sempt.id_sinde sindicato_id, sempt.sigla_sinde sindicato_sigla, sempt.razaosocial_sinde sindicao_razao_social,
                nt.etiqueta notificacao_etiqueta, upubt.id_user usuario_publicacao_id, upubt.nome_usuario usuario_publicacao_nome, 
                nt.data_registro notificacao_data_registro, upubt.id_grupoecon usuario_publicacao_grupo_economico_id,
                upubt.tipo usuario_publicacao_tipo, btsempt.idbase_territorialSindEmp sindicato_base_territorial_id, lt.id_localizacao sindicato_localizacao_id, lt.uf sindicato_localizacao_uf,
                nt.tipo_usuario_destino notificacao_tipo_destino, nt.id_tipo_usuario_destino notificacao_tipo_destino_id,
                cut.id_unidade estabelecimento_id, coalesce(cmt.id_empresa, cut.cliente_matriz_id_empresa) empresa_id, COALESCE(cgt.id_grupo_economico, cut.cliente_grupo_id_grupo_economico) grupo_economico_id
                from note_cliente nt
                left join usuario_adm upubt on nt.usuario_adm_id_user = upubt.id_user 
                inner join sind_emp sempt on nt.id_tipo_comentario = sempt.id_sinde  
                inner join base_territorialsindemp btsempt on sempt.id_sinde = btsempt.idbase_territorialSindEmp 
                inner join localizacao lt on btsempt.localizacao_id_localizacao1 = lt.id_localizacao 
                left join cliente_unidades cut on nt.tipo_usuario_destino = 'unidade' and nt.id_tipo_usuario_destino = cut.id_unidade 
                left join cliente_matriz cmt on nt.tipo_usuario_destino = 'matriz' and nt.id_tipo_usuario_destino = cmt.id_empresa 
                left join cliente_grupo cgt on nt.tipo_usuario_destino = 'grupo' and nt.id_tipo_usuario_destino = cgt.id_grupo_economico 
                where nt.tipo_comentario = 'laboral'
                union all
                select nt.id_notecliente notificacao_id, nt.tipo_comentario notificacao_tipo_comentario, nt.comentario notificacao_comentario,
                spatr.id_sindp sindicato_id, spatr.sigla_sp  sindicato_sigla, spatr.razaosocial_sp sindicao_razao_social,
                nt.etiqueta notificacao_etiqueta, upubt.id_user usuario_publicacao_id, upubt.nome_usuario usuario_publicacao_nome,
                nt.data_registro notificacao_data_registro, upubt.id_grupoecon usuario_publicacao_grupo_economico_id,
                upubt.tipo usuario_publicacao_tipo, btpatrt.idbase_territorialSindPatro sindicato_base_territorial_id, lt.id_localizacao sindicato_localizacao_id, lt.uf sindicato_localizacao_uf,
                nt.tipo_usuario_destino notificacao_tipo_destino, nt.id_tipo_usuario_destino notificacao_tipo_destino_id,
                cut.id_unidade estabelecimento_id, coalesce(cmt.id_empresa, cut.cliente_matriz_id_empresa) empresa_id, COALESCE(cgt.id_grupo_economico, cut.cliente_grupo_id_grupo_economico) grupo_economico_id
                from note_cliente nt
                left join usuario_adm upubt on nt.usuario_adm_id_user = upubt.id_user 
                inner join sind_patr spatr on nt.id_tipo_comentario = spatr.id_sindp  
                inner join base_territorialsindpatro btpatrt on spatr.id_sindp  = btpatrt.idbase_territorialSindPatro  
                inner join localizacao lt on btpatrt.localizacao_id_localizacao1  = lt.id_localizacao 
                left join cliente_unidades cut on nt.tipo_usuario_destino = 'unidade' and nt.id_tipo_usuario_destino = cut.id_unidade 
                left join cliente_matriz cmt on nt.tipo_usuario_destino = 'matriz' and nt.id_tipo_usuario_destino = cmt.id_empresa 
                left join cliente_grupo cgt on nt.tipo_usuario_destino = 'grupo' and nt.id_tipo_usuario_destino = cgt.id_grupo_economico 
                where nt.tipo_comentario = 'patronal';");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP VIEW comentario_sindicato_vw;");
        }
    }
}
