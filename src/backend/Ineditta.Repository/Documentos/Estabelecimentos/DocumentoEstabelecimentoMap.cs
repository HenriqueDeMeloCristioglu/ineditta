using Ineditta.Application.ClientesUnidades.Entities;
using Ineditta.Application.Documentos.Estabelecimentos.Entities;
using Ineditta.Application.Documentos.Sindicais.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ineditta.Repository.Documentos.Estabelecimentos
{
    public sealed class DocumentoEstabelecimentoMap : IEntityTypeConfiguration<DocumentoEstabelecimento>
    {
        public void Configure(EntityTypeBuilder<DocumentoEstabelecimento> builder)
        {
            builder.ToTable("documento_estabelecimento_tb");
            builder.HasKey(x => x.Id).HasName("PRIMARY");

            builder.Property(x => x.Id)
                .HasColumnName("id");

            builder.Property(x => x.DocumentoId)
                .HasColumnName("documento_id");

            builder.Property(x => x.EstabelecimentoId)
                .HasColumnName("estabelecimento_id");

            builder.HasOne<DocumentoSindical>()
                .WithMany()
                .HasForeignKey(x => x.DocumentoId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("documento_estabelecimento_tb_fk_1");

            builder.HasOne<ClienteUnidade>()
                .WithMany()
                .HasForeignKey(x => x.EstabelecimentoId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("documento_estabelecimento_tb_fk_2");
        }
    }
}
