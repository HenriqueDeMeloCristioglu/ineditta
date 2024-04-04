using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CSharpFunctionalExtensions;

using Ineditta.Application.TiposDocumentos.Entities;
using Ineditta.Repository.Converters;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ineditta.Repository.TiposDocumentos
{
    public sealed class TipoDocumentoMap : IEntityTypeConfiguration<TipoDocumento>
    {
        public void Configure(EntityTypeBuilder<TipoDocumento> builder)
        {
            builder.HasKey(e => e.Id).HasName("PRIMARY");

            builder
                .ToTable("tipo_doc");

            builder.Property(e => e.Id).HasColumnName("idtipo_doc");
            
            builder.Property<string>("Abrangencia")
                .HasMaxLength(2)
                .HasColumnName("abrangencia");

            builder.Property<string>("AtividadeEconomica")
                .HasMaxLength(2)
                .HasColumnName("atividade_economica");

            builder.Property<string>("DataBase")
                .HasMaxLength(2)
                .HasColumnName("data_base");

            builder.Property<string>("Descricao")
                .HasMaxLength(2)
                .HasColumnName("descricao_doc");

            builder.Property<string>("Estabelecimento")
                .HasMaxLength(2)
                .HasColumnName("estabelecimento");

            builder.Property<string>("FonteLegislacao")
                .HasMaxLength(2)
                .HasColumnName("fonte_leg");

            builder.Property(p => p.ModuloCadastro)
                .HasMaxLength(25)
                .HasColumnName("modulo_cad")
                .HasConversion(new EnumToStringConverter<TipoModuloCadastro>());

            builder.Property<string>(e => e.Nome)
                .HasMaxLength(45)
                .HasColumnName("nome_doc");

            builder.Property<string>("NumeroLegislacao")
                .HasMaxLength(2)
                .HasColumnName("numero_leg");

            builder.Property<string>("Origem")
                .HasMaxLength(2)
                .HasColumnName("origem");

            builder.Property<string>("Permissao")
                .HasMaxLength(3)
                .HasColumnName("permissao")
                .HasDefaultValueSql("'sim'");

            builder.Property<string>("PermitirCompartilhar")
                .HasMaxLength(2)
                .HasColumnName("permitir_compartilhar");

            builder.Property(e => e.Processado)
                .HasMaxLength(3)
                .HasColumnName("processado")
                .HasConversion(new BooleanToStringConverter());

            builder.Property<string>("Assunto")
                .HasMaxLength(2)
                .HasColumnName("refer_assunto");

            builder.Property(e => e.Sigla)
                .HasMaxLength(7)
                .HasColumnName("sigla_doc");

            builder.Property<string>("SindicatoLaboral")
                .HasMaxLength(2)
                .HasColumnName("sind_laboral");

            builder.Property<string>("SindicatoPatronal")
                .HasMaxLength(2)
                .HasColumnName("sind_patronal");

            builder.Property(e => e.Grupo)
                .HasMaxLength(45)
                .HasColumnName("tipo_doc");

            builder.Property<string>("TipoUnidadeCliente")
                .HasMaxLength(2)
                .HasColumnName("tipo_unid_cliente");

            builder.Property<string>("ValidadeFinal")
                .HasMaxLength(2)
                .HasColumnName("validade_final");

            builder.Property<string>("ValidadeInicial")
                .HasMaxLength(2)
                .HasColumnName("validade_inicial");

            builder.Property<string>("Versao")
                .HasMaxLength(2)
                .HasColumnName("versao");

            builder.Property<string>("Anuencia")
                .HasMaxLength(2)
                .HasColumnName("anuencia");
        }
    }
}
