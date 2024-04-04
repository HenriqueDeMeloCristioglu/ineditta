using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ineditta.Repository.Migrations
{
    /// <inheritdoc />
    public partial class v82 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE OR REPLACE VIEW dirigente_patronal_vw AS
	                                    select dirpatr.id_diretoriap id, dirpatr.dirigente_p nome, sindp.sigla_sp sigla, dirpatr.situacao_p situacao, 
		                                    cm.cliente_grupo_id_grupo_economico grupo, dirpatr.funcao_p cargo, cm.razao_social, dirpatr.inicio_mandatop inicio_mandato, 
		                                    dirpatr.termino_mandatop termino_mandato, cut.nome_unidade, cut.id_unidade unidade_id, dirpatr.sind_patr_id_sindp sindp_id from sind_dirpatro dirpatr
		                                    left join cliente_unidades cut on dirpatr.cliente_unidades_id_unidade = cut.id_unidade
		                                    left join cliente_matriz cm on cut.cliente_matriz_id_empresa = cm.id_empresa
		                                    left join sind_patr sindp on dirpatr.sind_patr_id_sindp = sindp.id_sindp 
		                                    order by dirpatr.dirigente_p asc;");

            migrationBuilder.Sql(@"CREATE OR REPLACE VIEW dirigente_laboral_vw AS
	                                    select diremp.id_diretoriae id, diremp.dirigente_e nome, sinde.sigla_sinde sigla, diremp.situacao_e situacao,
		                                    cm.cliente_grupo_id_grupo_economico grupo, diremp.funcao_e cargo, cm.razao_social, diremp.inicio_mandatoe inicio_mandato,
		                                    diremp.termino_mandatoe termino_mandato, cut.nome_unidade, cut.id_unidade unidade_id, diremp.sind_emp_id_sinde sinde_id from sind_diremp diremp
		                                    left join cliente_unidades cut on diremp.cliente_unidades_id_unidade = cut.id_unidade
		                                    left join cliente_matriz cm on cut.cliente_matriz_id_empresa = cm.id_empresa
		                                    left join sind_emp sinde on diremp.sind_emp_id_sinde = sinde.id_sinde
		                                    order by diremp.dirigente_e asc;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"drop view if exists dirigente_patronal_vw;");

            migrationBuilder.Sql(@"drop view if exists dirigente_laboral_vw;");
        }
    }
}
