using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Ineditta.Application.ClientesMatriz.Entities;
using Ineditta.Application.TiposDocumentos.Entities;
using Ineditta.Application.TiposDocumentosClientesMatriz.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ineditta.Repository.TiposDocumentosClientesMatriz
{
    public sealed class TipoDocumentoClienteMatrizMap : IEntityTypeConfiguration<TipoDocumentoClienteMatriz>
    {
        public void Configure(EntityTypeBuilder<TipoDocumentoClienteMatriz> builder)
        {
            builder.HasKey(e => e.Id).HasName("PRIMARY");

            builder
                .ToTable("tipo_documento_cliente_matriz");

            builder.Property(e => e.Id)
                .HasColumnName("id");

            builder.Property(e => e.ClienteMatrizId)
                .HasColumnName("cliente_matriz_id");

            builder.Property(e => e.TipoDocumentoId)
                .HasColumnName("tipo_documento_id");

            builder.HasOne<ClienteMatriz>()
                .WithMany()
                .HasForeignKey(d => d.ClienteMatrizId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("tipo_documento_matriz_cliente_ibfk_1");

            builder.HasOne<TipoDocumento>()
                .WithMany()
                .HasForeignKey(d => d.TipoDocumentoId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("tipo_documento_cliente_matriz_ibfk_2");
        }
    }
}
