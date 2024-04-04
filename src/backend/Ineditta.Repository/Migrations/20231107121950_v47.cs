using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ineditta.Repository.Migrations
{
    /// <inheritdoc />
    public partial class v47 : Migration
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
                    REPLACE(REPLACE(REPLACE(JSON_EXTRACT(ds.sind_laboral, '$[*].id'), '\""', ''), '[', ''), ']', '')  AS sindicatos_laborais_ids,
	                REPLACE(REPLACE(REPLACE(JSON_EXTRACT(ds.sind_laboral, '$[*].sigla'), '\""', ''), '[', ''), ']', '')  AS sindicatos_laborais,
                    REPLACE(REPLACE(REPLACE(JSON_EXTRACT(ds.sind_patronal, '$[*].id'), '\""', ''), '[', ''), ']', '')  AS sindicatos_patronais_ids,
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
	                and cgec2.ad_tipoinformacaoadicional_cdtipoinformacaoadicional IN (24,27,28,29)
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

            migrationBuilder.Sql(@"drop view if exists evento_calendario_trintidio_vw");

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
                    REPLACE(REPLACE(REPLACE(JSON_EXTRACT(ds.sind_laboral, '$[*].id'), '\""', ''), '[', ''), ']', '')  AS sindicatos_laborais_ids,
	                REPLACE(REPLACE(REPLACE(JSON_EXTRACT(ds.sind_laboral, '$[*].sigla'), '\""', ''), '[', ''), ']', '')  AS siglas_sindicatos_laborais,
                    REPLACE(REPLACE(REPLACE(JSON_EXTRACT(ds.sind_patronal, '$[*].id'), '\""', ''), '[', ''), ']', '')  AS sindicatos_patronais_ids,
                    REPLACE(REPLACE(REPLACE(JSON_EXTRACT(ds.sind_patronal, '$[*].sigla'), '\""', ''), '[', ''), ']', '')  AS siglas_sindicatos_patronais
                FROM
	                doc_sind ds
                JOIN tipo_doc td on td.idtipo_doc = ds.tipo_doc_idtipo_doc
                WHERE
	                ds.tipo_doc_idtipo_doc in (5,6)
	                OR (ds.tipo_doc_idtipo_doc in (8,9) and JSON_CONTAINS(ds.referencia, JSON_ARRAY('84')))
	                and ds.modulo = 'SISAP'
            ");

            migrationBuilder.Sql(@"drop view if exists evento_calendario_vencimento_documento_vw");

            migrationBuilder.Sql(@"
                CREATE OR REPLACE VIEW evento_calendario_vencimento_documento_vw AS
                SELECT
	                ds.validade_final `data`,
	                'Vencimento de Documentos' tipo_evento,
	                'Ineditta' origem,
	                ds.validade_inicial validade_inicial,
	                ds.validade_final validade_final,
	                ds.tipo_doc_idtipo_doc tipo_doc_id,
	                td.nome_doc tipo_doc_nome,
	                ds.cnae_doc atividades_economicas_json,
	                ds.cliente_estabelecimento cliente_estabelecimento,
	                ds.sind_laboral,
	                ds.sind_patronal,
	                ds.referencia,
	                REPLACE(REPLACE(REPLACE(JSON_EXTRACT(ds.cnae_doc, '$[*].subclasse') , '\""', ''), '[', ''), ']', '')  AS atividades_economicas,
                    REPLACE(REPLACE(REPLACE(JSON_EXTRACT(ds.sind_laboral, '$[*].id'), '\""', ''), '[', ''), ']', '')  AS sindicatos_laborais_ids,
	                REPLACE(REPLACE(REPLACE(JSON_EXTRACT(ds.sind_laboral, '$[*].sigla'), '\""', ''), '[', ''), ']', '')  AS siglas_sindicatos_laborais,
                    REPLACE(REPLACE(REPLACE(JSON_EXTRACT(ds.sind_patronal, '$[*].id'), '\""', ''), '[', ''), ']', '')  AS sindicatos_patronais_ids,
                    REPLACE(REPLACE(REPLACE(JSON_EXTRACT(ds.sind_patronal, '$[*].sigla'), '\""', ''), '[', ''), ']', '')  AS siglas_sindicatos_patronais
                FROM doc_sind ds
                LEFT JOIN tipo_doc td on td.idtipo_doc = ds.tipo_doc_idtipo_doc
                WHERE ds.modulo = 'SISAP'
                      AND ds.validade_final <> '0000-00-00'
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.Sql(@"drop view if exists evento_calendario_trintidio_vw");

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

            migrationBuilder.Sql(@"drop view if exists evento_calendario_vencimento_documento_vw");

            migrationBuilder.Sql(@"
                CREATE OR REPLACE VIEW evento_calendario_vencimento_documento_vw AS
                SELECT
	                ds.validade_final `data`,
	                'Vencimento de Documentos' tipo_evento,
	                'Ineditta' origem,
	                ds.validade_inicial validade_inicial,
	                ds.validade_final validade_final,
	                ds.tipo_doc_idtipo_doc tipo_doc_id,
	                td.nome_doc tipo_doc_nome,
	                ds.cnae_doc atividades_economicas_json,
	                ds.cliente_estabelecimento cliente_estabelecimento,
	                ds.sind_laboral,
	                ds.sind_patronal,
	                ds.referencia,
	                REPLACE(REPLACE(REPLACE(JSON_EXTRACT(ds.cnae_doc, '$[*].subclasse') , '\""', ''), '[', ''), ']', '')  AS atividades_economicas,
	                REPLACE(REPLACE(REPLACE(JSON_EXTRACT(ds.sind_laboral, '$[*].sigla'), '\""', ''), '[', ''), ']', '')  AS siglas_sindicatos_laborais,
	                REPLACE(REPLACE(REPLACE(JSON_EXTRACT(ds.sind_patronal, '$[*].sigla'), '\""', ''), '[', ''), ']', '')  AS siglas_sindicatos_patronais
                FROM doc_sind ds
                LEFT JOIN tipo_doc td on td.idtipo_doc = ds.tipo_doc_idtipo_doc
                WHERE ds.modulo = 'SISAP'
                      AND ds.validade_final <> '0000-00-00'
            ");
        }
    }
}
