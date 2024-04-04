using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CSharpFunctionalExtensions;

using Ineditta.Application.Localizacoes.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ineditta.Repository.Localizacoes
{
    public sealed class LocalizacaoMap : IEntityTypeConfiguration<Localizacao>
    {
        public void Configure(EntityTypeBuilder<Localizacao> builder)
        {
            builder.HasKey(e => e.Id).HasName("PRIMARY");

            builder
                .ToTable("localizacao")
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_general_ci");

            builder.Property(e => e.Id).HasColumnName("id_localizacao");

            builder.Property<string>("CodMunicipio")
                .HasMaxLength(20)
                .HasColumnName("cod_municipio");

            builder.Property<string>("CodPais")
                .HasMaxLength(20)
                .HasColumnName("cod_pais");

            builder.Property<string>("CodRegiao")
                .HasMaxLength(20)
                .HasColumnName("cod_regiao");

            builder.Property<string>("CodUf")
                .HasMaxLength(20)
                .HasColumnName("cod_uf");

            builder.OwnsOne(p => p.Pais, paisBuilder =>
            {
                paisBuilder.Property(e => e.Id)
                            .HasMaxLength(20)
                            .HasColumnName("pais");
            });

            builder.OwnsOne(p => p.Estado, estadoBuilder =>
            {
                estadoBuilder.Property(e => e.Id)
                            .HasMaxLength(20)
                            .HasColumnName("estado");
            });

            builder.OwnsOne(p => p.Uf, ufBuilder =>
            {
                ufBuilder.Property(u => u.Id)
                            .HasMaxLength(2)
                            .HasColumnName("uf");
            });

            builder.OwnsOne(p => p.Regiao, regiaoBuilder =>
            {
                regiaoBuilder.Property(r => r.Id)
                                .HasMaxLength(20)
                                .HasColumnName("regiao");
            });

            builder.Property(e => e.Municipio)
                .HasColumnType("text")
                .HasColumnName("municipio");
        }
    }
}
