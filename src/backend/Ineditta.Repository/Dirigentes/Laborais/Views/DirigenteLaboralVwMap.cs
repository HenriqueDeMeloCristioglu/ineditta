using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ineditta.Repository.Dirigentes.Laborais.Views
{
    public class DirigenteLaboralVwMap : IEntityTypeConfiguration<DirigenteLaboralVw>
    {
        public void Configure(EntityTypeBuilder<DirigenteLaboralVw> builder)
        {
            builder.ToView("dirigente_laboral_vw");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Nome).HasColumnName("nome");

            builder.Property(e => e.Sigla).HasColumnName("sigla");

            builder.Property(e => e.Situacao).HasColumnName("situacao");

            builder.Property(e => e.Grupo).HasColumnName("grupo");

            builder.Property(e => e.Cargo).HasColumnName("cargo");

            builder.Property(e => e.RazaoSocial).HasColumnName("razao_social");

            builder.Property(e => e.InicioMandato).HasColumnName("inicio_mandato");

            builder.Property(e => e.TerminoMandato).HasColumnName("termino_mandato");

            builder.Property(e => e.NomeUnidade).HasColumnName("nome_unidade");

            builder.Property(e => e.UnidadeId).HasColumnName("unidade_id");

            builder.Property(e => e.SindeId).HasColumnName("sinde_id");
        }
    }
}
