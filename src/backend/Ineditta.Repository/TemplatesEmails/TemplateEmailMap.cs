using Ineditta.Application.TemplatesEmails.Entities;
using Ineditta.Application.Usuarios.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Ineditta.Repository.TemplatesEmails
{
    public sealed class TemplateEmailMap : IEntityTypeConfiguration<TemplateEmail>
    {
        public void Configure(EntityTypeBuilder<TemplateEmail> builder)
        {
            builder.ToTable("template_email_tb");
            builder.HasKey(p => p.Id).HasName("PRIMARY");

            builder.Property(p => p.Id)
                .HasColumnName("id");

            builder.Property(p => p.TipoTemplate)
                .HasColumnName("tipo_template")
                .HasConversion(new EnumToNumberConverter<TipoTemplateEmail, int>());

            builder.Property(p => p.Nivel)
                .HasColumnName("nivel")
                .HasConversion(new EnumToNumberConverter<Nivel, int>());

            builder.Property(p => p.ReferenciaId)
                .HasColumnName("referencia_id");

            builder.Property(p => p.Html)
                .HasColumnName("html");
        }
    }
}
