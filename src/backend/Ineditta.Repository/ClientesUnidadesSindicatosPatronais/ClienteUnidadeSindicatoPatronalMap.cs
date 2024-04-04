using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Ineditta.Application.ClientesUnidades.Entities;
using Ineditta.Application.ClientesUnidadesSindicatosPatronais.Entities;
using Ineditta.Application.Sindicatos.Patronais.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ineditta.Repository.ClientesUnidadesSindicatosPatronais
{
    public sealed class ClienteUnidadeSindicatoPatronalMap : IEntityTypeConfiguration<ClienteUnidadeSindicatoPatronal>
    {
        public void Configure(EntityTypeBuilder<ClienteUnidadeSindicatoPatronal> builder)
        {
            builder.ToTable("cliente_unidade_sindicato_patronal_tb");
            builder.HasKey(x => x.Id).HasName("PRIMARY");

            builder.Property(x => x.Id)
                .HasColumnName("id");

            builder.Property(x => x.SindicatoPatronalId)
                .HasColumnName("sindicato_patronal_id");

            builder.Property(x => x.ClienteUnidadeId)
                .HasColumnName("cliente_unidade_id");

            builder.HasOne<ClienteUnidade>()
                .WithMany()
                .HasForeignKey(x => x.ClienteUnidadeId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("cliente_unidade_sinedicato_patronal_ibfk_1");

            builder.HasOne<SindicatoPatronal>()
                .WithMany()
                .HasForeignKey(x => x.SindicatoPatronalId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("cliente_unidade_sindicato_patronal_ibfk_2");
        }
    }
}
