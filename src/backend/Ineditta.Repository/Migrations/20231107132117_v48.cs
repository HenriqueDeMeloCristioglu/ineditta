using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ineditta.Repository.Migrations
{
    /// <inheritdoc />
    public partial class v48 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"drop view if exists evento_calendario_vencimento_mandato_laboral_vw");
            migrationBuilder.Sql(@"drop view if exists evento_calendario_vencimento_mandato_patronal_vw");

            migrationBuilder.Sql(@"
                CREATE OR REPLACE VIEW evento_calendario_vencimento_mandato_laboral_vw AS
                SELECT
                    se.id_sinde sindicato_id,
	                sd.termino_mandatoe `data`,
	                'Ineditta' origem,
	                sd.inicio_mandatoe validade_inicial,
	                sd.termino_mandatoe validade_final,
	                sd.dirigente_e dirigente,
	                sd.funcao_e funcao,
	                (SELECT GROUP_CONCAT(CONCAT(' ',descricao_subclasse))
                    FROM base_territorialsindemp bt 
                    INNER JOIN classe_cnae cc on cc.id_cnae = bt.classe_cnae_idclasse_cnae
                    WHERE bt.sind_empregados_id_sinde1 = se.id_sinde) AS atividades_economicas,
	                se.sigla_sinde AS siglas_sindicatos_laborais,
	                se.id_sinde sindicato_laboral_id
                FROM sind_diremp sd
                INNER JOIN sind_emp se ON se.id_sinde = sd.sind_emp_id_sinde
            ");

            migrationBuilder.Sql(@"
                CREATE OR REPLACE VIEW evento_calendario_vencimento_mandato_patronal_vw AS
                SELECT
                    sp.id_sindp sindicato_id,
	                sd.termino_mandatop `data`,
	                'Ineditta' origem,
	                sd.inicio_mandatop validade_inicial,
	                sd.termino_mandatop validade_final,
	                sd.dirigente_p dirigente,
	                sd.funcao_p funcao,
	                (SELECT GROUP_CONCAT(CONCAT(' ',descricao_subclasse))
                    FROM base_territorialsindpatro bt
                    INNER JOIN classe_cnae cc on cc.id_cnae = bt.classe_cnae_idclasse_cnae
                    WHERE bt.sind_patronal_id_sindp = sp.id_sindp) AS atividades_economicas,
	                sp.sigla_sp AS siglas_sindicatos_patronais,
	                sp.id_sindp sindicato_patronal_id
                FROM sind_dirpatro sd
                INNER JOIN sind_patr sp ON sp.id_sindp = sd.sind_patr_id_sindp
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"drop view if exists evento_calendario_vencimento_mandato_laboral_vw");
            migrationBuilder.Sql(@"drop view if exists evento_calendario_vencimento_mandato_patronal_vw");

            migrationBuilder.Sql(@"
                CREATE OR REPLACE VIEW evento_calendario_vencimento_mandato_laboral_vw AS
                SELECT
	                sd.termino_mandatoe `data`,
	                'Ineditta' origem,
	                sd.inicio_mandatoe validade_inicial,
	                sd.termino_mandatoe validade_final,
	                sd.dirigente_e dirigente,
	                sd.funcao_e funcao,
	                (SELECT GROUP_CONCAT(CONCAT(' ',descricao_subclasse))
                    FROM base_territorialsindemp bt 
                    INNER JOIN classe_cnae cc on cc.id_cnae = bt.classe_cnae_idclasse_cnae
                    WHERE bt.sind_empregados_id_sinde1 = se.id_sinde) AS atividades_economicas,
	                se.sigla_sinde AS siglas_sindicatos_laborais,
	                se.id_sinde sindicato_laboral_id
                FROM sind_diremp sd
                INNER JOIN sind_emp se ON se.id_sinde = sd.sind_emp_id_sinde
            ");

            migrationBuilder.Sql(@"
                CREATE OR REPLACE VIEW evento_calendario_vencimento_mandato_patronal_vw AS
                SELECT
	                sd.termino_mandatop `data`,
	                'Ineditta' origem,
	                sd.inicio_mandatop validade_inicial,
	                sd.termino_mandatop validade_final,
	                sd.dirigente_p dirigente,
	                sd.funcao_p funcao,
	                (SELECT GROUP_CONCAT(CONCAT(' ',descricao_subclasse))
                    FROM base_territorialsindpatro bt
                    INNER JOIN classe_cnae cc on cc.id_cnae = bt.classe_cnae_idclasse_cnae
                    WHERE bt.sind_patronal_id_sindp = sp.id_sindp) AS atividades_economicas,
	                sp.sigla_sp AS siglas_sindicatos_patronais,
	                sp.id_sindp sindicato_patronal_id
                FROM sind_dirpatro sd
                INNER JOIN sind_patr sp ON sp.id_sindp = sd.sind_patr_id_sindp
            ");
        }
    }
}
