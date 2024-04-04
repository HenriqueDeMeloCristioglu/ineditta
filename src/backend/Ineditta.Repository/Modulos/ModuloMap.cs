using CSharpFunctionalExtensions;

using Ineditta.Application.Modulos.Entities;
using Ineditta.Repository.Converters;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ineditta.Repository.Modulos
{
    public sealed class ModuloMap : IEntityTypeConfiguration<Modulo>
    {
        public void Configure(EntityTypeBuilder<Modulo> builder)
        {
            builder.HasKey(e => e.Id).HasName("PRIMARY");

            builder
                .ToTable("modulos");

            builder.Property(e => e.Id).HasColumnName("id_modulos");

            builder.Property(e => e.PermiteAlterar)
                .HasConversion(new BooleanToStringConverter())
                .HasMaxLength(2)
                .HasColumnType("varchar(2)")
                .HasColumnName("alterar");

            builder.Property(e => e.PermiteAprovar)
                .HasConversion(new BooleanToStringConverter())
                .HasMaxLength(2)
                .HasColumnType("varchar(2)")
                .HasColumnName("aprovar");

            builder.Property(e => e.PermiteComentar)
                .HasConversion(new BooleanToStringConverter())
                .HasMaxLength(2)
                .HasColumnType("varchar(2)")
                .HasColumnName("comentar");

            builder.Property(e => e.PermiteConsultar)
                .HasConversion(new BooleanToStringConverter())
                .HasMaxLength(2)
                .HasColumnType("varchar(2)")
                .HasColumnName("consultar");

            builder.Property(e => e.PermiteCriar)
                .HasConversion(new BooleanToStringConverter())
                .HasMaxLength(2)
                .HasColumnType("varchar(2)")
                .HasColumnName("criar");

            builder.Property(e => e.PermiteExcluir)
                .HasConversion(new BooleanToStringConverter())
                .HasMaxLength(2)
                .HasColumnType("varchar(2)")
                .HasColumnName("excluir");

            builder.Property(e => e.Nome)
            .HasMaxLength(55)
                .HasColumnName("modulos");

            builder.Property(e => e.Tipo)
                .HasConversion(new EnumToStringConverter<TipoModulo>())
                .HasMaxLength(20)
                .HasColumnType("varchar(20)")
                .HasDefaultValueSql("'Comercial'")
                .HasColumnName("tipo");

            builder.Property(e => e.Uri)
                .HasMaxLength(45)
                .HasColumnName("uri");
        }
    }
}
