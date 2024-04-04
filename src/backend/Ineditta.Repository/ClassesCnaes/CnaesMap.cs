using Ineditta.Application.Cnaes.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ineditta.Repository.ClassesCnaes
{
    internal sealed class CnaesMap : IEntityTypeConfiguration<Cnae>
    {
        public void Configure(EntityTypeBuilder<Cnae> builder)
        {
            builder.ToTable("classe_cnae");

            builder.HasKey(e => e.Id).HasName("PRIMARY");

            builder.Property(e => e.Id).HasColumnName("id_cnae");

            builder.Property(e => e.Categoria)
                .HasColumnType("text")
                .HasColumnName("categoria");

            builder.Property(e => e.DescricaoDivisao)
                .HasColumnType("text")
                .HasColumnName("descricao_divisão");

            builder.Property(e => e.DescricaoSubClasse)
                .HasColumnType("text")
                .HasColumnName("descricao_subclasse");

            builder.Property(e => e.Divisao).HasColumnName("divisao_cnae");

            builder.Property(e => e.SubClasse).HasColumnName("subclasse_cnae");
        }
    }
}
