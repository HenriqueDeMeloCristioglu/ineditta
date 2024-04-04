using System.Text;

namespace Ineditta.API.Factories.AcompanhamentosCcts
{
    public class RelatorioAcompanhamentoCctFactory
    {
        public StringBuilder Criar()
        {
            var stringBuilder = new StringBuilder(@"select vw.*
                                                        from acompanhamento_cct_vw as vw
                                                        inner join usuario_adm uat on uat.email_usuario = @email
                                                        where exists(
                                                        select 1 from cliente_unidades cut 
                                                        left join sind_emp slt on slt.id_sinde = vw.id_sindicato_laboral 
                                                        left join sind_patr spt on spt.id_sindp = vw.id_sindicato_patronal
                                                        left join base_territorialsindpatro btspt on btspt.sind_patronal_id_sindp = spt.id_sindp
                                                        left join base_territorialsindemp btset on btset.sind_empregados_id_sinde1 = slt.id_sinde
                                                        left join cnae_emp cet on cet.cliente_unidades_id_unidade = cut.id_unidade and cet.classe_cnae_idclasse_cnae = btset.classe_cnae_idclasse_cnae
                                                        where cut.localizacao_id_localizacao = btset.localizacao_id_localizacao1
                                                        and case when uat.nivel = @nivel then true
                                                                 else (vw.ids_cnaes is null or JSON_CONTAINS(vw.ids_cnaes, JSON_ARRAY(CAST(cet.classe_cnae_idclasse_cnae AS char))))
                                                            end
                                                        and exists(select 1 from cnae_emp cet where cet.cliente_unidades_id_unidade = cut.id_unidade and cet.classe_cnae_idclasse_cnae = btset.classe_cnae_idclasse_cnae)
                                                        and case when vw.id_sindicato_patronal is null then true else exists(select 1 from cnae_emp cet where cet.cliente_unidades_id_unidade = cut.id_unidade and cet.classe_cnae_idclasse_cnae = btspt.classe_cnae_idclasse_cnae) end
                                                        and case when uat.nivel = @nivel then true
                                                                    when uat.nivel = @nivelGrupoEconomico then uat.id_grupoecon = cut.cliente_grupo_id_grupo_economico
                                                                    else JSON_CONTAINS(uat.ids_fmge, JSON_ARRAY(CAST(cut.id_unidade AS char))) end)
                                                        and vw.fase <> 'Arquivada'");

            return stringBuilder;
        }
    }
}
