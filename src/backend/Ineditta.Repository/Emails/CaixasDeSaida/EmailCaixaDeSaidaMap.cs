using Ineditta.Application.Emails.CaixasDeSaida.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ineditta.Repository.Emails.CaixasDeSaida
{
    internal sealed class EmailCaixaDeSaidaMap : IEntityTypeConfiguration<EmailCaixaDeSaida>
    {
        public void Configure(EntityTypeBuilder<EmailCaixaDeSaida> builder)
        {
            builder.ToTable("email_caixa_de_saida_tb");

            builder.HasIndex(e => e.Id);

            builder.Property(e => e.Id)
                .HasColumnName("id");

            builder.OwnsOne(e => e.Email, em =>
            {
                em.Property(e => e.Valor)
                    .HasMaxLength(100)
                    .HasColumnName("email");
            });

            builder.Property(e => e.Assunto)
                .HasColumnName("assunto");

            builder.Property(e => e.Template)
                .HasColumnName("template");

            builder.Property(e => e.MessageId)
                .HasColumnName("message_id");

            builder.Property(e => e.DataInclusao)
                .HasColumnName("data_inclusao");
        }
    }
}
