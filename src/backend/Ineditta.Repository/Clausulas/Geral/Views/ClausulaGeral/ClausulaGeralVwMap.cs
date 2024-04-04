using Ineditta.Repository.Clausulas.Geral.Views.ClausulaGeral;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ineditta.Repository.ClausulasGerais.Views.ClausulaGeral
{
    public class ClausulaGeralVwMap : IEntityTypeConfiguration<ClausulaGeralVw>
    {
        public void Configure(EntityTypeBuilder<ClausulaGeralVw> builder)
        {
            builder.ToView("clausula_geral_vw");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id).HasColumnName("documento_id");

            builder.Property(e => e.Nome).HasColumnName("nome_doc");

            builder.Property(e => e.Processados).HasColumnName("quantidade_processados");

            builder.Property(e => e.NaoProcessados).HasColumnName("quantidade_nao_processados");

            builder.Property(e => e.Aprovados).HasColumnName("quantidade_aprovados");

            builder.Property(e => e.NaoAprovados).HasColumnName("quantidade_nao_aprovados");

            builder.Property(e => e.DataScrap).HasColumnName("data_scrap");

            builder.Property(e => e.DataSla).HasColumnName("data_sla");

            builder.Property(e => e.DataAprovacao).HasColumnName("data_aprovacao");
        }
    }
}
