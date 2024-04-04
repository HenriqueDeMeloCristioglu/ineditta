using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ineditta.Repository.Migrations
{
    /// <inheritdoc />
    public partial class v188 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW IF EXISTS informacoes_estabelecimentos_vw;");
            migrationBuilder.Sql(@"
                CREATE OR REPLACE VIEW informacoes_estabelecimentos_vw AS
                SELECT
                    cut.id_unidade UnidadeId,
	                cut.codigo_unidade CodigoUnidade,
	                cut.nome_unidade NomeUnidade,
	                cut.cnpj_unidade CnpjUnidade,
	                cut.cod_sindcliente CodigoSindicatoLaboral,
	                sindicato_laboral.value SindicatosLaborais,
	                sindicato_laboral.siglas SindicatosLaboraisSiglas,
	                sindicato_patronal.value SindicatosPatronais,
	                sindicato_patronal.siglas SindicatosPatronaisSiglas,
	                datas_bases.value datasBases,
                    ua.email_usuario EmailUsuario
                FROM cliente_unidades cut
                INNER JOIN usuario_adm ua ON TRUE
                LEFT JOIN LATERAL (
	                SELECT 
		                GROUP_CONCAT(DISTINCT bt.dataneg SEPARATOR ', ') value
	                FROM sind_emp se 
	                LEFT JOIN base_territorialsindemp bt ON bt.sind_empregados_id_sinde1 = se.id_sinde 
	                WHERE EXISTS (
	                    SELECT 1 FROM cnae_emp ce 
	                    WHERE cut.id_unidade = ce.cliente_unidades_id_unidade
	    	                  AND ce.classe_cnae_idclasse_cnae = bt.classe_cnae_idclasse_cnae 
	                          AND cut.localizacao_id_localizacao = bt.localizacao_id_localizacao1
	                )
                ) datas_bases ON true
                LEFT JOIN LATERAL (
	                SELECT 
		                JSON_ARRAYAGG(JSON_OBJECT('id', se.id_sinde, 'sigla', se.sigla_sinde)) value, 
		                GROUP_CONCAT(DISTINCT se.sigla_sinde SEPARATOR ', ') siglas 
	                FROM sind_emp se 
	                LEFT JOIN base_territorialsindemp bt ON bt.sind_empregados_id_sinde1 = se.id_sinde 
	                WHERE EXISTS (
	                    SELECT 1 FROM cnae_emp ce 
	                    WHERE cut.id_unidade = ce.cliente_unidades_id_unidade
	    	                  AND ce.classe_cnae_idclasse_cnae = bt.classe_cnae_idclasse_cnae 
	                          AND cut.localizacao_id_localizacao = bt.localizacao_id_localizacao1
	                )
	                GROUP BY se.id_sinde
                ) sindicato_laboral ON true
                LEFT JOIN LATERAL (
	                SELECT 
		                JSON_ARRAYAGG(JSON_OBJECT('id', sp.id_sindp, 'sigla', sp.sigla_sp)) value,
		                GROUP_CONCAT(DISTINCT sp.sigla_sp SEPARATOR ', ') siglas
	                FROM sind_patr sp 
	                LEFT JOIN base_territorialsindpatro bt2 ON bt2.sind_patronal_id_sindp = sp.id_sindp 
	                WHERE EXISTS (
	                    SELECT 1 FROM cnae_emp ce 
	                    WHERE cut.id_unidade = ce.cliente_unidades_id_unidade
	    	                  AND ce.classe_cnae_idclasse_cnae = bt2.classe_cnae_idclasse_cnae 
	                          AND cut.localizacao_id_localizacao = bt2.localizacao_id_localizacao1
	                )
	                GROUP BY sp.id_sindp
                ) sindicato_patronal ON true
                WHERE JSON_CONTAINS(ua.ids_fmge, JSON_ARRAY(cut.id_unidade));
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW IF EXISTS informacoes_estabelecimentos_vw;");
        }
    }
}
