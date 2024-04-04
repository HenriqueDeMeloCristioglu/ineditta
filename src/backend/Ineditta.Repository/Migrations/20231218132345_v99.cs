using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ineditta.Repository.Migrations
{
    /// <inheritdoc />
    public partial class v99 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE OR REPLACE VIEW sindicato_vw AS
	                                    select 
	                                        sinde.situacaocadastro_sinde sind_laboral_situacao, cnpj_sinde sind_laboral_cnpj, sinde.codigo_sinde sind_laboral_codigo,
	                                        sinde.sigla_sinde sind_laboral_sigla, sinde.razaosocial_sinde sind_laboral_razao, sinde.denominacao_sinde sind_laboral_denominacao,
	                                        sinde.logradouro_sinde sind_laboral_logradouro, sinde.municipio_sinde sind_laboral_municipio, sinde.uf_sinde sind_laboral_uf,
	                                        sinde.fone1_sinde sind_laboral_fone1, sinde.fone2_sinde sind_laboral_fone2, sinde.fone3_sinde sind_laboral_fone3,
	                                        sinde.ramal sind_laboral_ramal, sinde.negociador_sinde sind_laboral_negociador, sinde.contribuicao_sinde sind_laboral_contribuicao,
	                                        sinde.enquadramento_sinde sind_laboral_enquadramento, sinde.email1_sinde sind_laboral_email1, sinde.email2_sinde sind_laboral_email2,
	                                        sinde.email3_sinde sind_laboral_email3, sinde.site_sinde sind_laboral_site, sinde.twitter_sinde sind_laboral_twitter,
	                                        sinde.facebook_sinde sind_laboral_facebook, sinde.instagram_sinde sind_laboral_instagram, sinde.grau sind_laboral_grau, sinde.status sind_laboral_status,
	                                        sindp.situacaocadastro_sp sind_patronal_situacao, sindp.cnpj_sp sind_patronal_cnpj, sindp.codigo_sp sind_patronal_codigo,
	                                        sindp.sigla_sp sind_patronal_sigla, sindp.razaosocial_sp sind_patronal_razao, sindp.denominacao_sp sind_patronal_denominacao,
	                                        sindp.logradouro_sp sind_patronal_logradouro, sindp.municipio_sp sind_patronal_municipio, sindp.uf_sp sind_patronal_uf,
	                                        sindp.fone1_sp sind_patronal_fone1, sindp.fone2_sp sind_patronal_fone2, sindp.fone3_sp sind_patronal_fone3,
	                                        sindp.ramal sind_patronal_ramal, sindp.negociador_sp sind_patronal_negociador, sindp.contribuicao_sp sind_patronal_contribuicao,
	                                        sindp.enquadramento_sp sind_patronal_enquadramento, sindp.email1_sp sind_patronal_email1, sindp.email2_sp sind_patronal_email2,
	                                        sindp.email3_sp sind_patronal_email3, sindp.site_sp sind_patronal_site, sindp.twitter_sp sind_patronal_twitter,
	                                        sindp.facebook_sp sind_patronal_facebook, sindp.instagram_sp sind_patronal_instagram, sindp.grau_sp sind_patronal_grau,
	                                        sindp.status sind_patronal_status,
	                                        fedsinde.sigla federacao_laboral_sigla, fedsinde.nome federacao_laboral_nome, fedsinde.cnpj federacao_laboral_cnpj,
	                                        confsinde.sigla confederacao_laboral_sigla, confsinde.nome confederacao_laboral_nome, confsinde.cnpj confederacao_laboral_cnpj,
	                                        cs.sigla central_sindical_sigla, cs.nome_centralsindical central_sindical_nome, cs.cnpj central_sindical_cnpj, 
	                                        fedsindp.sigla federacao_patronal_sigla, fedsindp.nome federacao_patronal_nome, fedsindp.cnpj federacao_patronal_cnpj,
	                                        confsindp.sigla confederacao_patronal_sigla, confsindp.nome confederacao_patronal_nome, confsindp.cnpj confederacao_patronal_cnpj,
	                                        cut.cnae_unidade, bemp.classe_cnae_idclasse_cnae, cut.cliente_grupo_id_grupo_economico, cut.id_unidade, cut.cliente_matriz_id_empresa,
	                                        loc.id_localizacao, loc.uf, loc.regiao, sinde.id_sinde, bemp.sind_empregados_id_sinde1, bpatr.sind_patronal_id_sindp, sindp.id_sindp,
	                                        bemp.dataneg
	                                        from sind_emp sinde 
	                                        inner join base_territorialsindemp bemp on bemp.sind_empregados_id_sinde1 = sinde.id_sinde 
	                                        inner join base_territorialsindpatro bpatr on bpatr.localizacao_id_localizacao1 = bemp.localizacao_id_localizacao1
		                                        and bpatr.classe_cnae_idclasse_cnae = bemp.classe_cnae_idclasse_cnae
	                                        inner join sind_patr sindp on sindp.id_sindp = bpatr.sind_patronal_id_sindp
	                                        inner join localizacao loc on loc.id_localizacao = bemp.localizacao_id_localizacao1
	                                        inner join cliente_unidades cut on cut.localizacao_id_localizacao = loc.id_localizacao 
	                                        inner join classe_cnae cc on cc.id_cnae = bemp.classe_cnae_idclasse_cnae
                                            inner join cliente_matriz cm on cm.id_empresa = cut.cliente_matriz_id_empresa
                                            inner join cliente_grupo cg on cg.id_grupo_economico = cm.cliente_grupo_id_grupo_economico
                                            left join central_sindical cs on cs.id_centralsindical = sinde.central_sindical_id_centralsindical
                                            left join associacao fedsinde on fedsinde.id_associacao = sinde.federacao_id_associacao
                                            left join associacao confsinde on confsinde.id_associacao = sinde.confederacao_id_associacao
                                            left join associacao fedsindp on fedsindp.id_associacao = sindp.federacao_id_associacao
                                            left join associacao confsindp on confsindp.id_associacao = sindp.confederacao_id_associacao;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"drop view if exists sindicato_vw;");
        }
    }
}
