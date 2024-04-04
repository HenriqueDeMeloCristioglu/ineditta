using Ineditta.Application.CctsFases.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ineditta.Repository.Acompanhamentos.CctsFases
{
    internal sealed class FaseCctMap : IEntityTypeConfiguration<FasesCct>
    {
        public void Configure(EntityTypeBuilder<FasesCct> builder)
        {
            builder.ToTable("fase_cct");

            builder.HasKey(f => f.Id);

            builder.Property(f => f.Id)
                .HasColumnName("id_fase");

            builder.Property(f => f.Fase)
                .HasColumnName("fase_negociacao");

            builder.Property(f => f.Prioridade)
                .HasColumnName("prioridade");

            builder.Property(f => f.Periodicidade)
                .HasColumnName("periodicidade");

            builder.Property(f => f.Tipo)
                .HasColumnName("tipo_fase");
        }
    }
}
