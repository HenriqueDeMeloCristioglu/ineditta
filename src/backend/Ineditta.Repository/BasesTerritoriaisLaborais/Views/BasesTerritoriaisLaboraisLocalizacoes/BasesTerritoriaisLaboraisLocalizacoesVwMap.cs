using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ineditta.Repository.BasesTerritoriaisLaborais.Views.BasesTerritoriaisLaboraisLocalizacoes
{
    internal sealed class BasesTerritoriaisLaboraisLocalizacoesVwMap : IEntityTypeConfiguration<BasesTerritoriaisLaboraisLocalizacoesVw>
    {
        public void Configure(EntityTypeBuilder<BasesTerritoriaisLaboraisLocalizacoesVw> builder)
        {
            builder.ToView("base_territorial_sindicato_laboral_localizacao");

            builder.HasNoKey();

            builder.Property(e => e.Id).HasColumnName("id");

            builder.Property(e => e.Sigla).HasColumnName("sigla");

            builder.Property(e => e.Uf).HasColumnName("uf");

            builder.Property(e => e.Municipio).HasColumnName("municipio");

            builder.Property(e => e.Pais).HasColumnName("pais");

            builder.Property(e => e.SindicatoLaboralId).HasColumnName("sindicato_laboral_id");
        }
    }
}
