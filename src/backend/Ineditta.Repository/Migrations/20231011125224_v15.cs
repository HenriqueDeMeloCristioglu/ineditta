using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ineditta.Repository.Migrations
{
    /// <inheritdoc />
    public partial class v15 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP VIEW acompanhamento_cct_vw");

            migrationBuilder.Sql(@"CREATE FUNCTION ConvertMonthYear_ENtoPT(en_monthYear VARCHAR(10)) RETURNS VARCHAR(10)
                                BEGIN
                                    RETURN CASE 
                                        WHEN en_monthYear LIKE 'Jan/%' THEN REPLACE(en_monthYear, UPPER('Jan'), 'JAN')
                                        WHEN en_monthYear LIKE 'Feb/%' THEN REPLACE(en_monthYear, UPPER('Feb'), 'FEV')
                                        WHEN en_monthYear LIKE 'Mar/%' THEN REPLACE(en_monthYear, UPPER('Mar'), 'MAR')
                                        WHEN en_monthYear LIKE 'Apr/%' THEN REPLACE(en_monthYear, UPPER('Apr'), 'ABR')
                                        WHEN en_monthYear LIKE 'May/%' THEN REPLACE(en_monthYear, UPPER('May'), 'MAI')
                                        WHEN en_monthYear LIKE 'Jun/%' THEN REPLACE(en_monthYear, UPPER('Jun'), 'JUN')
                                        WHEN en_monthYear LIKE 'Jul/%' THEN REPLACE(en_monthYear, UPPER('Jul'), 'JUL')
                                        WHEN en_monthYear LIKE 'Aug/%' THEN REPLACE(en_monthYear, UPPER('Aug'), 'AGO')
                                        WHEN en_monthYear LIKE 'Sep/%' THEN REPLACE(en_monthYear, UPPER('Sep'), 'SET')
                                        WHEN en_monthYear LIKE 'Oct/%' THEN REPLACE(en_monthYear, UPPER('Oct'), 'OUT')
                                        WHEN en_monthYear LIKE 'Nov/%' THEN REPLACE(en_monthYear, UPPER('Nov'), 'NOV')
                                        WHEN en_monthYear LIKE 'Dec/%' THEN REPLACE(en_monthYear, UPPER('Dec'), 'DEZ')
                                        ELSE en_monthYear
                                    END;
                                END ");

            migrationBuilder.Sql(@"create or replace VIEW acompanhamento_cct_vw AS select cct.idacompanhanto_cct AS id,
                                slt.sigla_sinde sigla_sindicato_laboral,
                                slt.cnpj_sinde cnpj_sindicato_laboral,
                                spt.sigla_sp sigla_sindicato_patronal,
                                spt.cnpj_sp cnpj_sindicato_patronal,
                                spt.id_sindp id_sindicato_patronal,
                                slt.id_sinde id_sindicato_laboral,
                                tdt.nome_doc as nome_documento,
                                cct.data_base AS data_base,
                                cct.fase AS fase,
                                cct.observacoes_gerais AS observacoes_gerais,
                                date_format(cct.ultima_atualizacao, '%d/%m/%Y') AS ultima_atualizacao,
                                ConvertMonthYear_ENtoPT(ir.periodo_data) AS periodo_anterior,
                                ir.indicador AS indicador,
                                ir.dado_real AS dado_real,
                                date_format((ir.periodo_data + interval 1 month), '%b/%Y') ir_periodo,
                                 (
                                                        select
                                                            group_concat(table_group.descricao_subclasse separator ', ') as descricao_subclasse
                                                        from
                                                            (
                                                            select
                                                                t.idacompanhanto_cct AS idacompanhanto_cct,
                                                                t.ids_cnaes AS ids_cnaes,
                                                                data.Value AS Value,
                                                                cc.descricao_divisão  AS descricao_divisao,
                                                                cc.descricao_subclasse as descricao_subclasse
                                                            from
                                                                (acompanhanto_cct t
                                                            join (json_table(t.ids_cnaes,
                                                                '$[*]' columns (Value int path '$')) data
                                                            join classe_cnae cc on
                                                                ((cc.id_cnae = data.Value))))
                                                            where
                                                                (t.idacompanhanto_cct = cct.idacompanhanto_cct)) table_group
                                                        group by
                                                            NULL) AS atividades_economicas
                                from acompanhanto_cct cct
                                LEFT JOIN doc_sind dst ON cct.tipo_doc_idtipo_doc = dst.id_doc
                                LEFT JOIN tipo_doc tdt ON cct.tipo_doc_idtipo_doc = tdt.idtipo_doc
                                left join sind_emp slt on slt.id_sinde = cct.sind_emp_id_sinde
                                left join sind_patr spt on spt.id_sindp = cct.sind_patr_id_sindp
                                left join indecon_real ir on ir.periodo_data is not null and cct.data_base = ConvertMonthYear_ENtoPT(date_format((ir.periodo_data + interval 1 month), '%b/%Y')) and ir.indicador = 'INPC'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP VIEW if exists acompanhamento_cct_vw");
        }
    }
}
