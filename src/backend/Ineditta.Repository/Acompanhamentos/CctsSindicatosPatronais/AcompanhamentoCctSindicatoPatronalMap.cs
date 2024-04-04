using Ineditta.Application.Acompanhamentos.Ccts.Entities;
using Ineditta.Application.Acompanhamentos.CctsSindicatosPatronais.Entities;
using Ineditta.Application.Sindicatos.Patronais.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ineditta.Repository.Acompanhamentos.CctsSindicatosPatronais
{
    internal sealed class AcompanhamentoCctSindicatoPatronalMap : IEntityTypeConfiguration<AcompanhamentoCctSinditoPatronal>
    {
        public void Configure(EntityTypeBuilder<AcompanhamentoCctSinditoPatronal> builder)
        {
            builder.ToTable("acompanhamento_cct_sindicato_patronal_tb");

            builder.HasKey(a => a.Id)
                .HasName("PRIMARY");

            builder.Property(a => a.Id)
                .HasColumnName("id");

            builder.HasIndex(a => a.AcompanhamentoCctId, "ix001_acompanhamento_cct_sindicato_patronal_tb");

            builder.HasIndex(a => new { a.AcompanhamentoCctId, a.SindicatoId }, "ix002_acompanhamento_cct_sindicato_patronal_tb")
                .IsUnique();

            builder.Property(a => a.AcompanhamentoCctId)
                .HasColumnName("acompanhamento_cct_id");

            builder.Property(a => a.SindicatoId)
                .HasColumnName("sindicato_id");

            builder.HasOne<AcompanhamentoCct>()
                .WithMany()
                .HasForeignKey(a => a.AcompanhamentoCctId)
                .HasConstraintName("fk_acompanhamento_cct_sindicato_patronal_x_acompanhamento_cct");

            builder.HasOne<SindicatoPatronal>()
                .WithMany()
                .HasForeignKey(a => a.SindicatoId)
                .HasConstraintName("fkacompanhamento_cct_sindicato_patronal_x_sind_patr");
        }
    }
}
