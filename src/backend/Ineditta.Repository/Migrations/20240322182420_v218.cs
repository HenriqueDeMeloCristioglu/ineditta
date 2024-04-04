using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ineditta.Repository.Migrations
{
    /// <inheritdoc />
    public partial class v218 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"create or replace view ia_documento_sindical_vw as
                                SELECT 
                                    iads.id,
                                    td.nome_doc AS nome,
                                    ds.id_doc AS documento_referencia_id,
                                    CASE 
                                        WHEN iads.status = 'Quebrando Clausulas' THEN 'Quebrando Cláusulas'
                                        WHEN iads.status = 'Classificando Clausulas' THEN 'Classificando Cláusulas'
                                        WHEN iads.status = 'Aguardando Aprovacao Quebra Clausula' THEN 'Aguardando Aprovação Quebra Cláusula'
                                        WHEN iads.status = 'Aprovado' THEN 'Aprovado'
                                        WHEN iads.status = 'Aguardando Aprovacao Classificacao' THEN 'Aguardando Aprovação Classificação'
                                        WHEN iads.status = 'Aguardando Processamento' THEN 'Aguardando Processamento'
                                        ELSE 'Erro'
                                    END AS status,
                                    ds.versao_documento AS versao,
                                    ds.data_sla AS data_sla,
                                    iads.data_aprovacao AS data_aprovacao,
                                    us.nome_usuario AS usuario_aprovador,
                                    (
                                        SELECT COUNT(*)
                                        FROM ia_clausula_tb
                                        WHERE ia_documento_sindical_id = iads.id
                                    ) AS quantidade_clausulas,
                                    CASE
                                        WHEN EXISTS (
                                            SELECT 1
                                            FROM ia_clausula_tb
                                            WHERE ia_documento_sindical_id = iads.id AND status = 'Inconsistente'
                                        ) THEN 'Inconsistente'
                                        ELSE 'Consistente'
                                    END AS status_geral,
                                    s.assuntos AS assunto
                                FROM ia_documento_sindical_tb iads
                                INNER JOIN doc_sind ds ON iads.documento_referencia_id = ds.id_doc
                                INNER JOIN tipo_doc td ON ds.tipo_doc_idtipo_doc = td.idtipo_doc
                                LEFT JOIN usuario_adm us ON iads.usuario_aprovador_id = us.id_user
                                LEFT JOIN lateral (
	                                select GROUP_CONCAT(ec.nome_clausula SEPARATOR "", "") AS assuntos from estrutura_clausula ec 
	                                where JSON_CONTAINS(ds.referencia, cast(concat('""', ec.id_estruturaclausula, '""') as char))
                                ) s on true;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"drop view ia_documento_sindical_vw;");
        }
    }
}
