using Ineditta.Application.Usuarios.Entities;
using Ineditta.Repository.Converters;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ineditta.Repository.Usuarios
{
    internal sealed class UsuarioAdmsMap : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> builder)
        {
            builder.HasKey(e => e.Id).HasName("PRIMARY");

            builder.ToTable("usuario_adm");

            builder.HasIndex(e => e.JornadaId, "id_jornada_jornada");

            builder.HasIndex(e => e.IdSuperior, "id_user_superior");

            builder.Property(e => e.Id).HasColumnName("id_user");

            builder.Property<int>("AdmGlobal")
                   .HasColumnName("adm_global")
                   .HasDefaultValueSql("'0'");

            builder.OwnsOne(p => p.Ausencia, aust =>
            {
                aust.Property(p => p.DataInicial)
                    .HasDefaultValueSql("str_to_date(_utf8mb4\\'\\',_utf8mb4\\'%d/%m/%Y\\')")
                    .HasColumnName("ausencia_inicio");

                aust.Property(p => p.DataFinal)
                    .HasDefaultValueSql("str_to_date(_utf8mb4\\'\\',_utf8mb4\\'%d/%m/%Y\\')")
                    .HasColumnName("ausencia_fim");
            });

            builder.Property(e => e.Cargo)
                .HasMaxLength(85)
                .HasColumnName("cargo");

            builder.Property(e => e.Departamento)
                .HasMaxLength(50)
                .HasColumnName("departamento");

            builder.Property(e => e.DocumentoRestrito).HasColumnName("documento_restrito").HasConversion(new BooleanToIntConverter());

            builder.OwnsOne(p => p.Email, emailBuilder => emailBuilder.Property(p => p.Valor)
                            .HasMaxLength(125)
                            .HasColumnName("email_usuario"));

            builder.Property(p => p.Foto).HasColumnName("foto");

            builder.Property(e => e.GrupoEconomicoId)
                .HasDefaultValueSql("'0'")
                .HasColumnName("id_grupoecon");
            builder.Property(e => e.JornadaId).HasColumnName("id_jornada_jornada");
            builder.Property(e => e.IdSuperior).HasColumnName("id_user_superior");
            builder.Property(e => e.CnaesIds)
                .HasColumnType("json")
                .HasColumnName("ids_cnae")
                .HasConversion(new GenericConverter<int[]>());

            builder.Property(e => e.GruposClausulasIds)
                .HasColumnType("json")
                .HasColumnName("ids_gruc")
                .HasConversion(new GenericConverter<int[]>());

            builder.Property(e => e.LocalidadesIds)
                .HasColumnType("json")
                .HasColumnName("ids_localidade")
                .HasConversion(new GenericConverter<int[]>());

            builder.Property(e => e.EstabelecimentosIds)
                .HasColumnType("json")
                .HasColumnName("ids_fmge")
                .HasConversion(new GenericConverter<int[]>());

            builder.Property(e => e.Bloqueado)
                .HasDefaultValueSql("'0'")
                .HasColumnName("is_blocked")
                .HasConversion(new BooleanToIntConverter());
            builder.Property(e => e.ModulosComerciais)
                .HasColumnType("json")
                .HasColumnName("modulos_comercial")
                .HasField("_modulosComerciais")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasConversion(new UsuarioModuloConverter());
            builder.Property(e => e.ModulosSISAP)
                .HasColumnType("json")
                .HasColumnName("modulos_sisap")
                .HasField("_modulosSISAP")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasConversion(new UsuarioModuloConverter());
            builder.Property(e => e.Nivel)
                .HasMaxLength(20)
                .HasDefaultValueSql("'Ineditta'")
                .HasColumnName("nivel")
                .HasConversion(new NivelConverter());
            builder.Property(e => e.Nome)
                .HasMaxLength(145)
                .HasColumnName("nome_usuario");
            builder.Property(e => e.NotificarEmail)
                .HasDefaultValueSql("'0'")
                .HasColumnName("notifica_email")
                .HasConversion(new BooleanToIntConverter());
            builder.Property(e => e.NotificarWhatsapp)
                .HasDefaultValueSql("'0'")
                .HasColumnName("notifica_whatsapp")
                .HasConversion(new BooleanToIntConverter());
            builder.Property(e => e.Ramal)
                .HasMaxLength(20)
                .HasColumnName("ramal");
            builder.Property<string>("SenhaAdm")
                   .HasColumnName("senha_adm")
                   .HasDefaultValueSql("'Ineditta@10'");
            builder.Property(e => e.Celular)
                .HasMaxLength(20)
                .HasColumnName("telefone");
            builder.Property(e => e.Tipo)
                .HasMaxLength(10)
                .HasDefaultValueSql("'Ineditta'")
                .HasColumnName("tipo");
            builder.Property(e => e.TrocaSenhaLogin)
                .HasDefaultValueSql("'0'")
                .HasColumnName("troca_senha_login")
                .HasConversion(new BooleanToIntConverter());
        }
    }
}
