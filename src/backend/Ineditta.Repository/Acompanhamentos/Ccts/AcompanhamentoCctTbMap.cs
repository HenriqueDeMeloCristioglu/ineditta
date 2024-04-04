using Ineditta.Application.Acompanhamentos.Ccts.Entities;
using Ineditta.Application.Acompanhamentos.CctsStatusOpcoes.Entities;
using Ineditta.Application.AcompanhamentosCcts.Entities;
using Ineditta.Application.CctsFases.Entities;
using Ineditta.Application.TiposDocumentos.Entities;
using Ineditta.Repository.Converters;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ineditta.Repository.AcompanhamentosCct.Ccts
{
    internal sealed class AcompanhamentoCctTbMap : IEntityTypeConfiguration<AcompanhamentoCct>
    {
        public void Configure(EntityTypeBuilder<AcompanhamentoCct> builder)
        {
            builder.HasKey(e => e.Id).HasName("PRIMARY");

            builder.ToTable("acompanhamento_cct_tb");

            builder.HasIndex(e => e.FaseId, "fases_cct_id_fases_ifbk2");

            builder.HasIndex(e => e.TipoDocumentoId, "tipo_doc_idtipo_doc_ifbk5");

            builder.Property(e => e.Id)
                .HasColumnName("id");

            builder.Property(e => e.DataBase)
                .HasMaxLength(10)
                .HasColumnName("data_base");

            builder.Property(e => e.DataFinal).HasColumnName("data_final");

            builder.Property(e => e.DataInicial).HasColumnName("data_inicial");

            builder.Property(e => e.FaseId)
                .HasColumnName("fase_id");

            builder.Property(e => e.CnaesIds)
                .HasColumnType("json")
                .HasConversion<GenericConverter<IEnumerable<string>>>()
                .HasColumnName("cnaes_ids");

            builder.Property(e => e.GruposEconomicosIds)
                .HasColumnType("json")
                .HasConversion<GenericConverter<IEnumerable<int>>>()
                .HasColumnName("grupos_economicos_ids");

            builder.Property(e => e.EmpresasIds)
                .HasColumnType("json")
                .HasConversion<GenericConverter<IEnumerable<int>>>()
                .HasColumnName("empresas_ids");

            builder.Property(e => e.ObservacoesGerais)
                .HasColumnType("text")
                .HasColumnName("observacoes_gerais");

            builder.Property(e => e.ProximaLigacao).HasColumnName("proxima_ligacao");

            builder.Property(e => e.ScriptsSalvos)
                .HasDefaultValueSql("_utf8mb4\\'[]\\'")
                .HasColumnType("json")
                .HasColumnName("scripts_salvos")
                .HasConversion(new GenericPrivateResolverConverter<IEnumerable<Script>>())
                .HasField("_scriptsSalvos")
                .UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.Property(e => e.Anotacoes)
                .HasColumnType("LONGTEXT")
                .HasColumnName("anotacoes");

            builder.Property(e => e.StatusId)
                .HasMaxLength(15)
                .HasColumnName("status");

            builder.Property(e => e.TipoDocumentoId)
                .HasColumnName("tipo_documento_id");

            builder.Property(e => e.UsuarioResponsavelId)
                .HasColumnName("usuario_responsavel_id");

            builder.Property(e => e.DataProcessamento)
                .HasColumnName("data_processamento");

            builder.Property(e => e.ValidadeFinal)
                .HasColumnName("validade_final");

            builder.HasOne<FasesCct>()
                .WithMany()
                .HasForeignKey(a => a.FaseId)
                .HasConstraintName("fk_acompanhanto_cct_x_fase_cct");

            builder.HasMany(p => p.Assuntos)
                   .WithOne()
                   .HasForeignKey("AcompanhamentoCctId")
                   .HasConstraintName("fk_acompanhamento_cct_assunto_x_acompanhamento_cct")
                   .OnDelete(DeleteBehavior.Cascade)
                   .Metadata.PrincipalToDependent!.SetPropertyAccessMode(PropertyAccessMode.Field);

            builder.HasMany(p => p.Etiquetas)
                   .WithOne()
                   .HasForeignKey("AcompanhamentoCctId")
                   .HasConstraintName("fk_acompanhamento_cct_etiqueta_x_acompanhamento_cct")
                   .OnDelete(DeleteBehavior.Cascade)
                   .Metadata.PrincipalToDependent!.SetPropertyAccessMode(PropertyAccessMode.Field);

            builder.HasOne<AcompanhamentoCctStatus>()
                .WithMany()
                .HasForeignKey(a => a.StatusId)
                .HasConstraintName("fk_acompanhanto_cct_x_acompanhamento_cct_status_opcao");

            builder.HasOne<TipoDocumento>()
                .WithMany()
                .HasForeignKey(a => a.TipoDocumentoId)
                .HasConstraintName("fk_acompanhanto_cct_x_tipo_doc");
        }
    }
}
