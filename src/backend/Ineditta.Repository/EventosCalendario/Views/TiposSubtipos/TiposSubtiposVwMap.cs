using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ineditta.Repository.EventosCalendario.Views.TiposSubtipos
{
    public class TiposSubtiposVwMap : IEntityTypeConfiguration<TiposSubtiposVw>
    {
        public void Configure(EntityTypeBuilder<TiposSubtiposVw> builder)
        {
            builder.ToView("tipos_subtipos_evento_calendario_sindical_vw");
            builder.HasNoKey();

            builder.Property(p => p.Id)
                .HasColumnName("id");

            builder.Property(p => p.Nome)
                .HasColumnName("nome");

            builder.Property(p => p.TipoAssociado)
                .HasColumnName("tipo_associado");
        }
    }
}
