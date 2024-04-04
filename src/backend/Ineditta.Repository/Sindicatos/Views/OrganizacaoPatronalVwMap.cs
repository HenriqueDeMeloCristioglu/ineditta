using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ineditta.Repository.Sindicatos.Views
{
    public class OrganizacaoPatronalVwMap : IEntityTypeConfiguration<OrganizacaoPatronalVw>
    {
        public void Configure(EntityTypeBuilder<OrganizacaoPatronalVw> builder)
        {
            builder.ToView("organizacao_patronal_vw");
            builder.HasNoKey();

            builder.Property(x => x.SindicatoPatronalId)
                .HasColumnName("id");

            builder.Property(x => x.NomeConfederacao)
                .HasColumnName("nome_confederacao");

            builder.Property(x => x.NomeFederacao)
                .HasColumnName("nome_federacao");

            builder.Property(x => x.Associado)
                .HasColumnName("associado");

            builder.Property(x => x.Sigla)
                .HasColumnName("sigla");

            builder.Property(x => x.Cnpj)
                .HasColumnName("cnpj");
        }
    }
}
