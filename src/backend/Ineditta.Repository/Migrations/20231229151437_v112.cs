using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ineditta.Repository.Migrations
{
    /// <inheritdoc />
    public partial class v112 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP VIEW IF EXISTS acompanhamento_cct_vw");

            migrationBuilder.Sql(@"CREATE OR REPLACE
                                    ALGORITHM = UNDEFINED VIEW acompanhamento_cct_vw AS
                                    select
                                        cct.idacompanhanto_cct AS id,
                                        sempt.codigo_sinde as codigo_sindicato_laboral,
                                        sempt.sigla_sinde AS sigla_sindicato_laboral,
                                        sempt.cnpj_sinde AS cnpj_sindicato_laboral,
                                        spt.codigo_sp as codigo_sindicato_patronal,
                                        spt.sigla_sp AS sigla_sindicato_patronal,
                                        spt.cnpj_sp AS cnpj_sindicato_patronal,
                                        spt.id_sindp AS id_sindicato_patronal,
                                        spt.uf_sp AS uf_sindicato_patronal,
                                        sempt.id_sinde AS id_sindicato_laboral,
                                        sempt.uf_sinde AS uf_sindicato_laboral,
                                        tdt.nome_doc AS nome_documento,
                                        cct.data_base AS data_base,
                                        cct.fase AS fase,
                                        cct.observacoes_gerais AS observacoes_gerais,
                                        date_format(cct.ultima_atualizacao, '%d/%m/%Y') AS ultima_atualizacao,
                                        ConvertMonthYear_ENtoPT(ir.periodo_data) AS periodo_anterior,
                                        ir.indicador AS indicador,
                                        ir.dado_real AS dado_real,
                                        (convert(date_format((ir.periodo_data + interval 1 month), '%b/%Y')
                                            using utf8mb4) collate utf8mb4_0900_ai_ci) AS ir_periodo,
                                        cnaet.atividades_economicas AS atividades_economicas,
                                        cct.ids_cnaes AS ids_cnaes
                                    from
                                        (((((acompanhanto_cct cct
                                    left join sind_emp sempt on
                                        ((cct.sind_emp_id_sinde = sempt.id_sinde)))
                                    left join sind_patr spt on
                                        ((cct.sind_patr_id_sindp = spt.id_sindp)))
                                    left join tipo_doc tdt on
                                        ((cct.tipo_doc_idtipo_doc = tdt.idtipo_doc)))
                                    left join indecon_real ir on
                                        (((ir.periodo_data is not null)
                                            and (cct.data_base = ConvertMonthYear_ENtoPT(date_format((ir.periodo_data + interval 1 month), '%b/%Y')))
                                                and (ir.indicador = 'INPC'))))
                                    left join lateral (
                                        select
                                            group_concat(cnaet.descricao_subclasse separator ', ') AS atividades_economicas
                                        from
                                            classe_cnae cnaet
                                        where
                                            json_contains(cct.ids_cnaes,
                                            concat('[""', cast(cnaet.id_cnae as char charset utf8mb4), '""]'))) cnaet on
                                        (true));");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "tip_doc",
                table: "cliente_matriz",
                type: "json",
                nullable: false,
                collation: "utf8mb4_0900_ai_ci")
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
