using Ineditta.Application.Emails.StoragesManagers.Entities;
using Ineditta.Repository.Converters;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ineditta.Repository.Emails.StoragesManagers
{
    public sealed class EmailStorageManagerMap : IEntityTypeConfiguration<EmailStorageManager>
    {
        public void Configure(EntityTypeBuilder<EmailStorageManager> builder)
        {
            builder.ToTable("email_storage_manager_tb");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                .HasColumnName("id");

            builder.OwnsOne(e => e.To, em =>
            {
                em.Property(e => e.Valor)
                    .HasMaxLength(100)
                    .HasColumnName("to");
            });

            builder.OwnsOne(e => e.From, em =>
            {
                em.Property(e => e.Valor)
                    .HasMaxLength(100)
                    .HasColumnName("from");
            });

            builder.Property(e => e.MessageId)
                .HasColumnName("message_id");

            builder.Property(e => e.Assunto)
                .HasColumnName("assunto");

            builder.Property(e => e.Enviado)
                .HasConversion(new BooleanToIntConverter())
                .HasColumnName("enviado");

            builder.Property(e => e.DataInclusao)
                .HasColumnName("data_inclusao");

            builder.Property(e => e.RequestData)
                .HasDefaultValue("")
                .HasColumnName("request_data");
        }
    }
}
