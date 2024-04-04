using Ineditta.Application.Documentos.Localizacoes.Entities;
using Ineditta.Application.Documentos.Sindicais.Entities;
using Ineditta.Application.Localizacoes.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ineditta.Repository.Documentos.Localizacoes
{
    public sealed class DocumentoLocalizacoesMap : IEntityTypeConfiguration<DocumentoLocalizacao>
    {
        public void Configure(EntityTypeBuilder<DocumentoLocalizacao> builder)
        {
            builder.ToTable("documento_localizacao_tb");
            builder.HasKey(x => x.Id).HasName("PRIMARY");

            builder.Property(x => x.Id)
                .HasColumnName("id");

            builder.Property(x => x.DocumentoId)
                .HasColumnName("documento_id");

            builder.Property(x => x.LocalizacaoId)
                .HasColumnName("localizacao_id");

            builder.HasOne<DocumentoSindical>()
                .WithMany()
                .HasForeignKey(x => x.DocumentoId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("documento_localizacao_tb_fk_1");

            builder.HasOne<Localizacao>()
                .WithMany()
                .HasForeignKey(x => x.LocalizacaoId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("documento_localizacao_tb_fk_2");
        }
    }
}
