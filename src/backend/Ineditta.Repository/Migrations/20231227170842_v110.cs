using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ineditta.Repository.Migrations
{
    /// <inheritdoc />
    public partial class v110 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"ALTER TABLE __EFMigrationsHistory CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
                ALTER TABLE _bd_legado_busca_rapida CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
                ALTER TABLE _bd_legado_clausulas CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
                ALTER TABLE abrang_docsind CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
                ALTER TABLE abrangencia_documento CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
                ALTER TABLE acompanhamento_cliente CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
                ALTER TABLE acompanhamento_envolvidos_emp CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
                ALTER TABLE acompanhamento_envolvidos_patr CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
                ALTER TABLE acompanhanto_cct CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
                ALTER TABLE ad_tipoinformacaoadicional CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
                ALTER TABLE anuencia_inicial CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
                ALTER TABLE anuencia_usuarios CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
                ALTER TABLE associacao CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
                ALTER TABLE assunto CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
                ALTER TABLE atividades CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
                ALTER TABLE atividades_comentarios CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
                ALTER TABLE base_territorialsindemp CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
                ALTER TABLE base_territorialsindpatro CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
                ALTER TABLE cadastro_clientes CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
                ALTER TABLE calendario_geral_novo CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
                ALTER TABLE calendario_sindical_notificacao_tb CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
                ALTER TABLE calendario_sindical_tb CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
                ALTER TABLE calendario_sindical_usuario_tb CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
                ALTER TABLE central_sindical CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
                ALTER TABLE classe_cnae CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
                ALTER TABLE classe_cnae_doc_sind CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
                ALTER TABLE clausula_geral CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
                ALTER TABLE clausula_geral_estrutura_clausula CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
                ALTER TABLE cliente_grupo CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
                ALTER TABLE cliente_matriz CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
                ALTER TABLE cliente_unidades CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
                ALTER TABLE cnae_emp CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
                ALTER TABLE comentarios_tb CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
                ALTER TABLE dados_sdf CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
                ALTER TABLE DataProtectionKeys CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
                ALTER TABLE doc_sind CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
                ALTER TABLE doc_sind_cliente_unidades CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
                ALTER TABLE doc_sind_referencia CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
                ALTER TABLE doc_sind_sind_emp CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
                ALTER TABLE doc_sind_sind_patr CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
                ALTER TABLE documento_assunto CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
                ALTER TABLE documento_sindicato_mais_recente_usuario_tb CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
                ALTER TABLE documentos CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
                ALTER TABLE documentos_abrangencia CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
                ALTER TABLE documentos_cnae CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
                ALTER TABLE documentos_empresa CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
                ALTER TABLE documentos_localizados CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
                ALTER TABLE estrutura_clausula CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
                ALTER TABLE estrutura_clausulas_ad_tipoinformacaoadicional CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
                ALTER TABLE etiqueta_tb CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
                ALTER TABLE fase_cct CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
                ALTER TABLE feriados CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
                ALTER TABLE filtro_csv CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
                ALTER TABLE formulario_folha CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
                ALTER TABLE formulario_grupo CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
                ALTER TABLE grupo_clausula CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
                ALTER TABLE helpdesk CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
                ALTER TABLE idempotent_tb CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
                ALTER TABLE indecon CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
                ALTER TABLE indecon_real CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
                ALTER TABLE informacao_adicional_cliente_tb CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
                ALTER TABLE informacao_adicional_combo CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
                ALTER TABLE informacao_adicional_grupo CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
                ALTER TABLE informacoes_adicionais CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
                ALTER TABLE jfase CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
                ALTER TABLE jfase_tipodados_perguntas_legendas CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
                ALTER TABLE jornada CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
                ALTER TABLE localizacao CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
                ALTER TABLE modulos CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
                ALTER TABLE modulos_cliente CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
                ALTER TABLE negociacao CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
                ALTER TABLE negociacao_calculo CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
                ALTER TABLE negociacao_cenarios CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
                ALTER TABLE negociacao_pauta CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
                ALTER TABLE negociacao_premissas CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
                ALTER TABLE negociacao_rodada CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
                ALTER TABLE negociacao_script CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
                ALTER TABLE ninfoadicionais CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
                ALTER TABLE restrito_usuario CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
                ALTER TABLE sind_diremp CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
                ALTER TABLE sind_dirpatro CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
                ALTER TABLE sind_emp CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
                ALTER TABLE sind_patr CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
                ALTER TABLE sinonimos CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
                ALTER TABLE tarefas CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
                ALTER TABLE temporario_clausulageral CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
                ALTER TABLE tipo_doc CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
                ALTER TABLE tipo_etiqueta_tb CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
                ALTER TABLE tipounidade_cliente CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
                ALTER TABLE usuario_adm CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
                ALTER TABLE wh_calendario CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;");

            migrationBuilder.Sql(@"DROP FUNCTION IF EXISTS ConvertMonthYear_ENtoPT;");

            migrationBuilder.Sql(@"CREATE FUNCTION ConvertMonthYear_ENtoPT(en_monthYear VARCHAR(10)) RETURNS varchar(10) CHARSET utf8mb4 COLLATE utf8mb4_0900_ai_ci
                BEGIN
                    RETURN CASE 
                        WHEN UPPER(en_monthYear) LIKE 'JAN/%' THEN REPLACE(en_monthYear, 'Jan', 'JAN')
                        WHEN UPPER(en_monthYear) LIKE 'FEB/%' THEN REPLACE(en_monthYear, 'Feb', 'FEV')
                        WHEN UPPER(en_monthYear) LIKE 'MAR/%' THEN REPLACE(en_monthYear, 'Mar', 'MAR')
                        WHEN UPPER(en_monthYear) LIKE 'APR/%' THEN REPLACE(en_monthYear, 'Apr', 'ABR')
                        WHEN UPPER(en_monthYear) LIKE 'MAY/%' THEN REPLACE(en_monthYear, 'May', 'MAI')
                        WHEN UPPER(en_monthYear) LIKE 'JUN/%' THEN REPLACE(en_monthYear, 'Jun', 'JUN')
                        WHEN UPPER(en_monthYear) LIKE 'JUL/%' THEN REPLACE(en_monthYear, 'Jul', 'JUL')
                        WHEN UPPER(en_monthYear) LIKE 'AUG/%' THEN REPLACE(en_monthYear, 'Aug', 'AGO')
                        WHEN UPPER(en_monthYear) LIKE 'SEP/%' THEN REPLACE(en_monthYear, 'Sep', 'SET')
                        WHEN UPPER(en_monthYear) LIKE 'OCT/%' THEN REPLACE(en_monthYear, 'Oct', 'OUT')
                        WHEN UPPER(en_monthYear) LIKE 'NOV/%' THEN REPLACE(en_monthYear, 'Nov', 'NOV')
                        WHEN UPPER(en_monthYear) LIKE 'DEC/%' THEN REPLACE(en_monthYear, 'Dec', 'DEZ')
                        ELSE en_monthYear
                    END;
                END;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"ALTER TABLE __EFMigrationsHistory CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
                            ALTER TABLE _bd_legado_busca_rapida CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
                            ALTER TABLE _bd_legado_clausulas CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
                            ALTER TABLE abrang_docsind CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
                            ALTER TABLE abrangencia_documento CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
                            ALTER TABLE acompanhamento_cliente CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
                            ALTER TABLE acompanhamento_envolvidos_emp CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
                            ALTER TABLE acompanhamento_envolvidos_patr CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
                            ALTER TABLE acompanhanto_cct CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
                            ALTER TABLE ad_tipoinformacaoadicional CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
                            ALTER TABLE anuencia_inicial CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
                            ALTER TABLE anuencia_usuarios CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
                            ALTER TABLE associacao CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
                            ALTER TABLE assunto CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
                            ALTER TABLE atividades CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
                            ALTER TABLE atividades_comentarios CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
                            ALTER TABLE base_territorialsindemp CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
                            ALTER TABLE base_territorialsindpatro CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
                            ALTER TABLE cadastro_clientes CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
                            ALTER TABLE calendario_geral_novo CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
                            ALTER TABLE calendario_sindical_notificacao_tb CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
                            ALTER TABLE calendario_sindical_tb CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
                            ALTER TABLE calendario_sindical_usuario_tb CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
                            ALTER TABLE central_sindical CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
                            ALTER TABLE classe_cnae CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
                            ALTER TABLE classe_cnae_doc_sind CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
                            ALTER TABLE clausula_geral CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
                            ALTER TABLE clausula_geral_estrutura_clausula CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
                            ALTER TABLE cliente_grupo CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
                            ALTER TABLE cliente_matriz CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
                            ALTER TABLE cliente_unidades CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
                            ALTER TABLE cnae_emp CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
                            ALTER TABLE comentarios_tb CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
                            ALTER TABLE dados_sdf CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
                            ALTER TABLE DataProtectionKeys CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
                            ALTER TABLE doc_sind CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
                            ALTER TABLE doc_sind_cliente_unidades CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
                            ALTER TABLE doc_sind_referencia CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
                            ALTER TABLE doc_sind_sind_emp CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
                            ALTER TABLE doc_sind_sind_patr CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
                            ALTER TABLE documento_assunto CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
                            ALTER TABLE documento_sindicato_mais_recente_usuario_tb CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
                            ALTER TABLE documentos CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
                            ALTER TABLE documentos_abrangencia CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
                            ALTER TABLE documentos_cnae CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
                            ALTER TABLE documentos_empresa CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
                            ALTER TABLE documentos_localizados CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
                            ALTER TABLE estrutura_clausula CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
                            ALTER TABLE estrutura_clausulas_ad_tipoinformacaoadicional CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
                            ALTER TABLE etiqueta_tb CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
                            ALTER TABLE fase_cct CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
                            ALTER TABLE feriados CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
                            ALTER TABLE filtro_csv CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
                            ALTER TABLE formulario_folha CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
                            ALTER TABLE formulario_grupo CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
                            ALTER TABLE grupo_clausula CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
                            ALTER TABLE helpdesk CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
                            ALTER TABLE idempotent_tb CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
                            ALTER TABLE indecon CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
                            ALTER TABLE indecon_real CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
                            ALTER TABLE informacao_adicional_cliente_tb CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
                            ALTER TABLE informacao_adicional_combo CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
                            ALTER TABLE informacao_adicional_grupo CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
                            ALTER TABLE informacoes_adicionais CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
                            ALTER TABLE jfase CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
                            ALTER TABLE jfase_tipodados_perguntas_legendas CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
                            ALTER TABLE jornada CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
                            ALTER TABLE localizacao CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
                            ALTER TABLE modulos CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
                            ALTER TABLE modulos_cliente CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
                            ALTER TABLE negociacao CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
                            ALTER TABLE negociacao_calculo CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
                            ALTER TABLE negociacao_cenarios CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
                            ALTER TABLE negociacao_pauta CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
                            ALTER TABLE negociacao_premissas CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
                            ALTER TABLE negociacao_rodada CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
                            ALTER TABLE negociacao_script CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
                            ALTER TABLE ninfoadicionais CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
                            ALTER TABLE restrito_usuario CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
                            ALTER TABLE sind_diremp CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
                            ALTER TABLE sind_dirpatro CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
                            ALTER TABLE sind_emp CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
                            ALTER TABLE sind_patr CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
                            ALTER TABLE sinonimos CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
                            ALTER TABLE tarefas CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
                            ALTER TABLE temporario_clausulageral CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
                            ALTER TABLE tipo_doc CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
                            ALTER TABLE tipo_etiqueta_tb CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
                            ALTER TABLE tipounidade_cliente CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
                            ALTER TABLE usuario_adm CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
                            ALTER TABLE wh_calendario CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;");

            migrationBuilder.Sql(@"DROP FUNCTION IF EXISTS ConvertMonthYear_ENtoPT;");

            migrationBuilder.Sql(@"CREATE FUNCTION ConvertMonthYear_ENtoPT(en_monthYear VARCHAR(10)) RETURNS varchar(10) CHARSET utf8mb4 COLLATE utf8mb4_unicode_ci
                BEGIN
                    RETURN CASE 
                        WHEN UPPER(en_monthYear) LIKE 'JAN/%' THEN REPLACE(en_monthYear, 'Jan', 'JAN')
                        WHEN UPPER(en_monthYear) LIKE 'FEB/%' THEN REPLACE(en_monthYear, 'Feb', 'FEV')
                        WHEN UPPER(en_monthYear) LIKE 'MAR/%' THEN REPLACE(en_monthYear, 'Mar', 'MAR')
                        WHEN UPPER(en_monthYear) LIKE 'APR/%' THEN REPLACE(en_monthYear, 'Apr', 'ABR')
                        WHEN UPPER(en_monthYear) LIKE 'MAY/%' THEN REPLACE(en_monthYear, 'May', 'MAI')
                        WHEN UPPER(en_monthYear) LIKE 'JUN/%' THEN REPLACE(en_monthYear, 'Jun', 'JUN')
                        WHEN UPPER(en_monthYear) LIKE 'JUL/%' THEN REPLACE(en_monthYear, 'Jul', 'JUL')
                        WHEN UPPER(en_monthYear) LIKE 'AUG/%' THEN REPLACE(en_monthYear, 'Aug', 'AGO')
                        WHEN UPPER(en_monthYear) LIKE 'SEP/%' THEN REPLACE(en_monthYear, 'Sep', 'SET')
                        WHEN UPPER(en_monthYear) LIKE 'OCT/%' THEN REPLACE(en_monthYear, 'Oct', 'OUT')
                        WHEN UPPER(en_monthYear) LIKE 'NOV/%' THEN REPLACE(en_monthYear, 'Nov', 'NOV')
                        WHEN UPPER(en_monthYear) LIKE 'DEC/%' THEN REPLACE(en_monthYear, 'Dec', 'DEZ')
                        ELSE en_monthYear
                    END;
                END;");
        }
    }
}
