using Ineditta.Application.Acompanhamentos.Ccts.Entities;
using Ineditta.Application.Acompanhamentos.CctsSindicatosLaborais.Entities;
using Ineditta.Application.Sindicatos.Laborais.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ineditta.Repository.Acompanhamentos.CctsSindicatosLaborais
{
    internal sealed class AcompanhamentoCctSindicatoLaboralMap : IEntityTypeConfiguration<AcompanhamentoCctSinditoLaboral>
    {
        public void Configure(EntityTypeBuilder<AcompanhamentoCctSinditoLaboral> builder)
        {
            builder.ToTable("acompanhamento_cct_sindicato_laboral_tb");

            builder.HasKey(a => a.Id)
                .HasName("PRIMARY");

            builder.Property(a => a.Id)
                .HasColumnName("id");

            builder.HasIndex(a => a.AcompanhamentoCctId, "ix001_acompanhamento_cct_sindicato_laboral_tb");

            builder.HasIndex(a => new { a.AcompanhamentoCctId, a.SindicatoId }, "ix002_acompanhamento_cct_sindicato_laboral_tb")
                .IsUnique();

            builder.Property(a => a.AcompanhamentoCctId)
                .HasColumnName("acompanhamento_cct_id");

            builder.Property(a => a.SindicatoId)
                .HasColumnName("sindicato_id");

            builder.HasOne<AcompanhamentoCct>()
                .WithMany()
                .HasForeignKey(a => a.AcompanhamentoCctId)
                .HasConstraintName("fk_acompanhamento_cct_sindicato_laboral_x_acompanhamento_cct");

            builder.HasOne<SindicatoLaboral>()
                .WithMany()
                .HasForeignKey(a => a.SindicatoId)
                .HasConstraintName("fk_acompanhamento_cct_sindicato_laboral_x_sind_emp");
        }
    }
}
