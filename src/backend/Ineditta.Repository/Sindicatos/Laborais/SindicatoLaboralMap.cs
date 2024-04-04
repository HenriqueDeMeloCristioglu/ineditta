using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CSharpFunctionalExtensions;

using Ineditta.Application.Sindicatos.Laborais.Entities;
using Ineditta.Repository.Converters;
using Ineditta.Repository.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ineditta.Repository.Sindicatos.Laborais
{
    internal sealed class SindicatoLaboralMap : IEntityTypeConfiguration<SindicatoLaboral>
    {
        public void Configure(EntityTypeBuilder<SindicatoLaboral> builder)
        {
            builder.HasKey(e => e.Id).HasName("PRIMARY");

            builder
                .ToTable("sind_emp")
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_general_ci");

            builder.HasIndex(e => e.ConfederacaoId, "associacao_id_associacao");
            builder.HasIndex(e => e.FederacaoId, "federacao_id_associacao");
            builder.HasIndex(e => e.CentralSindicalId, "central_sindical_id_centralsindical");

            builder.Property(e => e.Id).HasColumnName("id_sinde");

            builder.OwnsOne(p => p.Cnpj, cnpjBuilder =>
            {
                cnpjBuilder.Property(c => c.Value)
                            .HasMaxLength(20)
                            .HasColumnName("cnpj_sinde");
            });

            builder.OwnsOne(p => p.CodigoSindical, codigoSindicalBuilder =>
            {
                codigoSindicalBuilder.Property(c => c.Valor)
                            .HasMaxLength(125)
                            .HasColumnName("codigo_sinde");
            });

            builder.Property(e => e.Contribuicao)
                .HasMaxLength(45)
                .HasColumnName("contribuicao_sinde");

            builder.Property(e => e.Denominacao)
                .HasMaxLength(450)
                .HasColumnName("denominacao_sinde");

            builder.OwnsOne(p => p.Email1, emailBuilder =>
            {
                emailBuilder.Property(c => c.Valor)
                            .HasMaxLength(100)
                            .HasColumnName("email1_sinde");
            });
            builder.OwnsOne(p => p.Email2, emailBuilder =>
            {
                emailBuilder.Property(c => c.Valor)
                            .HasMaxLength(100)
                            .HasColumnName("email2_sinde");
            });
            builder.OwnsOne(p => p.Email3, emailBuilder =>
            {
                emailBuilder.Property(c => c.Valor)
                            .HasMaxLength(100)
                            .HasColumnName("email3_sinde");
            });

            builder.Property(e => e.Enquadramento)
                .HasMaxLength(45)
                .HasColumnName("enquadramento_sinde");

            builder.Property(e => e.Facebook)
                .HasMaxLength(45)
                .HasColumnName("facebook_sinde");

            builder.OwnsOne(p => p.Telefone1, telefoneBuilder =>
            {
                telefoneBuilder.Property(c => c.Valor)
                            .HasMaxLength(20)
                            .HasColumnName("fone1_sinde");
            });
            builder.OwnsOne(p => p.Telefone2, telefoneBuilder =>
            {
                telefoneBuilder.Property(c => c.Valor)
                            .HasMaxLength(20)
                            .HasColumnName("fone2_sinde");
            });
            builder.OwnsOne(p => p.Telefone3, telefoneBuilder =>
            {
                telefoneBuilder.Property(c => c.Valor)
                            .HasMaxLength(20)
                            .HasColumnName("fone3_sinde");
            });

            builder.Property(e => e.Grau)
                .HasMaxLength(45)
                .HasColumnName("grau")
                .HasConversion(new EnumToStringConverter<Grau>());

            builder.Property(e => e.Instagram)
                .HasMaxLength(45)
                .HasColumnName("instagram_sinde");

            builder.Property(e => e.Logradouro)
                .HasMaxLength(155)
                .HasColumnName("logradouro_sinde");

            builder.Property(e => e.Municipio)
                .HasMaxLength(45)
                .HasColumnName("municipio_sinde");

            builder.Property(e => e.Negociador)
                .HasMaxLength(45)
                .HasColumnName("negociador_sinde");

            builder.OwnsOne(p => p.Ramal, ramalBuilder =>
            {
                ramalBuilder.Property(c => c.Valor)
                            .HasMaxLength(45)
                            .HasColumnName("ramal_sinde");
            });

            builder.Property(e => e.RazaoSocial)
                .HasMaxLength(200)
                .HasColumnName("razaosocial_sinde");

            builder.Property(e => e.Sigla)
                .HasMaxLength(45)
                .HasColumnName("sigla_sinde");

            builder.Property(e => e.Site)
                .HasMaxLength(100)
                .HasColumnName("site_sinde");

            builder.Property(e => e.Situacao)
                .HasMaxLength(45)
                .HasColumnName("situacaocadastro_sinde");

            builder.Property(e => e.Status)
                .HasMaxLength(45)
                .HasColumnName("status")
                .HasConversion(new BooleanToStringAtivoInativoConverter());

            builder.Property(e => e.Twitter)
                .HasMaxLength(45)
                .HasColumnName("twitter_sinde");

            builder.Property(e => e.Uf)
                .HasMaxLength(2)
            .HasColumnName("uf_sinde");

            builder.Property(e => e.CentralSindicalId).HasColumnName("central_sindical_id_centralsindical");
            builder.Property(e => e.ConfederacaoId).HasColumnName("confederacao_id_associacao");
            builder.Property(e => e.FederacaoId).HasColumnName("federacao_id_associacao");

            builder.HasOne<CentralSindical>()
                .WithMany(d => d.SindEmps)
                .HasForeignKey(s => s.CentralSindicalId)
                .HasConstraintName("sind_emp_ibfk_5");
            
            builder.HasOne<Associacao>()
                .WithMany(p => p.SindEmpConfederacaoIdAssociacaoNavigations)
                .HasForeignKey(d => d.ConfederacaoId)
                .HasConstraintName("sind_emp_ibfk_4");

            builder.HasOne<Associacao>()
                .WithMany(p => p.SindEmpFederacaoIdAssociacaoNavigations)
                .HasForeignKey(d => d.FederacaoId)
                .HasConstraintName("sind_emp_ibfk_6");
        }
    }
}
