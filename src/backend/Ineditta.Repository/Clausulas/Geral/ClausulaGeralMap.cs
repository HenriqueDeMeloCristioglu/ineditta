using Ineditta.Application.Clausulas.Gerais.Entities;
using Ineditta.Application.Documentos.Sindicais.Entities;
using Ineditta.Application.EstruturasClausulas.Gerais.Entities;
using Ineditta.Application.Usuarios.Entities;
using Ineditta.Repository.Converters;
using Ineditta.Repository.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

using Sinonimo = Ineditta.Application.Sinonimos.Entities.Sinonimo;

namespace Ineditta.Repository.Clausulas.Geral
{
    public class ClausulaGeralMap : IEntityTypeConfiguration<ClausulaGeral>
    {
        public void Configure(EntityTypeBuilder<ClausulaGeral> builder)
        {
            builder.ToTable("clausula_geral");

            builder.HasIndex(e => e.DocumentoSindicalId, "fk_clausula_geral_doc_sind1_idx");

            builder.HasKey(e => e.Id)
                .HasName("PRIMARY");

            builder.Property(e => e.Id)
                .HasColumnName("id_clau");

            builder.Property(e => e.Aprovado)
                .HasMaxLength(45)
                .HasDefaultValueSql("nao")
                .HasConversion(e => e.HasValue && e.Value ? "sim" : "nao", e => e == "sim")
                .HasColumnName("aprovado");

            builder.Property(e => e.UsuarioAprovadorId)
                .HasColumnName("aprovador");

            builder.Property(e => e.AssuntoId)
                .HasColumnName("assunto_idassunto");

            builder.Property(e => e.DataAprovacao)
                .HasColumnName("data_aprovacao");

            builder.Property(e => e.DataProcessamento)
                .HasColumnName("data_processamento");

            builder.Property(e => e.DocumentoSindicalId)
                .HasColumnName("doc_sind_id_documento");

            builder.Property(e => e.EstruturaClausulaId)
                .HasColumnName("estrutura_id_estruturaclausula");

            builder.Property(e => e.Liberado)
                .HasMaxLength(2)
                .HasConversion(e => e.HasValue && e.Value ? "S" : null, e => e == "S")
                .HasColumnName("liberado");

            builder.Property(e => e.Numero)
                .HasColumnName("numero_clausula");

            builder.Property(e => e.ResponsavelProcessamento)
                .HasColumnName("responsavel_processamento");

            builder.Property(e => e.SinonimoId)
                .HasColumnName("sinonimo_id")
                .IsRequired(false);

            builder.Property(e => e.Texto)
                .HasColumnName("tex_clau");

            builder.Property(e => e.TextoResumido)
                .HasColumnName("texto_resumido");

            builder.Property(e => e.ConstaNoDocumento)
                .HasConversion(new BooleanToIntConverter())
                .HasColumnName("consta_no_documento");

            builder.Property(e => e.DataProcessamentoDocumento)
                .HasColumnName("data_processamento_documento");

            builder.Property(e => e.ResumoStatus)
                .HasColumnName("resumo_status");

            builder.HasOne<EstruturaClausula>()
                .WithMany()
                .HasForeignKey(c => c.EstruturaClausulaId)
                .HasConstraintName("fk_clausula_geral_x_estrutura_clausula");

            builder.HasOne<DocumentoSindical>()
                .WithMany()
                .HasForeignKey(c => c.DocumentoSindicalId)
                .HasConstraintName("fk_clausula_geral_x_doc_sind");

            builder.HasOne<Usuario>()
                .WithMany()
                .HasForeignKey(c => c.ResponsavelProcessamento)
                .HasConstraintName("fk_clausula_geral_x_usuario_adm");

            builder.HasOne<Assunto>()
                .WithMany()
                .HasForeignKey(c => c.AssuntoId)
                .HasConstraintName("fk_clausula_geral_x_assunto");

            builder.HasOne<Sinonimo>()
                .WithMany()
                .HasForeignKey(c => c.SinonimoId)
                .HasConstraintName("fk_clausula_geral_x_sinonimos");
        }
    }
}
