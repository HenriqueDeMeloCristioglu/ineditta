using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ineditta.Repository.Migrations
{
    /// <inheritdoc />
    public partial class v90 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"drop view if exists acompanhamento_cct_vw;");

            migrationBuilder.Sql(@"create or replace view acompanhamento_cct_vw as 
                                select cct.idacompanhanto_cct id,
                                sempt.sigla_sinde AS sigla_sindicato_laboral,
                                    sempt.cnpj_sinde AS cnpj_sindicato_laboral,
                                    spt.sigla_sp AS sigla_sindicato_patronal,
                                    spt.cnpj_sp AS cnpj_sindicato_patronal,
                                    spt.id_sindp AS id_sindicato_patronal,
                                    sempt.id_sinde AS id_sindicato_laboral,
                                    tdt.nome_doc AS nome_documento,
                                    cct.data_base AS data_base,
                                    cct.fase AS fase,
                                    cct.observacoes_gerais AS observacoes_gerais,
                                    date_format(cct.ultima_atualizacao, '%d/%m/%Y') AS ultima_atualizacao,
                                    ConvertMonthYear_ENtoPT(ir.periodo_data) AS periodo_anterior,
                                    ir.indicador AS indicador,
                                    ir.dado_real AS dado_real,
                                    date_format((ir.periodo_data + interval 1 month), '%b/%Y') AS ir_periodo,
                                    cnaet.atividades_economicas,
                                    cct.ids_cnaes
                                from acompanhanto_cct cct
                                left join sind_emp sempt on cct.sind_emp_id_sinde = sempt.id_sinde 
                                left join sind_patr spt on cct.sind_patr_id_sindp = spt.id_sindp 
                                left join tipo_doc tdt on cct.tipo_doc_idtipo_doc = tdt.idtipo_doc 
                                left join indecon_real ir 
	                                on ir.periodo_data is not null
                                    and cct.data_base = ConvertMonthYear_ENtoPT(date_format((ir.periodo_data + interval 1 month), '%b/%Y'))
                                    and ir.indicador = 'INPC'
                                left join lateral (select group_concat(cnaet.descricao_subclasse separator ', ') as atividades_economicas
				                                   from classe_cnae cnaet
				                                   where json_contains(cct.ids_cnaes, concat('[""', cast(cnaet.id_cnae as char), '""]'))) cnaet on true;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"drop view if exists acompanhamento_cct_vw;");

            migrationBuilder.Sql(@"create or replace view acompanhamento_cct_vw as 
                                select cct.idacompanhanto_cct id,
                                sempt.sigla_sinde AS sigla_sindicato_laboral,
                                    sempt.cnpj_sinde AS cnpj_sindicato_laboral,
                                    spt.sigla_sp AS sigla_sindicato_patronal,
                                    spt.cnpj_sp AS cnpj_sindicato_patronal,
                                    spt.id_sindp AS id_sindicato_patronal,
                                    sempt.id_sinde AS id_sindicato_laboral,
                                    tdt.nome_doc AS nome_documento,
                                    cct.data_base AS data_base,
                                    cct.fase AS fase,
                                    cct.observacoes_gerais AS observacoes_gerais,
                                    date_format(cct.ultima_atualizacao, '%d/%m/%Y') AS ultima_atualizacao,
                                    ConvertMonthYear_ENtoPT(ir.periodo_data) AS periodo_anterior,
                                    ir.indicador AS indicador,
                                    ir.dado_real AS dado_real,
                                    date_format((ir.periodo_data + interval 1 month), '%b/%Y') AS ir_periodo,
                                    cnaet.atividades_economicas
                                from acompanhanto_cct cct
                                left join sind_emp sempt on cct.sind_emp_id_sinde = sempt.id_sinde 
                                left join sind_patr spt on cct.sind_patr_id_sindp = spt.id_sindp 
                                left join tipo_doc tdt on cct.tipo_doc_idtipo_doc = tdt.idtipo_doc 
                                left join indecon_real ir 
	                                on ir.periodo_data is not null
                                    and cct.data_base = ConvertMonthYear_ENtoPT(date_format((ir.periodo_data + interval 1 month), '%b/%Y'))
                                    and ir.indicador = 'INPC'
                                left join lateral (select group_concat(cnaet.descricao_subclasse separator ', ') as atividades_economicas
				                                   from classe_cnae cnaet
				                                   where json_contains(cct.ids_cnaes, concat('[""', cast(cnaet.id_cnae as char), '""]'))) cnaet on true;");
        }
    }
}
