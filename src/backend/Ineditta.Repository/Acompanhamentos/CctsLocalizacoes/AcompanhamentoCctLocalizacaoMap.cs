using Ineditta.Application.Acompanhamentos.Ccts.Entities;
using Ineditta.Application.Acompanhamentos.CctsLocalizacoes.Entities;
using Ineditta.Application.Localizacoes.Entities;
using Ineditta.Repository.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Localizacao = Ineditta.Application.Localizacoes.Entities.Localizacao;

namespace Ineditta.Repository.Acompanhamentos.CctsLocalizacoes
{
    internal sealed class AcompanhamentoCctLocalizacaoMap : IEntityTypeConfiguration<AcompanhamentoCctLocalizacao>
    {
        public void Configure(EntityTypeBuilder<AcompanhamentoCctLocalizacao> builder)
        {
            builder.ToTable("acompanhamento_cct_localizacao_tb");

            builder.HasKey(a => a.Id)
                .HasName("PRIMARY");

            builder.Property(a => a.Id)
                .HasColumnName("id");

            builder.HasIndex(a => a.AcompanhamentoCctId, "ix001_acompanhamento_cct_localizacao_tb");

            builder.HasIndex(a => new { a.AcompanhamentoCctId, a.LocalizacaoId }, "ix002_acompanhamento_cct_localizacao_tb");

            builder.Property(a => a.AcompanhamentoCctId)
                .HasColumnName("acompanhamento_cct_id");

            builder.Property(a => a.LocalizacaoId)
                .HasColumnName("localizacao_id");

            builder.HasOne<AcompanhamentoCct>()
                .WithMany()
                .HasForeignKey(a => a.AcompanhamentoCctId)
                .HasConstraintName("fk_acompanhamento_cct_localizacao_x_acompanhamento_cct");

            builder.HasOne<Localizacao>()
                .WithMany()
                .HasForeignKey(a => a.LocalizacaoId)
                .HasConstraintName("fk_acompanhamento_cct_localizacao_x_localizacao");
        }
    }
}
