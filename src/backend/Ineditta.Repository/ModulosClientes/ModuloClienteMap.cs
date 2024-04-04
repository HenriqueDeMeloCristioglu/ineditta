using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CSharpFunctionalExtensions;

using Ineditta.Application.ClientesMatriz.Entities;
using Ineditta.Application.Modulos.Entities;
using Ineditta.Application.ModulosClientes.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ineditta.Repository.ModulosClientes
{
    public sealed class ModuloClienteMap : IEntityTypeConfiguration<ModuloCliente>
    {
        public void Configure(EntityTypeBuilder<ModuloCliente> builder)
        {
            builder.HasKey(e => e.Id).HasName("PRIMARY");

            builder.ToTable("modulos_cliente");

            builder.HasIndex(e => e.ClienteMatrizId, "cliente_matriz_id_empresa");

            builder.HasIndex(e => e.ModuloId, "modulos_id_modulos");

            builder.Property(e => e.Id).HasColumnName("idmodulo_cliente");
            builder.Property(e => e.ClienteMatrizId).HasColumnName("cliente_matriz_id_empresa");
            builder.Property(e => e.DataFim).HasColumnName("data_fim");
            builder.Property(e => e.DataInicio).HasColumnName("data_inicio");
            builder.Property(e => e.ModuloId).HasColumnName("modulos_id_modulos");

            builder.HasOne<ClienteMatriz>()
                .WithMany()
                .HasForeignKey(d => d.ClienteMatrizId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("modulos_cliente_ibfk_2");

            builder.HasOne<Modulo>()
                .WithMany()
                .HasForeignKey(d => d.ModuloId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("modulos_cliente_ibfk_1");
        }
    }
}
