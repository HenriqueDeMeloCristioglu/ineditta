using Ineditta.Application.Documentos.Sindicais.Dtos;
using Ineditta.Repository.Converters;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ineditta.Repository.Documentos.Sindicais.Views.DocumentosSindicaisSisapsVw
{
    internal sealed class DocumentoSindicalSisapVwMap : IEntityTypeConfiguration<DocumentoSindicalSisapVw>
    {
        public void Configure(EntityTypeBuilder<DocumentoSindicalSisapVw> builder)
        {
            builder.ToView("documentos_sindicais_sisap_vw");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                .HasColumnName("Id");

            builder.Property(p => p.NomeDoc)
                .HasColumnName("NomeDocumento");

            builder.Property(p => p.Cnaes)
                .HasColumnType("json")
                .HasColumnName("CnaeDocs")
                .HasConversion(new GenericPrivateResolverCamelCaseConverter<IEnumerable<Cnae>?>());

            builder.Property(p => p.DataValidadeInicial)
                .HasColumnName("ValidadeInicial");

            builder.Property(p => p.DataValidadeFinal)
                .HasColumnName("ValidadeFinal");

            builder.Property(p => p.SindicatosLaborais)
                .HasColumnType("json")
                .HasColumnName("NomeSindicatoLaboral")
                .HasConversion(new GenericPrivateResolverCamelCaseConverter<IEnumerable<SindicatoLaboral>?>());

            builder.Property(p => p.SindicatosPatronais)
                .HasColumnType("json")
                .HasColumnName("NomeSindicatoPatronal")
                .HasConversion(new GenericPrivateResolverCamelCaseConverter<IEnumerable<SindicatoPatronal>?>());

            builder.Property(p => p.UsuarioAprovador)
                .HasColumnName("NomeUsuarioAprovador");

            builder.Property(p => p.DataAssinatura)
                .HasColumnName("DataAssinatura");

            builder.Property(p => p.CnaeSubclasseCodigos)
                .HasColumnType("json")
                .HasColumnName("CnaeSubclasseCodigos");

            builder.Property(p => p.DataSla)
                .HasColumnName("DataSla");
        }
    }
}
