using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ineditta.Repository.Migrations
{
    /// <inheritdoc />
    public partial class v13 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE OR REPLACE ALGORITHM = UNDEFINED VIEW acompanhamento_cct_vw AS
                    select
                        cct.idacompanhanto_cct AS id,
                        sd.sigla_sinde AS sigla_sindicato_laboral,
                        sd.cnpj_sinde AS cnpj_sindicato_laboral,
                        sp.sigla_sp AS sigla_sindicato_patronal,
                        sp.cnpj_sp AS cnpj_sindicato_patronal,
                        (
                        select
                            group_concat(table_group.descricao_subclasse separator ', ') as descricao_subclasse
                        from
                            (
                            select
                                t.idacompanhanto_cct AS idacompanhanto_cct,
                                t.ids_cnaes AS ids_cnaes,
                                data.Value AS Value,
                                cc.descricao_divisão AS descricao_divisao,
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
                            NULL) AS atividades_economicas,
                        td.tipo_doc as nome_documento,
                        cct.data_base AS data_base,
                        replace(replace(replace(replace(replace(replace(replace(replace(replace(replace(replace(replace(date_format(ir.periodo_data, '%b/%Y'), 'Jan', 'JAN'), 'Feb', 'FEV'), 'Mar', 'MAR'), 'Apr', 'ABR'), 'May', 'MAI'), 'Jun', 'JUN'), 'Jul', 'JUL'), 'Aug', 'AGO'), 'Sep', 'SET'), 'Oct', 'OUT'), 'Nov', 'NOV'), 'Dec', 'DEZ') AS periodo_anterior,
                        ir.dado_real AS dado_real,
                        cct.fase AS fase,
                        cct.observacoes_gerais AS observacoes_gerais,
                        ir.indicador AS indicador,
                        date_format(cct.ultima_atualizacao, '%d/%m/%Y') AS ultima_atualizacao,
                        cu.cliente_grupo_id_grupo_economico cliente_unidade_grupo_economico_id
                    from
                        ((((((acompanhanto_cct cct
                    LEFT JOIN doc_sind ds ON cct.tipo_doc_idtipo_doc = ds.id_doc
                    LEFT JOIN tipo_doc td ON cct.tipo_doc_idtipo_doc = td.idtipo_doc
                    left join sind_emp sd on
                        ((sd.id_sinde = cct.sind_emp_id_sinde)))
                    left join sind_patr sp on
                        ((sp.id_sindp = cct.sind_patr_id_sindp)))
                    left join base_territorialsindemp be on
                        ((be.sind_empregados_id_sinde1 = sd.id_sinde)))
                    left join base_territorialsindpatro bp on
                        ((bp.sind_patronal_id_sindp = sp.id_sindp)))
                    left join cliente_unidades cu on
                        (((cu.localizacao_id_localizacao = be.localizacao_id_localizacao1)
                            and json_contains(cu.cnae_unidade,
                            concat('{""id"":', be.classe_cnae_idclasse_cnae, '}'))
                                and(cu.localizacao_id_localizacao = bp.localizacao_id_localizacao1)
                                    and json_contains(cu.cnae_unidade,
                                    concat('{""id"":', bp.classe_cnae_idclasse_cnae, '}')))))
                    left join indecon_real ir on
                        (((ir.periodo_data is not null)
                            and(replace(replace(replace(replace(replace(replace(replace(replace(replace(replace(replace(replace(date_format((ir.periodo_data + interval 1 month), '%b/%Y'), 'Jan', 'JAN'), 'Feb', 'FEV'), 'Mar', 'MAR'), 'Apr', 'ABR'), 'May', 'MAI'), 'Jun', 'JUN'), 'Jul', 'JUL'), 'Aug', 'AGO'), 'Sep', 'SET'), 'Oct', 'OUT'), 'Nov', 'NOV'), 'Dec', 'DEZ') = cct.data_base))))");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP VIEW if exists acompanhamento_cct_vw");
        }
    }
}
