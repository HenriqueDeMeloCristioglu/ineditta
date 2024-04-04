using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ineditta.Application.AIs.Clausulas.Entities;
using Ineditta.Application.Sinonimos.Entities;
using Ineditta.Application.AIs.DocumentosSindicais.Entities;
using Ineditta.Application.EstruturasClausulas.Gerais.Entities;

namespace Ineditta.Repository.IA.IAClausulas
{
    public sealed class IAClausulaMap : IEntityTypeConfiguration<IAClausula>
    {
        public void Configure(EntityTypeBuilder<IAClausula> builder)
        {
            builder.ToTable("ia_clausula_tb");
            builder.HasKey(x => x.Id).HasName("PRIMARY");

            builder.Property(x => x.Id)
                .HasColumnName("id");

            builder.Property(x => x.IADocumentoSindicalId)
                .HasColumnName("ia_documento_sindical_id");

            builder.Property(x => x.Nome)
                .HasColumnName("nome");

            builder.Property(x => x.Texto)
                .HasColumnName("texto");

            builder.Property(x => x.Grupo)
                .HasColumnName("grupo");

            builder.Property(x => x.SubGrupo)
                .HasColumnName("sub_grupo");

            builder.Property(x => x.EstruturaClausulaId)
                .HasColumnName("estrutura_clausula_id");

            builder.Property(x => x.DataProcessamento)
                .HasColumnName("data_processamento");

            builder.Property(x => x.Numero)
                .HasColumnName("numero");

            builder.Property(x => x.SinonimoId)
                .HasColumnName("sinonimo_id");

            builder.Property(x => x.Status)
                .HasColumnName("status");

            builder.HasOne<EstruturaClausula>()
                .WithMany()
                .HasForeignKey(x => x.EstruturaClausulaId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("ia_clausula_tb_ibfk_1")
                .IsRequired(false);

            builder.HasOne<Sinonimo>()
                .WithMany()
                .HasForeignKey(x => x.SinonimoId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("ia_clausula_tb_ibfk_2")
                .IsRequired(false);

            builder.HasOne<IADocumentoSindical>()
                .WithMany()
                .HasForeignKey(x => x.IADocumentoSindicalId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("ia_clausula_tb_ibfk_3")
                .IsRequired(true);
        }
    }
}
