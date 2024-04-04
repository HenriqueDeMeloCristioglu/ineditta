using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ineditta.Repository.Sindicatos.Views
{
    public class SindicatoVwMap : IEntityTypeConfiguration<SindicatoVw>
    {
        public void Configure(EntityTypeBuilder<SindicatoVw> builder)
        {
            builder.ToView("sindicato_vw");

            builder.HasNoKey();

            builder.Property(e => e.SindLaboralSituacao).HasColumnName("sind_laboral_situacao");

            builder.Property(e => e.SindLaboralCnpj).HasColumnName("sind_laboral_cnpj");

            builder.Property(e => e.SindLaboralCodigo).HasColumnName("sind_laboral_codigo");

            builder.Property(e => e.SindLaboralSigla).HasColumnName("sind_laboral_sigla");

            builder.Property(e => e.SindLaboralRazao).HasColumnName("sind_laboral_razao");

            builder.Property(e => e.SindLaboralDenominacao).HasColumnName("sind_laboral_denominacao");

            builder.Property(e => e.SindLaboralLogradouro).HasColumnName("sind_laboral_logradouro");

            builder.Property(e => e.SindLaboralMunicipio).HasColumnName("sind_laboral_municipio");

            builder.Property(e => e.SindLaboralUf).HasColumnName("sind_laboral_uf");

            builder.Property(e => e.SindLaboralFone1).HasColumnName("sind_laboral_fone1");

            builder.Property(e => e.SindLaboralFone2).HasColumnName("sind_laboral_fone2");

            builder.Property(e => e.SindLaboralFone3).HasColumnName("sind_laboral_fone3");

            builder.Property(e => e.SindLaboralRamal).HasColumnName("sind_laboral_ramal");

            builder.Property(e => e.SindLaboralNegociador).HasColumnName("sind_laboral_negociador");

            builder.Property(e => e.SindLaboralContribuicao).HasColumnName("sind_laboral_contribuicao");

            builder.Property(e => e.SindLaboralEnquadramento).HasColumnName("sind_laboral_enquadramento");

            builder.Property(e => e.SindLaboralEmail1).HasColumnName("sind_laboral_email1");

            builder.Property(e => e.SindLaboralEmail2).HasColumnName("sind_laboral_email2");

            builder.Property(e => e.SindLaboralEmail3).HasColumnName("sind_laboral_email3");

            builder.Property(e => e.SindLaboralSite).HasColumnName("sind_laboral_site");

            builder.Property(e => e.SindLaboralTwitter).HasColumnName("sind_laboral_twitter");

            builder.Property(e => e.SindLaboralFacebook).HasColumnName("sind_laboral_facebook");

            builder.Property(e => e.SindLaboralInstagram).HasColumnName("sind_laboral_instagram");

            builder.Property(e => e.SindLaboralGrau).HasColumnName("sind_laboral_grau");

            builder.Property(e => e.SindLaboralStatus).HasColumnName("sind_laboral_status");

            builder.Property(e => e.SindLaboralDataNegociacao).HasColumnName("dataneg");

            builder.Property(e => e.SindPatronalSituacao).HasColumnName("sind_patronal_situacao");

            builder.Property(e => e.SindPatronalCnpj).HasColumnName("sind_patronal_cnpj");

            builder.Property(e => e.SindPatronalCodigo).HasColumnName("sind_patronal_codigo");

            builder.Property(e => e.SindPatronalSigla).HasColumnName("sind_patronal_sigla");

            builder.Property(e => e.SindPatronalRazao).HasColumnName("sind_patronal_razao");

            builder.Property(e => e.SindPatronalDenominacao).HasColumnName("sind_patronal_denominacao");

            builder.Property(e => e.SindPatronalLogradouro).HasColumnName("sind_patronal_logradouro");

            builder.Property(e => e.SindPatronalMunicipio).HasColumnName("sind_patronal_municipio");

            builder.Property(e => e.SindPatronalUf).HasColumnName("sind_patronal_uf");

            builder.Property(e => e.SindPatronalFone1).HasColumnName("sind_patronal_fone1");

            builder.Property(e => e.SindPatronalFone2).HasColumnName("sind_patronal_fone2");

            builder.Property(e => e.SindPatronalFone3).HasColumnName("sind_patronal_fone3");

            builder.Property(e => e.SindPatronalRamal).HasColumnName("sind_patronal_ramal");

            builder.Property(e => e.SindPatronalNegociador).HasColumnName("sind_patronal_negociador");

            builder.Property(e => e.SindPatronalContribuicao).HasColumnName("sind_patronal_contribuicao");

            builder.Property(e => e.SindPatronalEnquadramento).HasColumnName("sind_patronal_enquadramento");

            builder.Property(e => e.SindPatronalEmail1).HasColumnName("sind_patronal_email1");

            builder.Property(e => e.SindPatronalEmail2).HasColumnName("sind_patronal_email2");

            builder.Property(e => e.SindPatronalEmail3).HasColumnName("sind_patronal_email3");

            builder.Property(e => e.SindPatronalSite).HasColumnName("sind_patronal_site");

            builder.Property(e => e.SindPatronalTwitter).HasColumnName("sind_patronal_twitter");

            builder.Property(e => e.SindPatronalFacebook).HasColumnName("sind_patronal_facebook");

            builder.Property(e => e.SindPatronalInstagram).HasColumnName("sind_patronal_instagram");

            builder.Property(e => e.SindPatronalGrau).HasColumnName("sind_patronal_grau");

            builder.Property(e => e.SindPatronalStatus).HasColumnName("sind_patronal_status");

            builder.Property(e => e.FederacaoLaboralSigla).HasColumnName("federacao_laboral_sigla");

            builder.Property(e => e.FederacaoLaboralNome).HasColumnName("federacao_laboral_nome");

            builder.Property(e => e.FederacaoLaboralCnpj).HasColumnName("federacao_laboral_cnpj");

            builder.Property(e => e.ConfederacaoLaboralSigla).HasColumnName("confederacao_laboral_sigla");

            builder.Property(e => e.ConfederacaoLaboralNome).HasColumnName("confederacao_laboral_nome");

            builder.Property(e => e.ConfederacaoLaboralCnpj).HasColumnName("confederacao_laboral_cnpj");

            builder.Property(e => e.CentralSindicalSigla).HasColumnName("central_sindical_sigla");

            builder.Property(e => e.CentralSindicalNome).HasColumnName("central_sindical_nome");

            builder.Property(e => e.CentralSindicalCnpj).HasColumnName("central_sindical_cnpj");

            builder.Property(e => e.FederacaoPatronalSigla).HasColumnName("federacao_patronal_sigla");

            builder.Property(e => e.FederacaoPatronalNome).HasColumnName("federacao_patronal_nome");

            builder.Property(e => e.FederacaoPatronalCnpj).HasColumnName("federacao_patronal_cnpj");

            builder.Property(e => e.ConfederacaoPatronalSigla).HasColumnName("confederacao_patronal_sigla");

            builder.Property(e => e.ConfederacaoPatronalNome).HasColumnName("confederacao_patronal_nome");

            builder.Property(e => e.ConfederacaoPatronalCnpj).HasColumnName("confederacao_patronal_cnpj");
        }
    }
}
