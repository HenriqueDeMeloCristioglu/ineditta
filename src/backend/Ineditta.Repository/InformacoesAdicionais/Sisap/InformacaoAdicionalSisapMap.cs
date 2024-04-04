using Ineditta.Application.Clausulas.Gerais.Entities;
using Ineditta.Application.Documentos.Sindicais.Entities;
using Ineditta.Application.EstruturasClausulas.Gerais.Entities;
using Ineditta.Application.InformacoesAdicionais.Sisap.Entities;
using Ineditta.Repository.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ineditta.Repository.InformacoesAdicionais.Sisap
{
    public sealed class InformacaoAdicionalSisapMap : IEntityTypeConfiguration<InformacaoAdicionalSisap>
    {
        public void Configure(EntityTypeBuilder<InformacaoAdicionalSisap> builder)
        {
            builder.ToTable("clausula_geral_estrutura_clausula");

            builder.HasKey(e => e.Id)
                .HasName("PRIMARY");

            builder.Property(e => e.Id)
                .HasColumnName("id_clausulageral_estrutura_clausula");

            builder.Property(e => e.TipoinformacaoadicionalId)
                .HasColumnName("ad_tipoinformacaoadicional_cdtipoinformacaoadicional");

            builder.Property(e => e.ClausulaGeralId)
                .HasColumnName("clausula_geral_id_clau");

            builder.Property(e => e.Combo)
                .HasMaxLength(45)
                .HasColumnName("combo");

            builder.Property(e => e.Data)
                .HasMaxLength(10)
                .HasColumnName("data");

            builder.Property(e => e.Descricao)
                .HasColumnType("text")
                .HasColumnName("descricao");

            builder.Property(e => e.DocumentoSindicalId)
                .HasColumnName("doc_sind_id_doc");

            builder.Property(e => e.EstruturaClausulaId)
                .HasColumnName("estrutura_clausula_id_estruturaclausula");

            builder.Property(e => e.SequenciaLinha)
                .HasMaxLength(2)
                .HasColumnName("grupo_dados");

            builder.Property(e => e.Hora)
                .HasMaxLength(45)
                .HasColumnName("hora");

            builder.Property(e => e.InforamcacaoAdicionalGrupoId)
                .HasColumnName("id_info_tipo_grupo");

            builder.Property(e => e.NomeInformacaoEstruturaClausulaId)
                .HasColumnName("nome_informacao");

            builder.Property(e => e.Numerico)
                .HasDefaultValue(0.00m)
                .HasColumnType("decimal(18,2)")
                .HasColumnName("numerico");

            builder.Property(e => e.Percentual)
                .HasDefaultValue(0.00m)
                .HasColumnType("decimal(18,2)")
                .HasColumnName("percentual");

            builder.Property(e => e.SequenciaItem)
                .HasColumnName("sequencia");

            builder.Property(e => e.Texto)
                .HasMaxLength(150)
                .HasColumnName("texto");

            builder.HasOne<ClausulaGeral>()
                .WithMany()
                .HasForeignKey(i => i.ClausulaGeralId)
                .HasConstraintName("fk_clausula_geral_estrutura_clausula_x_clausula_geral");

            builder.HasOne<EstruturaClausula>()
                .WithMany()
                .HasForeignKey(i => i.EstruturaClausulaId)
                .HasConstraintName("fk_clausula_geral_estrutura_clausula_x_estrutura_clausula");

            builder.HasOne<AdTipoinformacaoadicional>()
                .WithMany()
                .HasForeignKey(i => i.TipoinformacaoadicionalId)
                .HasConstraintName("fk_clausula_geral_estrutura_clausula_x_ad_tipoinformacaoadicional");

            builder.HasOne<DocumentoSindical>()
                .WithMany()
                .HasForeignKey(i => i.DocumentoSindicalId)
                .HasConstraintName("fk_clausula_geral_estrutura_clausula_x_doc_sind");

            builder.HasOne<AdTipoinformacaoadicional>()
                .WithMany()
                .HasForeignKey(i => i.InforamcacaoAdicionalGrupoId)
                .HasConstraintName("fk_clausula_geral_estrutura_clausula_x_info_grupo");

            builder.HasOne<EstruturaClausula>()
                .WithMany()
                .HasForeignKey(i => i.NomeInformacaoEstruturaClausulaId)
                .HasConstraintName("fk_clausula_geral_estrutura_clausula_x_nome_estrutura_clausula");
        }
    }
}
