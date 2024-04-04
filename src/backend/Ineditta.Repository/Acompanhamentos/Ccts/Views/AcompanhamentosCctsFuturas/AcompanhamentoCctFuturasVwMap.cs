using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ineditta.Repository.Acompanhamentos.Ccts.Views.AcompanhamentosCctsFuturas
{
    public class AcompanhamentoCctFuturasVwMap : IEntityTypeConfiguration<AcompanhamentoCctFuturasVw>
    {
        public void Configure(EntityTypeBuilder<AcompanhamentoCctFuturasVw> builder)
        {
            builder.ToView("acompanhamento_cct_futuras_vw");

            builder.HasNoKey();

            builder.Property(e => e.SiglaSinde).HasColumnName("sigla_sinde");

            builder.Property(e => e.SiglaSp).HasColumnName("sigla_sp");

            builder.Property(e => e.SindEmpregadosIdSinde).HasColumnName("sind_empregados_id_sinde1");

            builder.Property(e => e.SindPatronalIdSindp).HasColumnName("sind_patronal_id_sindp");

            builder.Property(e => e.Dataneg).HasColumnName("dataneg");

            builder.Property(e => e.ClasseCnaeIdClasseCnae).HasColumnName("classe_cnae_idclasse_cnae");

            builder.Property(e => e.DescricaoSubClasse).HasColumnName("descricao_subclasse");

            builder.Property(e => e.Responsavel).HasColumnName("responsavel");

            builder.Property(e => e.ResponsavelId).HasColumnName("responsavel_id");

            builder.Property(e => e.DataIni).HasColumnName("data_ini");
        }
    }
}
