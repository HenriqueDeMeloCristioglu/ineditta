using Ineditta.Application.NotificacaoEventos.Entities;
using Ineditta.Repository.Converters;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ineditta.Repository.NotificacoesEventos
{
    internal sealed class NotificacaoMap : IEntityTypeConfiguration<Notificacao>
    {
        public void Configure(EntityTypeBuilder<Notificacao> builder)
        {
            builder.ToTable("calendario_sindical_notificacao_tb");

            builder.HasKey(p => p.Id).HasName("PRIMARY");

            builder.Property(p => p.EventoId)
                .IsRequired(true)
                .HasColumnName("evento_id");

            builder.Property(p => p.Notificados)
                .HasColumnType("json")
                .HasColumnName("notificados")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasConversion(new NotificadosEventoConverter());

            builder.Property(p => p.NotificadosEm)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("notificados_em");
        }
    }
}
