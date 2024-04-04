using Ineditta.Application.Documentos.Sindicais.Entities;
using Ineditta.Application.Documentos.SindicatosLaborais;
using Ineditta.Application.Sindicatos.Laborais.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ineditta.Repository.Documentos.SindicatosLaborais
{
    public sealed class DocumentoSindicatoLaboralMap : IEntityTypeConfiguration<DocumentoSindicatoLaboral>
    {
        public void Configure(EntityTypeBuilder<DocumentoSindicatoLaboral> builder)
        {
            builder.ToTable("documento_sindicato_laboral_tb");
            builder.HasKey(x => x.Id).HasName("PRIMARY");

            builder.Property(x => x.Id)
                .HasColumnName("id");

            builder.Property(x => x.DocumentoSindicalId)
                .HasColumnName("documento_id");

            builder.Property(x => x.SindicatoLaboralId)
                .HasColumnName("sindicato_laboral_id");

            builder.HasOne<DocumentoSindical>()
                .WithMany()
                .HasForeignKey(x => x.DocumentoSindicalId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("documento_sindicato_laboral_tb_ibfk_1");

            builder.HasOne<SindicatoLaboral>()
                .WithMany()
                .HasForeignKey(x => x.SindicatoLaboralId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("documento_sindicato_laboral_tb_ibfk_2");

            builder.HasIndex(x => new { x.DocumentoSindicalId, x.SindicatoLaboralId })
                .IsUnique()
                .HasDatabaseName("ix001_documento_sindicato_laboral_tb");
        }
    }
}
