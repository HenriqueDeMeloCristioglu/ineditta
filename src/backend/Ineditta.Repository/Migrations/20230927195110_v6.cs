using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ineditta.Repository.Migrations
{
    /// <inheritdoc />
    public partial class V6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "anuencia",
                table: "tipo_doc",
                type: "varchar(2)",
                maxLength: 2,
                nullable: true,
                collation: "utf8mb3_general_ci")
                .Annotation("MySql:CharSet", "utf8mb3");

            migrationBuilder.Sql(@"
                CREATE OR REPLACE
                ALGORITHM = UNDEFINED VIEW acompanhamento_cct_vw AS
                select
                    cct.idacompanhanto_cct AS id,
                    sd.sigla_sinde AS sigla_sindicato_laboral,
                    sd.cnpj_sinde AS cnpj_sindicato_laboral,
                    sp.sigla_sp AS sigla_sindicato_patronal,
                    sp.cnpj_sp AS cnpj_sindicato_patronal,
                    (
                    select
                        group_concat(table_group.descricao_divisao separator ', ')
                    from
                        (
                        select
                            t.idacompanhanto_cct AS idacompanhanto_cct,
                            t.ids_cnaes AS ids_cnaes,
                            data.Value AS Value,
                            cc.descricao_divisão AS descricao_divisao
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
                    ds.titulo_documento as nome_documento,
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
                            and (cu.localizacao_id_localizacao = bp.localizacao_id_localizacao1)
                                and json_contains(cu.cnae_unidade,
                                concat('{""id"":', bp.classe_cnae_idclasse_cnae, '}')))))
                left join indecon_real ir on
                    (((ir.periodo_data is not null)
                        and (replace(replace(replace(replace(replace(replace(replace(replace(replace(replace(replace(replace(date_format((ir.periodo_data + interval 1 month), '%b/%Y'), 'Jan', 'JAN'), 'Feb', 'FEV'), 'Mar', 'MAR'), 'Apr', 'ABR'), 'May', 'MAI'), 'Jun', 'JUN'), 'Jul', 'JUL'), 'Aug', 'AGO'), 'Sep', 'SET'), 'Oct', 'OUT'), 'Nov', 'NOV'), 'Dec', 'DEZ') = cct.data_base))));");

            migrationBuilder.Sql(@"CREATE OR REPLACE
                ALGORITHM = UNDEFINED VIEW acompanhamento_cct_futuras_vw AS
                select
                    distinct se.sigla_sinde AS sigla_sinde,
                    sp.sigla_sp AS sigla_sp,
                    emp.sind_empregados_id_sinde1 AS sind_empregados_id_sinde1,
                    patr.sind_patronal_id_sindp AS sind_patronal_id_sindp,
                    emp.dataneg AS dataneg,
                    emp.classe_cnae_idclasse_cnae AS classe_cnae_idclasse_cnae,
                    cc.descricao_subclasse AS descricao_subclasse,
                    ifnull(usu.nome_usuario, '-') AS responsavel,
                    ifnull(usu.id_user, 0) AS responsavel_id,
                    date_format((select date_format((str_to_date(concat('01/', if((emp.dataneg = 'JAN'), 'Jan', if((emp.dataneg = 'FEV'), 'Feb', if((emp.dataneg = 'MAR'), 'Mar', if((emp.dataneg = 'ABR'), 'Apr', if((emp.dataneg = 'MAI'), 'May', if((emp.dataneg = 'JUN'), 'Jun', if((emp.dataneg = 'JUL'), 'Jul', if((emp.dataneg = 'AGO'), 'Aug', if((emp.dataneg = 'SET'), 'Sep', if((emp.dataneg = 'OUT'), 'Oct', if((emp.dataneg = 'NOV'), 'Nov', if((emp.dataneg = 'DEZ'), 'Dec', NULL)))))))))))), '/', year(now())), '%d/%b/%Y') - interval 0 month), '%Y-%m-01') AS first_day_of_prev_month), '%d/%m/%Y') AS data_ini
                from
                    ((((((base_territorialsindemp emp
                join base_territorialsindpatro patr on
                    (((emp.localizacao_id_localizacao1 = patr.localizacao_id_localizacao1)
                        and (emp.classe_cnae_idclasse_cnae = patr.classe_cnae_idclasse_cnae))))
                join sind_emp se on
                    ((se.id_sinde = emp.sind_empregados_id_sinde1)))
                join sind_patr sp on
                    ((sp.id_sindp = patr.sind_patronal_id_sindp)))
                join classe_cnae cc on
                    ((cc.id_cnae = emp.classe_cnae_idclasse_cnae)))
                join localizacao loc on
                    ((loc.id_localizacao = emp.localizacao_id_localizacao1)))
                left join usuario_adm usu on
                    ((json_contains(usu.ids_cnae,
                    concat('""', cc.divisao_cnae, '""'),
                    '$')
                        and json_contains(usu.ids_localidade,
                        concat('""', loc.cod_uf, '""'),
                        '$'))))
                where
                    (emp.dataneg like '%%')
                order by
                    data_ini;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "anuencia",
                table: "tipo_doc");

            migrationBuilder.Sql(@"DROP VIEW acompanhamento_cct_vw");

            migrationBuilder.Sql(@"DROP VIEW acompanhamento_cct_futuras_vw");
        }
    }
}
