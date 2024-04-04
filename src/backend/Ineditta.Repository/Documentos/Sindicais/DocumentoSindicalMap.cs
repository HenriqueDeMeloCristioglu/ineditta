using Ineditta.Application.Documentos.Sindicais.Dtos;
using Ineditta.Application.Documentos.Sindicais.Entities;
using Ineditta.Repository.Converters;
using Ineditta.Repository.Documentos.Sindicais.Converters;
using Ineditta.Repository.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using TipoModulo = Ineditta.Application.Documentos.Sindicais.Entities.TipoModulo;

namespace Ineditta.Repository.DocumentosSindicais
{
    internal sealed class DocumentoSindicalMap : IEntityTypeConfiguration<DocumentoSindical>
    {
        public void Configure(EntityTypeBuilder<DocumentoSindical> builder)
        {
            builder.ToTable("doc_sind");

            builder
                .HasKey(e => e.Id)
                .HasName("PRIMARY");

            builder
                .Property(e => e.Id)
                .HasColumnName("id_doc");

            builder.Property(e => e.Anuencia)
               .HasMaxLength(3)
               .HasColumnName("anuencia");

            builder
                .Property(e => e.CaminhoArquivo)
                .HasMaxLength(255)
                .HasColumnName("caminho_arquivo");

            builder
                .Property(e => e.DataSla)
                .HasColumnName("data_sla");

            builder
                .Property(e => e.DataAprovacao)
                .HasColumnName("data_aprovacao");
            builder
                .Property(e => e.DataAssinatura)
                .HasColumnName("data_assinatura");

            builder.Property(p => p.DataAtualizacao)
                        .HasColumnType("date")
                        .HasColumnName("data_atualizacao");

            builder
                .Property<DateOnly?>("DataCadastro")
                .HasDefaultValueSql("'0000-00-00'")
                .HasColumnName("data_cadastro");

            builder
                .Property(e => e.DataRegistroMTE)
                .HasColumnName("data_reg_mte");

            builder.Property(e => e.DataScrap)
                .HasColumnName("data_scrap");

            builder.Property(e => e.DataUpload)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("data_upload");

            builder.Property(e => e.Database)
             .HasMaxLength(10)
             .HasColumnName("database_doc");

            builder.Property(e => e.Descricao)
               .HasMaxLength(300)
               .HasColumnName("descricao_documento");

            builder.Property(e => e.Restrito)
               .HasMaxLength(5)
               .HasColumnName("doc_restrito")
               .HasConversion(new BooleanToStringFullConverter())
               .HasDefaultValueSql("'Não'")
               .IsRequired();

            builder.Property(e => e.DocumentoLocalizacaoId)
                .HasColumnName("documento_id_documento");

            builder.Property(e => e.FonteWeb)
                .HasMaxLength(500)
                .HasColumnName("fonte_web");

            builder.Property("FormularioComunicado")
                .HasColumnType("json")
                .HasColumnName("formulario_comunicado");

            builder.Property(e => e.Modulo)
                .HasMaxLength(45)
                .HasDefaultValueSql("''")
                .HasColumnName("modulo")
                .HasConversion(new Converters.EnumToStringConverter<TipoModulo>());

            builder.Property(e => e.NumeroRegistroMTE)
                .HasMaxLength(20)
                .HasColumnName("num_reg_mte")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");

            builder.Property(e => e.NumeroLei)
                .HasMaxLength(45)
                .HasColumnName("numero_lei");

            builder.Property(e => e.NumeroSolicitacao)
                .HasMaxLength(20)
                .HasColumnName("numero_solicitacao_mr")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");

            builder.Property(e => e.Observacao)
                .HasColumnType("text")
                .HasColumnName("observacao")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");

            builder.Property(e => e.Origem)
                .HasMaxLength(45)
                .HasColumnName("origem")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");

            builder.Property(e => e.Permissao)
                .HasMaxLength(3)
                .HasColumnName("permissao")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");

            builder
                .Property(e => e.DataProrrogacao).HasColumnName("prorrogacao_doc");

            builder.Property(e => e.Referencias)
                .HasColumnType("json")
                .HasColumnName("referencia")
                .HasField("_referencias")
                .UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.Property(e => e.ScrapAprovado)
                .HasMaxLength(2)
                .HasColumnName("scrap_aprovado");

            builder.Property(e => e.SlaEntrega)
                .HasMaxLength(2)
                .HasColumnName("sla_entrega");

            builder.Property(e => e.Status)
                .HasMaxLength(15)
                .HasColumnName("status");

            builder
                .Property<int?>("UsuarioAdmnistradorId")
                .HasColumnName("usuario_adm_id_usuario");

            builder
                .Property(e => e.TipoDocumentoId)
                .HasColumnName("tipo_doc_idtipo_doc");

            builder
                .Property(e => e.TipoNegocioId)
                .HasColumnName("tipounidade_cliente_id_tiponegocio");

            builder.Property(e => e.Titulo)
                .HasMaxLength(255)
                .HasDefaultValueSql("''")
                .HasColumnName("titulo_documento");

            builder.Property(e => e.Uf)
                .HasMaxLength(2)
                .HasColumnName("uf");

            builder
                .Property(e => e.UsuarioAprovadorId)
                .HasColumnName("usuario_aprovador");

            builder
                .Property<int?>("UsuarioCadastroId")
                .HasColumnName("usuario_cadastro");

            builder
                .Property(e => e.UsuarioResponsavelId)
                .HasColumnName("usuario_responsavel");

            builder
                .Property(e => e.DataValidadeFinal)
                .HasColumnName("validade_final");

            builder
                .Property(e => e.DataValidadeInicial)
                .HasColumnName("validade_inicial");

            builder.Property(e => e.Versao)
                .HasMaxLength(45)
                .HasDefaultValueSql("''")
                .HasColumnName("versao_documento");

            builder
                .Property(e => e.DataLiberacao)
                .HasDefaultValueSql("'0000-00-00'")
                .HasColumnName("data_liberacao_clausulas");

            builder
                .Property(e => e.NomeArquivo)
                .HasColumnName("nome_arquivo");

            builder.HasOne<TipounidadeCliente>()
                   .WithMany()
                   .HasForeignKey(d => d.TipoNegocioId)
                   .OnDelete(DeleteBehavior.Cascade)
                   .HasConstraintName("doc_sind_ibfk_4");

            builder.Property(e => e.SindicatosPatronais)
                .HasColumnType("json")
                .HasColumnName("sind_patronal")
                .HasField("_sindicatoPatronais")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasConversion(new GenericPrivateResolverCamelCaseConverter<IEnumerable<SindicatoPatronal>?>());

            builder.Property(e => e.SindicatosLaborais)
                .HasColumnType("json")
                .HasColumnName("sind_laboral")
                .HasField("_sindicatoLaborais")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasConversion(new GenericPrivateResolverCamelCaseConverter<IEnumerable<SindicatoLaboral>?>());

            builder.Property(e => e.Cnaes)
                .HasColumnType("json")
                .HasColumnName("cnae_doc")
                .HasField("_cnaes")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasConversion(new GenericPrivateResolverCamelCaseConverter<IEnumerable<Cnae>?>());

            builder.Property(e => e.Abrangencias)
                .HasColumnType("json")
                .HasColumnName("abrangencia")
                .HasField("_abrangencias")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasConversion(new AbrangenciaConverter());

            builder.Property(e => e.Estabelecimentos)
                .HasColumnType("json")
                .HasColumnName("cliente_estabelecimento")
                .HasField("_estabelecimentos")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasConversion(new GenericPrivateResolverCamelCaseConverter<IEnumerable<Estabelecimento>?>());
        }
    }
}
