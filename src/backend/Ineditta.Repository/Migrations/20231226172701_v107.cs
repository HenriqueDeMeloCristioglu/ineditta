using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ineditta.Repository.Migrations
{
    /// <inheritdoc />
    public partial class v107 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP view if exists comentarios_vw;");

            migrationBuilder.Sql(@"CREATE OR REPLACE VIEW comentarios_vw as
                                    SELECT
                                        ct.id AS id,
                                        CASE ct.tipo
                                            WHEN 1 THEN 'Cláusula'
                                            WHEN 2 THEN 'Sindicato Laboral'
                                            WHEN 3 THEN 'Sindicato Patronal'
                                            WHEN 4 THEN 'Filial'
                                            ELSE ''
                                        END AS tipo,
                                        CASE ct.tipo_usuario_destino
                                            WHEN 1 THEN 'Grupo'
                                            WHEN 2 THEN 'Matriz'
                                            WHEN 3 THEN 'Unidade'
                                            ELSE ''
                                        END AS tipo_usuario_destino,
                                        CASE ct.tipo_notificacao
                                            WHEN 1 THEN 'Fixo'
                                            WHEN 2 THEN 'Temporario'
                                            ELSE ''
                                        END AS tipo_notificacao,
                                        ct.data_validade,
                                        ua.nome_usuario AS nome_usuario,
                                        ua.id_user AS usuario_id,
                                        ct.valor AS comentario,
                                        et.id AS etiqueta_id,
                                        et.nome AS etiqueta_nome,
                                        ct.visivel,
                                        ct.usuario_inclusao_id,
                                        ec.nome_clausula,
                                        se.sigla_sinde,
                                        sp.sigla_sp
                                    FROM comentarios_tb ct
                                    INNER JOIN usuario_adm ua ON ct.usuario_inclusao_id = ua.id_user
                                    INNER JOIN etiqueta_tb et ON ct.etiqueta_id = et.id
                                    LEFT JOIN clausula_geral cg ON ct.referencia_id = cg.id_clau AND ct.tipo = 1
                                    LEFT JOIN estrutura_clausula ec ON cg.estrutura_id_estruturaclausula = ec.id_estruturaclausula AND ct.tipo = 1
                                    LEFT JOIN sind_emp se ON ct.referencia_id = se.id_sinde AND ct.tipo = 2
                                    LEFT JOIN sind_patr sp ON ct.referencia_id = sp.id_sindp AND ct.tipo = 3;");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP view if exists comentarios_vw;");

            migrationBuilder.Sql(@"CREATE OR REPLACE VIEW comentarios_vw as
                                    SELECT
                                        ct.id,
                                        CASE ct.tipo
                                            WHEN 1 THEN 'Cláusula'
                                            WHEN 2 THEN 'Sindicato Laboral'
                                            WHEN 3 THEN 'Sindicato Patronal'
                                            WHEN 4 THEN 'Filial'
                                            ELSE ''
                                        END AS tipo,
                                        CASE ct.tipo_usuario_destino
                                            WHEN 1 THEN 'Grupo'
                                            WHEN 2 THEN 'Matriz'
                                            WHEN 3 THEN 'Unidade'
                                            ELSE ''
                                        END AS tipo_usuario_destino,
                                        CASE ct.tipo_notificacao
                                            WHEN 1 THEN 'Fixa'
                                            WHEN 2 THEN 'Temporaria'
                                            ELSE ''
                                        END AS tipo_notificacao,
                                        ct.data_validade,
                                        ua.nome_usuario AS nome_usuario,
                                        ua.id_user AS usuario_id,
                                        ct.valor AS comentario,
                                        ct.etiqueta_id,
                                        ct.visivel,
                                        ct.usuario_inclusao_id
                                    FROM comentarios_tb ct
                                    INNER JOIN usuario_adm ua ON ct.usuario_inclusao_id = ua.id_user;");

            migrationBuilder.Sql(@"DROP FUNCTION IF EXISTS ConvertMonthYear_ENtoPT;");

            migrationBuilder.Sql(@"CREATE FUNCTION ConvertMonthYear_ENtoPT(en_monthYear VARCHAR(10)) RETURNS varchar(10) CHARSET utf8mb4 COLLATE utf8mb4_unicode_ci
                                BEGIN
                                    RETURN CASE 
                                        WHEN UPPER(en_monthYear) LIKE 'JAN/%' THEN REPLACE(en_monthYear, 'Jan', 'JAN')
                                        WHEN UPPER(en_monthYear) LIKE 'FEV/%' THEN REPLACE(en_monthYear, 'Fev', 'FEV')
                                        WHEN UPPER(en_monthYear) LIKE 'MAR/%' THEN REPLACE(en_monthYear, 'Mar', 'MAR')
                                        WHEN UPPER(en_monthYear) LIKE 'ABR/%' THEN REPLACE(en_monthYear, 'Abr', 'ABR')
                                        WHEN UPPER(en_monthYear) LIKE 'MAI/%' THEN REPLACE(en_monthYear, 'Mai', 'MAI')
                                        WHEN UPPER(en_monthYear) LIKE 'JUN/%' THEN REPLACE(en_monthYear, 'Jun', 'JUN')
                                        WHEN UPPER(en_monthYear) LIKE 'JUL/%' THEN REPLACE(en_monthYear, 'Jul', 'JUL')
                                        WHEN UPPER(en_monthYear) LIKE 'AGO/%' THEN REPLACE(en_monthYear, 'Ago', 'AGO')
                                        WHEN UPPER(en_monthYear) LIKE 'SET/%' THEN REPLACE(en_monthYear, 'Set', 'SET')
                                        WHEN UPPER(en_monthYear) LIKE 'OUT/%' THEN REPLACE(en_monthYear, 'Out', 'OUT')
                                        WHEN UPPER(en_monthYear) LIKE 'NOV/%' THEN REPLACE(en_monthYear, 'Nov', 'NOV')
                                        WHEN UPPER(en_monthYear) LIKE 'DEZ/%' THEN REPLACE(en_monthYear, 'Dez', 'DEZ')
                                        ELSE en_monthYear
                                    END;
                                END;");
        }
    }
}
