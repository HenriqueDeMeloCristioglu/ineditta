using Ineditta.Repository.Converters;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ineditta.Repository.ClientesUnidades.Views
{
    public class InformacoesEstabelecimentosVwMap : IEntityTypeConfiguration<InformacoesEstabelecimentosVw>
    {
        public void Configure(EntityTypeBuilder<InformacoesEstabelecimentosVw> builder)
        {
            builder.ToView("informacoes_estabelecimentos_vw");
            builder.HasNoKey();

            builder.Property(p => p.UnidadeId)
                .HasColumnName("UnidadeId");

            builder.Property(p => p.SindicatosPatronais)
                .HasColumnType("json")
                .HasColumnName("SindicatosPatronais")
                .HasConversion(new GenericPrivateResolverCamelCaseConverter<IEnumerable<SindicatoPatronalInformacoesEstabelecimentoDto>>());

            builder.Property(p => p.SindicatosLaborais)
                .HasColumnType("json")
                .HasColumnName("SindicatosLaborais")
                .HasConversion(new GenericPrivateResolverCamelCaseConverter<IEnumerable<SindicatoLaboralInformacoesEstabelecimentoDto>>());

            builder.Property(p => p.SindicatosPatronaisSiglas)
                .HasColumnName("SindicatosPatronaisSiglas");

            builder.Property(p => p.SindicatosLaboraisSiglas)
                .HasColumnName("SindicatosLaboraisSiglas");

            builder.Property(p => p.CodigoSindicatoCliente)
                .HasColumnName("CodigoSindicatoLaboral");

            builder.Property(p => p.CodigoEstabelecimento)
                .HasColumnName("CodigoUnidade");

            builder.Property(p => p.NomeEstabelecimento)
                .HasColumnName("NomeUnidade");

            builder.Property(p => p.CnpjEstabalecimento)
                .HasColumnName("CnpjUnidade");

            builder.Property(p => p.DatasBases)
                .HasColumnName("datasBases");

            builder.Property(p => p.EmailUsuario)
                .HasColumnName("EmailUsuario");
        }
    }
}
