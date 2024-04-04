using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Ineditta.Repository.AcompanhamentosCct.Views;
using Ineditta.Repository.Converters;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ineditta.Repository.Usuarios.Views
{
    public class UsuariosAdmsVwMap : IEntityTypeConfiguration<UsuariosAdmsVw>
    {
        public void Configure(EntityTypeBuilder<UsuariosAdmsVw> builder)
        {
            builder.ToView("usuarios_adms_vw");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Nome).HasColumnName("nome");

            builder.Property(e => e.Email).HasColumnName("email");

            builder.Property(e => e.Cargo).HasColumnName("cargo");

            builder.Property(e => e.Telefone).HasColumnName("telefone");

            builder.Property(e => e.Ramal).HasColumnName("ramal");

            builder.Property(e => e.Departamento).HasColumnName("departamento");

            builder.Property(e => e.IdSuperior).HasColumnName("id_superior");

            builder.Property(e => e.DataCriacao).HasColumnName("data_criacao");

            builder.Property(e => e.NomeUserCriador).HasColumnName("nome_user_criador");

            builder.Property(e => e.IdsFmge)
                .HasColumnType("json")
                .HasColumnName("ids_fmge")
                .HasConversion(new GenericConverter<int[]>());

            builder.Property(e => e.IdGrupoEconomico).HasColumnName("id_grupoecon");

            builder.Property(e => e.Nivel).HasColumnName("nivel");
        }
    }
}
