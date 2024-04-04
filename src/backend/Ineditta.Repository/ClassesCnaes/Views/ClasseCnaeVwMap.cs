using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ineditta.Repository.ClassesCnaes.Views
{
    internal sealed class ClasseCnaeVwMap : IEntityTypeConfiguration<ClasseCnaeVw>
    {
        public void Configure(EntityTypeBuilder<ClasseCnaeVw> builder)
        {
            builder.ToView("classe_cnae_vw");

            builder.HasNoKey();

            builder.Property(e => e.Id).HasColumnName("id");
            builder.Property(e => e.Divisao).HasColumnName("divisao");
            builder.Property(e => e.Subclasse).HasColumnName("subclasse");
            builder.Property(e => e.DescricaoSubclasse).HasColumnName("descricao_subclasse");
            builder.Property(e => e.Categoria).HasColumnName("categoria");
            builder.Property(e => e.SiglasSindicatosPatronais).HasColumnName("siglas_sindicatos_patronais");
            builder.Property(e => e.SiglasSindicatosLaborais).HasColumnName("siglas_sindicatos_laborais");
        }
    }
}
