using Ineditta.Application.ClientesMatriz.Entities;
using Ineditta.Application.ClientesUnidades.Entities;
using Ineditta.Application.GruposEconomicos.Entities;
using Ineditta.Application.Localizacoes.Entities;
using Ineditta.Repository.Converters;
using Ineditta.Repository.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Localizacao = Ineditta.Application.Localizacoes.Entities.Localizacao;

namespace Ineditta.Repository.ClientesUnidades
{
    
    internal sealed class ClienteUnidadeMap : IEntityTypeConfiguration<ClienteUnidade>
    {
        public void Configure(EntityTypeBuilder<ClienteUnidade> builder)
        {
            builder.HasKey(e => e.Id).HasName("PRIMARY");

            builder.ToTable("cliente_unidades");

            builder.Property(e => e.Nome)
                .HasMaxLength(65)
                .HasColumnName("nome_unidade");

            builder.HasIndex(e => e.EmpresaId, "cliente_matriz_id_empresa");

            builder.HasIndex(e => e.LocalizacaoId, "localizacao_id_localizacao");

            builder.HasIndex(e => e.TipoNegocioId, "tipounidade_cliente_id_tiponegocio");

            builder.Property(e => e.Id).HasColumnName("id_unidade");
            builder.OwnsOne(p => p.Logradouro, logrd =>
            {
                logrd.Property(e => e.Bairro)
                .HasMaxLength(85)
                .HasColumnName("bairro");

                logrd.OwnsOne(e => e.Cep, v =>
                {
                    v.Property(r => r.Value)
                    .HasMaxLength(20)
                    .HasColumnName("cep");
                });

                logrd.Property(e => e.Regiao)
                    .HasMaxLength(45)
                    .HasColumnName("regional");

                logrd.Property(e => e.Endereco)
                    .HasMaxLength(145)
                    .HasColumnName("logradouro");
            });
            builder.Property<int>("GrupoEconomicoId")
                .HasColumnName("cliente_grupo_id_grupo_economico");
            builder.Property(e => e.EmpresaId).HasColumnName("cliente_matriz_id_empresa");
            builder.Property(e => e.CnaesUnidades)
                .HasColumnType("json")
                .HasColumnName("cnae_unidade")
                .HasField("_cnaesUnidades")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasConversion(new GenericPrivateResolverCamelCaseConverter<IEnumerable<CnaeUnidade>?>());

            builder.OwnsOne(e => e.Cnpj, p =>
            {
                p.Property(el => el.Value)
                .HasColumnName("cnpj_unidade")
                .HasMaxLength(20);
            });

            builder.Property(e => e.CodigoSindicatoCliente)
                .HasMaxLength(35)
                .HasColumnName("cod_sindcliente");
            builder.Property(e => e.CodigoSindicatoPatronal)
                .HasMaxLength(35)
                .HasColumnName("cod_sindpatrocliente");
            builder.Property(e => e.Codigo)
                .HasMaxLength(45)
                .HasColumnName("codigo_unidade");
            builder.Property(e => e.DataAusencia).HasColumnName("data_inativo");

            builder.Property<DateTime>("DataInclusao")
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("data_inclusao");

            builder.Property(e => e.LocalizacaoId).HasColumnName("localizacao_id_localizacao");
            builder.Property<string>("NomeEmpresa")
                .HasMaxLength(90)
                .HasColumnName("nome_empresa");
            builder.Property<string>("NomeGrupoEconomico")
                .HasColumnType("text")
                .HasColumnName("nome_grupoeconomico");
            builder.Property(e => e.TipoNegocioId).HasColumnName("tipounidade_cliente_id_tiponegocio");

            builder.Property(e => e.CnaeFilial)
            .HasColumnName("cnae_filial");

            builder
                .HasOne<ClienteMatriz>()
                .WithMany()
                .HasForeignKey(d => d.EmpresaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("cliente_unidades_ibfk_1");

            builder.HasOne<Localizacao>()
                .WithMany()
                .HasForeignKey(d => d.LocalizacaoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("cliente_unidades_ibfk_3");

            builder.HasOne<TipounidadeCliente>()
                .WithMany(p => p.ClienteUnidades)
                .HasForeignKey(d => d.TipoNegocioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("cliente_unidades_ibfk_2");
        }
    }
}
