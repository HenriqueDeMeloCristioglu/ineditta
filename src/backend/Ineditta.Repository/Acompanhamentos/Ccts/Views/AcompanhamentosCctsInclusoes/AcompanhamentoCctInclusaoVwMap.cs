using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ineditta.Repository.Acompanhamentos.Ccts.Views.AcompanhamentosCctsInclusoes
{
    internal sealed class AcompanhamentoCctInclusaoVwMap : IEntityTypeConfiguration<AcompanhamentoCctInclusaoVw>
    {
        public void Configure(EntityTypeBuilder<AcompanhamentoCctInclusaoVw> builder)
        {
            builder.ToView("acompanhamento_cct_inclusao_vw");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Id)
                .HasColumnName("id");

            builder.Property(a => a.DataInicial)
                .HasColumnName("data_inicial");

            builder.Property(a => a.DataFinal)
                .HasColumnName("data_final");

            builder.Property(a => a.DataAlteracao)
                .HasColumnName("data_alteracao");

            builder.Property(a => a.Status)
                .HasColumnName("status");

            builder.Property(a => a.NomeUsuario)
                .HasColumnName("nome_usuario");

            builder.Property(a => a.Fase)
                .HasColumnName("fase");

            builder.Property(a => a.NomeDocumento)
                .HasColumnName("nome_documento");

            builder.Property(a => a.ProximaLigacao)
                .HasColumnName("proxima_ligacao");

            builder.Property(a => a.DataBase)
                .HasColumnName("data_base");

            builder.Property(a => a.SiglaSinditoPatronal)
                .HasColumnName("sigla_sindicato_patronal");

            builder.Property(a => a.UfSinditoPatronal)
                .HasColumnName("uf_sindicato_patronal");

            builder.Property(a => a.SiglaSinditoLaboral)
                .HasColumnName("sigla_sindicato_empregado");

            builder.Property(a => a.UfSinditoLaboral)
                .HasColumnName("uf_sindicato_empregado");

            builder.Property(a => a.DescricaoSubClasse)
                .HasColumnName("descricao_sub_classe");

            builder.Property(a => a.Etiquetas)
                .HasColumnName("etiquetas");
        }
    }
}
