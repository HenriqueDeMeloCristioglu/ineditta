using Ineditta.Application.Documentos.Sindicais.Entities;
using Ineditta.Application.Documentos.SindicatosPatronais;
using Ineditta.Application.Sindicatos.Patronais.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ineditta.Repository.Documentos.SindicatosPatronais
{
    public sealed class DocumentoSindicatoPatronalMap : IEntityTypeConfiguration<DocumentoSindicatoPatronal>
    {
        public void Configure(EntityTypeBuilder<DocumentoSindicatoPatronal> builder)
        {
            builder.ToTable("documento_sindicato_patronal_tb");
            builder.HasKey(x => x.Id).HasName("PRIMARY");

            builder.Property(x => x.Id)
                .HasColumnName("id");

            builder.Property(x => x.DocumentoSindicalId)
                .HasColumnName("documento_id");

            builder.Property(x => x.SindicatoPatronalId)
                .HasColumnName("sindicato_patronal_id");

            builder.HasOne<DocumentoSindical>()
                .WithMany()
                .HasForeignKey(x => x.DocumentoSindicalId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("documento_sindicato_patronal_tb_ibfk_1");

            builder.HasOne<SindicatoPatronal>()
                .WithMany()
                .HasForeignKey(x => x.SindicatoPatronalId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("documento_sindicato_patronal_tb_ibfk_2");

            builder.HasIndex(x => new { x.DocumentoSindicalId, x.SindicatoPatronalId })
                .IsUnique()
                .HasDatabaseName("ix001_documento_sindicato_patronal_tb");
        }
    }
}
