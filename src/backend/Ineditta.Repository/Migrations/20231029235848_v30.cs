using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ineditta.Repository.Migrations
{
    /// <inheritdoc />
    public partial class v30 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"drop view if exists evento_calendario_trintidio_vw");
            migrationBuilder.Sql(@"drop view if exists evento_calendario_vencimento_mandato_laboral_vw");

            migrationBuilder.Sql(@"
                CREATE OR REPLACE VIEW evento_calendario_trintidio_vw AS
                SELECT 
	                DATE_SUB(ds.validade_inicial, INTERVAL 60 DAY) `data`,
	                'Trintídio' grupo,
	                'Ineditta' origem,
	                DATE_SUB(ds.validade_inicial, INTERVAL 60 DAY) validade_inicial,
	                DATE_SUB(ds.validade_final, INTERVAL 29 DAY) validade_final,
	                ds.tipo_doc_idtipo_doc tipo_doc,
	                td.nome_doc,
	                ds.validade_inicial database_doc,
	                ds.cnae_doc atividades_economicas_json,
	                ds.cliente_estabelecimento cliente_estabelecimento,
	                ds.sind_laboral,
	                ds.sind_patronal,
	                ds.referencia,
	                REPLACE(REPLACE(REPLACE(JSON_EXTRACT(ds.cnae_doc, '$[*].subclasse') , '\""', ''), '[', ''), ']', '')  AS atividades_economicas,
	                REPLACE(REPLACE(REPLACE(JSON_EXTRACT(ds.sind_laboral, '$[*].sigla'), '\""', ''), '[', ''), ']', '')  AS siglas_sindicatos_laborais,
	                REPLACE(REPLACE(REPLACE(JSON_EXTRACT(ds.sind_patronal, '$[*].sigla'), '\""', ''), '[', ''), ']', '')  AS siglas_sindicatos_patronais
                FROM
	                doc_sind ds
                JOIN tipo_doc td on td.idtipo_doc = ds.tipo_doc_idtipo_doc
                WHERE
	                ds.tipo_doc_idtipo_doc in (5,6)
	                OR (ds.tipo_doc_idtipo_doc in (8,9) and JSON_CONTAINS(ds.referencia, JSON_ARRAY('84')))
	                and ds.modulo = 'SISAP'
            ");

            migrationBuilder.Sql(@"
                CREATE OR REPLACE VIEW evento_calendario_vencimento_mandato_laboral_vw AS
                SELECT
	                sd.termino_mandatoe `data`,
	                'Ineditta' origem,
	                sd.inicio_mandatoe validade_inicial,
	                sd.termino_mandatoe validade_final,
	                sd.dirigente_e dirigente,
	                sd.funcao_e funcao,
	                (SELECT GROUP_CONCAT(CONCAT(' ',descricao_subclasse))
                    FROM base_territorialsindemp bt 
                    INNER JOIN classe_cnae cc on cc.id_cnae = bt.classe_cnae_idclasse_cnae
                    WHERE bt.sind_empregados_id_sinde1 = se.id_sinde) AS atividades_economicas,
	                se.sigla_sinde AS siglas_sindicatos_laborais,
	                se.id_sinde sindicato_laboral_id
                FROM sind_diremp sd
                INNER JOIN sind_emp se ON se.id_sinde = sd.sind_emp_id_sinde
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"drop view if exists evento_calendario_trintidio_vw");
            migrationBuilder.Sql(@"drop view if exists evento_calendario_vencimento_mandato_laboral_vw");
        }
    }
}
