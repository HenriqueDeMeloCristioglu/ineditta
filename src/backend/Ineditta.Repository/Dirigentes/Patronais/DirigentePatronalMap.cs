using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CSharpFunctionalExtensions;

using Ineditta.Application.ClientesUnidades.Entities;
using Ineditta.Application.DirigentesPatronais.Entities;
using Ineditta.Application.Sindicatos.Patronais.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ineditta.Repository.Dirigentes.Patronais
{
    public sealed class DirigentePatronalMap : IEntityTypeConfiguration<DirigentePatronal>
    {
        public void Configure(EntityTypeBuilder<DirigentePatronal> builder)
        {
            builder.ToTable("sind_dirpatro");
            builder.HasKey(e => e.Id).HasName("PRIMARY");

            builder.HasIndex(e => e.SindicatoPatronalId, "fk_sind_dirpatro_sind_patr1");

            builder.Property(e => e.Id).HasColumnName("id_diretoriap");

            builder.Property<int?>("MatrizId").HasColumnName("cliente_matriz_id_empresa");

            builder.Property(e => e.EstabelecimentoId).HasColumnName("cliente_unidades_id_unidade");

            builder.Property(e => e.Nome)
                .HasMaxLength(145)
                .HasColumnName("dirigente_p");

            builder.Property(e => e.Funcao)
                .HasMaxLength(100)
                .HasColumnName("funcao_p");

            builder.Property(e => e.DataInicioMandato).HasColumnName("inicio_mandatop");

            builder.Property(e => e.SindicatoPatronalId).HasColumnName("sind_patr_id_sindp");

            builder.Property(e => e.Situacao)
                .HasMaxLength(45)
                .HasColumnName("situacao_p");

            builder.Property(e => e.DataFimMandato).HasColumnName("termino_mandatop");

            builder.HasOne<SindicatoPatronal>()
                .WithMany()
                .HasForeignKey(d => d.SindicatoPatronalId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_sind_dirpatro_sind_patr1");

            builder.HasOne<ClienteUnidade>()
                .WithMany()
                .HasForeignKey(d => d.EstabelecimentoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_sind_dirpatro_sind_patr2");
        }
    }
}
