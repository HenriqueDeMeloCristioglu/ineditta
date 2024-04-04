using Ineditta.Repository.Converters;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Ineditta.Repository.Clausulas.Geral.Models;

namespace Ineditta.Repository.Clausulas.Views.ComparativoMapaSindical
{
    public class ComparativoMapaSindicalPrincipalVwMap : IEntityTypeConfiguration<ComparativoMapaSindicalPrincipalVw>
    {
        public void Configure(EntityTypeBuilder<ComparativoMapaSindicalPrincipalVw> builder)
        {
            builder.ToView("comparativo_mapa_sindical_principal_vw");

            builder.HasNoKey();

            builder.Property(e => e.DocumentoId).HasColumnName("documento_id");
            builder.Property(e => e.SindicatosPatronais).HasColumnName("sindicatos_patronais")
                   .HasColumnType("json")
                   .HasConversion(new GenericConverter<SindPatronal[]>());

            builder.Property(e => e.SindicatosLaborais).HasColumnName("sindicatos_laborais")
                   .HasColumnType("json")
                   .HasConversion(new GenericConverter<SindLaboral[]>());

            builder.Property(e => e.DataAprovacao).HasColumnName("data_aprovacao");
            builder.Property(e => e.Database).HasColumnName("data_base");
            builder.Property(e => e.Cnaes)
                   .HasColumnName("cnaes")
                   .HasColumnType("json")
                   .HasConversion(new GenericConverter<CnaeDoc[]>());

            builder.Property(e => e.Abrangencia)
                   .HasColumnName("abrangencia")
                   .HasColumnType("json")
                   .HasConversion(new GenericConverter<Abrangencia[]>());

            builder.Property(e => e.Estabelecimentos)
                   .HasColumnName("estabelecimentos")
                   .HasColumnType("json")
                   .HasConversion(new GenericConverter<ClienteEstabelecimento[]>());

            builder.Property(e => e.ValidadeInicial).HasColumnName("validade_inicial");
            builder.Property(e => e.ValidadeFinal).HasColumnName("validade_final");
            builder.Property(e => e.Uf).HasColumnName("uf");
            builder.Property(e => e.IndiceProjetado).HasColumnName("indice_projetado");
            builder.Property(e => e.InpcId).HasColumnName("inpc_id");
            builder.Property(e => e.DocumentoNome).HasColumnName("documento_nome");
            builder.Property(e => e.DataUpload).HasColumnName("data_upload");
            builder.Property(e => e.Descricao).HasColumnName("descricao_documento");
            builder.Property(e => e.SiglasSindicatosPatronais).HasColumnName("siglas_sindicatos_patronais");
            builder.Property(e => e.SiglasSindicatosLaborais).HasColumnName("siglas_sindicatos_laborais");
            builder.Property(e => e.CnaesSubclasses).HasColumnName("cnaes_subclasses");
        }
    }
}
