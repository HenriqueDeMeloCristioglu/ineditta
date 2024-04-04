using CSharpFunctionalExtensions;

using Ineditta.Application.GruposClausulas.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ineditta.Repository.GruposClausulas
{
    internal sealed class GrupoClausulaMap : IEntityTypeConfiguration<GrupoClausula>
    {
        public void Configure(EntityTypeBuilder<GrupoClausula> builder)
        {
            builder.ToTable("grupo_clausula");

            builder.HasKey(e => e.Id).HasName("PRIMARY");

            builder.Property(e => e.Id)
                .HasColumnName("idgrupo_clausula");

            builder.Property(e => e.Cor)
                .HasMaxLength(15)
                .HasColumnName("cor");

            builder.Property(e => e.Nome)
                .HasMaxLength(80)
                .HasColumnName("nome_grupo");
        }
    }
}
