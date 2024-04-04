using Ineditta.Application.BasesTerritoriaisLaborais.Entities;
using Ineditta.Application.Cnaes.Entities;
using Ineditta.Application.Localizacoes.Entities;
using Ineditta.Application.Sindicatos.Laborais.Entities;
using Ineditta.Repository.Converters;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ineditta.Repository.BasesTerritoriaisLaborais
{
    internal sealed class BaseTerritorialLaboralMap : IEntityTypeConfiguration<BaseTerritorialLaboral>
    {
        public void Configure(EntityTypeBuilder<BaseTerritorialLaboral> builder)
        {
            builder.HasKey(e => e.Id).HasName("PRIMARY");

            builder
                .ToTable("base_territorialsindemp")
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_general_ci");

            builder.HasIndex(e => e.CnaeId, "classe_cnae_idclasse_cnae");
            builder.HasIndex(e => e.LocalizacaoId, "localizacao_id_localizacao1");
            builder.HasIndex(e => e.SindicatoId, "sind_empregados_id_sinde1");

            builder.Property(e => e.Id).HasColumnName("idbase_territorialSindEmp");
            builder.Property(e => e.CnaeId).HasColumnName("classe_cnae_idclasse_cnae");
            builder.Property(e => e.DataFinal).HasColumnName("data_final");
            builder.Property(e => e.DataInicial).HasColumnName("data_inicial");
            builder.Property(e => e.DataNegociacao)
                .HasMaxLength(4)
                .HasColumnName("dataneg")
                .HasConversion(new EnumToStringConverter<DataNegociacao>());

            builder.Property(e => e.LocalizacaoId).HasColumnName("localizacao_id_localizacao1");
            builder.Property(e => e.SindicatoId).HasColumnName("sind_empregados_id_sinde1");

            builder.HasOne<Cnae>()
                .WithMany()
                .HasForeignKey(e => e.CnaeId)
                .HasConstraintName("base_territorialsindemp_ibfk_3");

            builder.HasOne<Localizacao>()
                .WithMany()
                .HasForeignKey(e => e.LocalizacaoId)
                .HasConstraintName("base_territorialsindemp_ibfk_2");

            builder.HasOne<SindicatoLaboral>()
                .WithMany()
                .HasForeignKey(e => e.SindicatoId)
                .HasConstraintName("base_territorialsindemp_ibfk_1");
        }
    }
}
