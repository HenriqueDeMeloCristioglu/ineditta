using Ineditta.Application.Acompanhamentos.CctsStatusOpcoes.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ineditta.Repository.Acompanhamentos.CctsStatus
{
    internal sealed class AcompanhamentoCctStatusMap : IEntityTypeConfiguration<AcompanhamentoCctStatus>
    {
        public void Configure(EntityTypeBuilder<AcompanhamentoCctStatus> builder)
        {
            builder.ToTable("acompanhamento_cct_status_opcao_tb");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Id)
                .HasColumnName("id");

            builder.Property(a => a.Descricao)
                .HasColumnName("descricao")
                .HasMaxLength(250);
        }
    }
}
