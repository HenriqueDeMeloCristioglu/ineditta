using Ineditta.Application.ClientesMatriz.Entities;
using Ineditta.Application.GruposEconomicos.Entities;
using Ineditta.Repository.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ineditta.Repository.ClientesMatriz
{
    internal sealed class ClienteMatrizMap : IEntityTypeConfiguration<ClienteMatriz>
    {
        public void Configure(EntityTypeBuilder<ClienteMatriz> builder)
        {
            builder.HasKey(e => e.Id).HasName("PRIMARY");

            builder.ToTable("cliente_matriz");

            builder.HasIndex(e => e.GrupoEconomicoId, "cliente_grupo_id_grupo_economico");

            builder.Property(e => e.Id).HasColumnName("id_empresa");

            builder.Property(e => e.AberturaNegociacao)
                .HasColumnName("abri_neg");

            builder.Property<string>("Bairro")
                .HasColumnType("text")
                .HasMaxLength(95)
                .HasColumnName("bairro");

            builder.Property<string>("Cep")
                .HasColumnType("text")
                .HasMaxLength(8)
                .HasColumnName("cep");

            builder.Property<string>("Cidade")
                .HasColumnType("text")
                .HasMaxLength(145)
                .HasColumnName("cidade");

            builder.Property<string>("ClasseDocumento")
                .HasMaxLength(100)
                .HasColumnType("text")
                .HasColumnName("classe_doc");

            builder.Property(e => e.GrupoEconomicoId)
                .HasColumnName("cliente_grupo_id_grupo_economico");

            builder.Property<string>("Cnpj")
                .HasMaxLength(20)
                .HasColumnName("cnpj_empresa");

            builder.Property(e => e.Codigo)
                .HasMaxLength(20)
                .HasColumnName("codigo_empresa");

            builder.Property(e => e.DataCorteForpag)
                .HasColumnName("data_cortefopag");

            builder.Property(e => e.DataInativacao)
                .HasColumnName("data_inativacao");

            builder.Property<DateTime>("DataInclusao")
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("data_inclusao");

            builder.Property(e => e.Logo)
                .HasMaxLength(200)
                .HasColumnType("text")
                .HasColumnName("logo_empresa");

            builder.Property<string>("Logradouro")
                .HasMaxLength(150)
                .HasColumnType("text")
                .HasColumnName("logradouro_empresa");

            builder.Property(e => e.Nome)
                .HasMaxLength(90)
                .HasColumnName("nome_empresa");

            builder.Property<string>("RazaoSocial")
                .HasMaxLength(115)
                .HasColumnName("razao_social");

            builder.Property<string>("SlaEntrega")
                .HasColumnType("text")
                .HasMaxLength(2)
                .HasColumnName("sla_entrega");

            builder.Property(e => e.SlaPrioridade)
                .HasColumnType("text")
                .HasMaxLength(45)
                .HasColumnName("sla_prioridade");

            builder.Property(e => e.TipoProcessamento)
                .HasMaxLength(45)
                .HasColumnName("tipo_processamento");

            builder.Property<string>("Uf")
                .HasColumnType("text")
                .HasMaxLength(2)
                .HasColumnName("uf");

            builder.HasOne<GrupoEconomico>()
                .WithMany()
                .HasForeignKey(d => d.GrupoEconomicoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("cliente_matriz_ibfk_1");
        }
    }
}
