using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ineditta.Repository.ClientesUnidades.Views
{
    internal sealed class EstabelecimentosVwMap : IEntityTypeConfiguration<EstabelecimentosVw>
    {
        public void Configure(EntityTypeBuilder<EstabelecimentosVw> builder)
        {
            builder.ToView("estabelecimentos_vw");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Nome).HasColumnName("nome");

            builder.Property(e => e.NomeGrupoEconomico).HasColumnName("nome_grupo_economico");

            builder.Property(e => e.Filial).HasColumnName("filial");

            builder.Property(e => e.Grupo).HasColumnName("grupo");

            builder.Property(e => e.Cnpj).HasColumnName("cnpj");

            builder.Property(e => e.ClienteGrupoIdGrupoEconomico).HasColumnName("cliente_grupo_id_grupo_economico");
        }
    }
}
