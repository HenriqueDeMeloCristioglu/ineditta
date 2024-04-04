using Ineditta.Application.BasesTerritoriaisPatronais.Entities;
using Ineditta.Application.Documentos.Sindicais.Dtos;
using Ineditta.Application.Localizacoes.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ineditta.Repository.BasesTerritoriaisPatronais
{
    internal sealed class BaseTerritorialPatronalMap : IEntityTypeConfiguration<BaseTerritorialPatronal>
    {
        public void Configure(EntityTypeBuilder<BaseTerritorialPatronal> builder)
        {
            builder.ToTable("base_territorialsindpatro");

            builder.HasKey(e => e.Id).HasName("PRIMARY");

            builder.HasIndex(e => e.CnaeId, "classe_cnae_idclasse_cnae");

            builder.HasIndex(e => e.LocalizacaoId, "localizacao_id_localizacao1");

            builder.HasIndex(e => e.SindicatoId, "sind_patronal_id_sindp");

            builder.Property(e => e.Id).HasColumnName("idbase_territorialSindPatro");
            builder.Property(e => e.CnaeId).HasColumnName("classe_cnae_idclasse_cnae");
            builder.Property(e => e.DataFinal).HasColumnName("data_final");
            builder.Property(e => e.DataInicial).HasColumnName("data_inicial");
            builder.Property(e => e.LocalizacaoId).HasColumnName("localizacao_id_localizacao1");
            builder.Property(e => e.SindicatoId).HasColumnName("sind_patronal_id_sindp");

            builder.HasOne<Cnae>()
                .WithMany()
                .HasForeignKey(e => e.CnaeId)
                .HasConstraintName("base_territorialsindpatro_ibfk_3");

            builder.HasOne<Localizacao>()
                .WithMany()
                .HasForeignKey(e => e.LocalizacaoId)
                .HasConstraintName("base_territorialsindpatro_ibfk_2");

            builder.HasOne<SindicatoPatronal>()
                .WithMany()
                .HasForeignKey(e => e.SindicatoId)
                .HasConstraintName("base_territorialsindpatro_ibfk_1");  
        }
    }
}
