using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ineditta.Repository.Migrations
{
    /// <inheritdoc />
    public partial class v92 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP VIEW IF EXISTS calendario_sindical_assembleia_reuniao_vw;");

            migrationBuilder.Sql(@"
                CREATE OR REPLACE VIEW calendario_sindical_assembleia_reuniao_vw AS
                SELECT
                    cst.data_referencia data_referencia,
                    CASE WHEN cst.origem = 1 THEN 'Ineditta' ELSE 'Cliente' END AS origem,
                    CASE WHEN cst.tipo_evento = 7 THEN 'Assembleia patronal com as empresas' ELSE 'Reunião entre entidades sindicais' END AS tipo_evento,
                    ac.sind_emp_id_sinde sindicato_laboral_id,
                    ac.sind_patr_id_sindp sindicato_patronal_id,
                    se.sigla_sinde sindicato_laboral_sigla,
                    sp.sigla_sp sindicato_patronal_sigla,
                    ac.ids_cnaes atividades_economicas_ids,
                    ac.data_base data_base,
                    td.idtipo_doc tipo_doc_id,
                    td.nome_doc nome_documento,
                    ac.fase fase_documento,
                    cst.chave_referencia chave_referencia,
                    ac.grupos_economicos_ids grupos_economicos_ids,
                    ac.empresas_ids matrizes_ids,
                    cct.descricoes_subclasses descricoes_subclasses,
                    cst.tipo_evento tipo_evento_id
                FROM calendario_sindical_tb cst
                JOIN acompanhanto_cct ac ON ac.idacompanhanto_cct = cst.chave_referencia
                LEFT JOIN sind_emp se ON se.id_sinde = ac.sind_emp_id_sinde
                LEFT JOIN sind_patr sp ON sp.id_sindp = ac.sind_patr_id_sindp
                LEFT JOIN tipo_doc td ON td.idtipo_doc = ac.tipo_doc_idtipo_doc 
                LEFT JOIN LATERAL (SELECT GROUP_concat(cct.descricao_subclasse separator ';') descricoes_subclasses FROM classe_cnae cct
                                   where JSON_CONTAINS(ac.ids_cnaes, JSON_ARRAY( cast(cct.id_cnae as char)))) cct on true
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP VIEW IF EXISTS calendario_sindical_assembleia_reuniao_vw;");
        }
    }
}
