using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Ineditta.Application.CalendarioSindicais.Usuarios.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ineditta.Repository.CalendariosSindicais
{
    public class CalendarioSindicalUsuarioMap : IEntityTypeConfiguration<CalendarioSindicalUsuario>
    {
        public void Configure(EntityTypeBuilder<CalendarioSindicalUsuario> builder)
        {
            builder.ToTable("calendario_sindical_usuario_tb");

            builder.HasKey(e => e.Id).HasName("PRIMARY");

            builder.Property(p => p.Id)
                .HasColumnName("id");

            builder.Property(p => p.Titulo)
                .HasColumnName("titulo");

            builder.Property(p => p.Comentarios)
                .HasColumnName("comentarios");

            builder.Property(p => p.Local)
                .HasColumnName("local");

            builder.Property(p => p.DataHora)
                .HasColumnName("data_hora");

            builder.Property(p => p.Recorrencia)
                .HasColumnName("recorrencia")
                .HasDefaultValue(TipoRecorrencia.NaoRepetir);

            builder.Property(e => e.NotificarAntes)
                .HasColumnName("notificar_antes")
                .HasDefaultValueSql("'00:00:00'");

            builder.Property(p => p.Visivel)
                .HasColumnName("visivel");

            builder.Property<DateTime>("DataInclusao")
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("data_inclusao");

            builder.Property(p => p.ValidadeRecorrencia)
                .HasColumnName("validade_recorrencia");

            builder.Property(p => p.IdUsuario)
                .HasColumnName("id_usuario");
        }
    }
}
