using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ineditta.Repository.Migrations
{
    /// <inheritdoc />
    public partial class v19 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE OR REPLACE VIEW acompanhamento_cct_futuras_vw AS
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
                    data_ini");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"drop view acompanhamento_cct_futuras_vw");
        }
    }
}
