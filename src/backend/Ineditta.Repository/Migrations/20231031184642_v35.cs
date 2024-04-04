using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ineditta.Repository.Migrations
{
    /// <inheritdoc />
    public partial class v35 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"drop view if exists evento_calendario_desconto_pagamento_vencimento_vw;");

            migrationBuilder.Sql(@"
                CREATE OR REPLACE VIEW evento_calendario_desconto_pagamento_vencimento_vw AS
                SELECT 
	                cgec.id_clausulageral_estrutura_clausula,
	                cgec.clausula_geral_id_clau,
	                cgec.grupo_dados,
	                cgec.ad_tipoinformacaoadicional_cdtipoinformacaoadicional,
	                cgec.combo frequencia,
	                cgec2.ad_tipoinformacaoadicional_cdtipoinformacaoadicional nome_evento_id,
	                at2.nmtipoinformacaoadicional nome_evento,
	                cgec2.`data` data_evento,
	                REPLACE(REPLACE(REPLACE(JSON_EXTRACT(ds.cnae_doc, '$[*].subclasse') , '\""', ''), '[', ''), ']', '')  AS atividades_economicas,
	                REPLACE(REPLACE(REPLACE(JSON_EXTRACT(ds.sind_laboral, '$[*].sigla'), '\""', ''), '[', ''), ']', '')  AS sindicatos_laborais,
	                REPLACE(REPLACE(REPLACE(JSON_EXTRACT(ds.sind_patronal, '$[*].sigla'), '\""', ''), '[', ''), ']', '')  AS sindicatos_patronais,
	                ec2.nome_clausula,
	                gc.nome_grupo,
	                ds.*,
	                td.nome_doc nome_documento,
	                td.idtipo_doc tipo_doc_id,
	                'Ineditta' origem_evento
                FROM clausula_geral_estrutura_clausula cgec
                LEFT JOIN clausula_geral_estrutura_clausula cgec2 ON 
	                cgec2.clausula_geral_id_clau = cgec.clausula_geral_id_clau 
	                and cgec2.grupo_dados = cgec.grupo_dados 
	                and cgec2.ad_tipoinformacaoadicional_cdtipoinformacaoadicional IN (24,28,29)
                LEFT JOIN doc_sind ds ON ds.id_doc = cgec.doc_sind_id_doc
                LEFT JOIN tipo_doc td ON td.idtipo_doc = ds.tipo_doc_idtipo_doc
                LEFT JOIN ad_tipoinformacaoadicional at2 ON at2.cdtipoinformacaoadicional = cgec2.ad_tipoinformacaoadicional_cdtipoinformacaoadicional
                LEFT JOIN estrutura_clausula ec2 ON ec2.id_estruturaclausula = cgec.nome_informacao
                LEFT JOIN grupo_clausula gc ON gc.idgrupo_clausula = ec2.grupo_clausula_idgrupo_clausula
                WHERE cgec.nome_informacao IN (
	                  SELECT ec.id_estruturaclausula FROM estrutura_clausula ec
	                  WHERE ec.calendario = 'S'
                )
	                  AND cgec.ad_tipoinformacaoadicional_cdtipoinformacaoadicional IN (11)
	                  AND CASE WHEN cgec.ad_tipoinformacaoadicional_cdtipoinformacaoadicional = 11 THEN cgec.combo <> 'Não se aplica' and cgec.combo <> 'Não Consta' ELSE true END
	                  AND cgec2.`data` is not null
	                  AND ds.modulo = 'SISAP'
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"drop view if exists evento_calendario_desconto_pagamento_vencimento_vw;");
        }
    }
}
