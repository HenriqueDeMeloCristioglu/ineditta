using Ineditta.Application.EstruturasClausulas.Gerais.Entities;
using Ineditta.Application.GruposClausulas.Entities;
using Ineditta.Repository.Converters;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ineditta.Repository.EstruturasClausulas.Gerais
{
    internal sealed class EstruturaClausulaMap : IEntityTypeConfiguration<EstruturaClausula>
    {
        public void Configure(EntityTypeBuilder<EstruturaClausula> builder)
        {
            builder.HasKey(e => e.Id).HasName("PRIMARY");

            builder.ToTable("estrutura_clausula");

            builder.Property(e => e.Id)
                .HasColumnName("id_estruturaclausula");

            builder.Property(e => e.Calendario)
                .HasMaxLength(1)
                .HasConversion(new BooleanToStringConverter())
                .HasColumnName("calendario");

            builder.Property(e => e.Classe)
                .HasMaxLength(1)
                .HasConversion(new BooleanToStringConverter())
                .HasColumnName("classe_clausula");

            builder.Property(e => e.GrupoClausulaId)
                .HasColumnName("grupo_clausula_idgrupo_clausula");

            builder.Property(e => e.Nome)
                .HasMaxLength(80)
                .HasColumnName("nome_clausula");

            builder.Property(e => e.Tipo)
                .HasMaxLength(2)
                .HasConversion(new EnumToStringConverter<Tipo>())
                .HasColumnName("tipo_clausula");

            builder.Property(e => e.Tarefa)
                .HasMaxLength(2)
                .HasConversion(new BooleanToStringConverter())
                .HasColumnName("tarefa");

            builder.Property(e => e.Resumivel)
                .HasDefaultValue(0)
                .HasConversion(new BooleanToIntConverter())
                .HasColumnName("resumivel");

            builder.Property(e => e.InstrucaoIa)
                .HasColumnType("longtext")
                .HasColumnName("instrucao_ia");

            builder.Property(e => e.MaximoPalavrasIA)
                .HasColumnName("maximo_palavras_ia");

            builder.HasOne<GrupoClausula>()
                .WithMany()
                .HasForeignKey(e => e.GrupoClausulaId)
                .HasConstraintName("fk_estrutura_clausula_x_grupo_clausula");

            builder.HasMany(p => p.GruposEconomicos)
                   .WithOne()
                   .HasForeignKey("EstruturaClausulaId")
                   .HasConstraintName("fk_estrutura_clausula_x_estrutura_clausula_grupo_economico")
                   .OnDelete(DeleteBehavior.Cascade)
                   .Metadata.PrincipalToDependent!.SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
