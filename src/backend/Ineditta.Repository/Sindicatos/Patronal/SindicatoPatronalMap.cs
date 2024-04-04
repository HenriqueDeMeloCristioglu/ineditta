﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CSharpFunctionalExtensions;

using Ineditta.Application.Sindicatos.Laborais.Entities;
using Ineditta.Application.Sindicatos.Patronais.Entities;
using Ineditta.Repository.Converters;
using Ineditta.Repository.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using static Dapper.SqlMapper;

namespace Ineditta.Repository.Sindicatos.Patronal
{
    internal sealed class SindicatoPatronalMap : IEntityTypeConfiguration<SindicatoPatronal>
    {
        public void Configure(EntityTypeBuilder<SindicatoPatronal> builder)
        {
            builder.ToTable("sind_patr");

            builder.HasKey(e => e.Id).HasName("PRIMARY");

            builder
                .ToTable("sind_patr")
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_general_ci");

            builder.HasIndex(e => e.ConfederacaoId, "confederacao_id_associacao");

            builder.HasIndex(e => e.FederacaoId, "federacao_id_associacao");


            builder.Property(e => e.Id).HasColumnName("id_sindp");

            builder.OwnsOne(p => p.Cnpj, cnpjBuilder =>
            {
                cnpjBuilder.Property(c => c.Value)
                            .HasMaxLength(20)
                            .HasColumnName("cnpj_sp");
            });

            builder.OwnsOne(p => p.CodigoSindical, codigoSindicalBuilder =>
            {
                codigoSindicalBuilder.Property(c => c.Valor)
                            .HasMaxLength(125)
                            .HasColumnName("codigo_sp");
            });

            builder.Property(e => e.Contribuicao)
                .HasMaxLength(45)
                .HasColumnName("contribuicao_sp");

            builder.Property(e => e.Denominacao)
                .HasMaxLength(450)
                .HasColumnName("denominacao_sp");

            builder.OwnsOne(p => p.Email1, emailBuilder =>
            {
                emailBuilder.Property(c => c.Valor)
                            .HasMaxLength(100)
                            .HasColumnName("email1_sp");
            });
            builder.OwnsOne(p => p.Email2, emailBuilder =>
            {
                emailBuilder.Property(c => c.Valor)
                            .HasMaxLength(100)
                            .HasColumnName("email2_sp");
            });
            builder.OwnsOne(p => p.Email3, emailBuilder =>
            {
                emailBuilder.Property(c => c.Valor)
                            .HasMaxLength(100)
                            .HasColumnName("email3_sp");
            });

            builder.Property(e => e.Enquadramento)
                .HasMaxLength(45)
                .HasColumnName("enquadramento_sp");

            builder.Property(e => e.Facebook)
                .HasMaxLength(45)
                .HasColumnName("facebook_sp");

            builder.Property(e => e.FederacaoId).HasColumnName("federacao_id_associacao");

            builder.Property(e => e.ConfederacaoId).HasColumnName("confederacao_id_associacao");

            builder.OwnsOne(p => p.Telefone1, telefoneBuilder =>
            {
                telefoneBuilder.Property(c => c.Valor)
                            .HasMaxLength(20)
                            .HasColumnName("fone1_sp");
            });
            builder.OwnsOne(p => p.Telefone2, telefoneBuilder =>
            {
                telefoneBuilder.Property(c => c.Valor)
                            .HasMaxLength(20)
                            .HasColumnName("fone2_sp");
            });
            builder.OwnsOne(p => p.Telefone3, telefoneBuilder =>
            {
                telefoneBuilder.Property(c => c.Valor)
                            .HasMaxLength(20)
                            .HasColumnName("fone3_sp");
            });

            builder.Property(e => e.Grau)
                .HasMaxLength(45)
                .HasColumnName("grau_sp")
                .HasConversion(new EnumToStringConverter<Grau>());

            builder.Property(e => e.Instagram)
                .HasMaxLength(45)
                .HasColumnName("instagram_sp");

            builder.Property(e => e.Logradouro)
                .HasMaxLength(155)
                .HasColumnName("logradouro_sp");

            builder.Property(e => e.Municipio)
                .HasMaxLength(45)
                .HasColumnName("municipio_sp");

            builder.Property(e => e.Negociador)
                .HasMaxLength(45)
                .HasColumnName("negociador_sp");

            builder.OwnsOne(p => p.Ramal, ramalBuilder =>
            {
                ramalBuilder.Property(c => c.Valor)
                            .HasMaxLength(45)
                            .HasColumnName("ramal_sp");
            });

            builder.Property(e => e.RazaoSocial)
                .HasMaxLength(200)
                .HasColumnName("razaosocial_sp");

            builder.Property(e => e.Sigla)
                .HasMaxLength(45)
                .HasColumnName("sigla_sp");

            builder.Property(e => e.Site)
                .HasMaxLength(100)
                .HasColumnName("site_sp");

            builder.Property(e => e.Situacao)
                .HasMaxLength(45)
                .HasColumnName("situacaocadastro_sp");

            builder.Property(e => e.Status)
                .HasMaxLength(20)
                .HasColumnName("status")
                .HasConversion(new BooleanToStringAtivoInativoConverter());

            builder.Property(e => e.Twitter)
                .HasMaxLength(45)
                .HasColumnName("twitter_sp");

            builder.Property(e => e.Uf)
                .HasMaxLength(2)
                .HasColumnName("uf_sp");

            builder.Property(e => e.FederacaoId).HasColumnName("federacao_id_associacao");

            builder.Property(e => e.ConfederacaoId).HasColumnName("confederacao_id_associacao");

            builder.HasOne<Associacao>()
                .WithMany(p => p.SindPatrConfederacaoIdAssociacaoNavigations)
                .HasForeignKey(p => p.ConfederacaoId)
                .HasConstraintName("sind_patr_ibfk_1");

            builder.HasOne<Associacao>()
                .WithMany(p => p.SindPatrFederacaoIdAssociacaoNavigations)
                .HasForeignKey(d => d.FederacaoId)
                .HasConstraintName("sind_patr_ibfk_2");
        }
    }
}