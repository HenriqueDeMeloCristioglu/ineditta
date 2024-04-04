using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Ineditta.Repository.Models;
using Ineditta.Application.Sinonimos.Entities;
using Ineditta.Application.EstruturasClausulas.Gerais.Entities;

namespace Ineditta.Repository.Sinonimos
{
    public sealed class SinonimoMap : IEntityTypeConfiguration<Sinonimo>
    {
        public void Configure(EntityTypeBuilder<Sinonimo> builder)
        {
            builder.ToTable("sinonimos");

            builder.HasKey(e => e.Id).HasName("PRIMARY");

            builder.Property(e => e.Id)
                .HasColumnName("id_sinonimo");

            builder.Property(e => e.Nome)
                .HasColumnType("text")
                .HasColumnName("nome_sinonimo");

            builder.Property(e => e.EstruturaClausulaId)
                .HasColumnName("estrutura_clausula_id_estruturaclausula");

            builder.Property(e => e.AssuntoId)
                .HasColumnName("assunto_idassunto");

            builder.HasOne<EstruturaClausula>()
                .WithMany()
                .HasForeignKey(e => e.EstruturaClausulaId)
                .HasConstraintName("fk_sinonimo_x_estrutura_clausula");

            builder.HasOne<Assunto>()
                .WithMany()
                .HasForeignKey(e => e.AssuntoId)
                .HasConstraintName("fk_sinonimo_x_assunto");

            builder.HasIndex(e => e.EstruturaClausulaId, "estrutura_clausula_id_estruturaclausula");
        }
    }
}
