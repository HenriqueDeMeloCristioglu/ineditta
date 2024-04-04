using Ineditta.Application.Jornada.Entities;
using Ineditta.Application.Usuarios.Entities;
using Ineditta.Repository.Models;
using Ineditta.Repository.Usuarios;
using Microsoft.EntityFrameworkCore;
using Dapper;
using System.Dynamic;
using Ineditta.Repository.Dapper.Mappers;
using Ineditta.BuildingBlocks.Core.Auth;
using Ineditta.BuildingBlocks.Core.Domain.Contracts;
using Ineditta.Application.ClientesUnidades.Entities;
using Ineditta.Repository.Comentarios.Views;
using Ineditta.BuildingBlocks.Core.Idempotency.Database;
using Ineditta.Repository.Usuarios.Views;
using Ineditta.Repository.ClientesUnidades.Views;
using Ineditta.Repository.MapasSindicais.Views;
using Ineditta.Repository.Clausulas.Views.InformacoesAdicionais;
using Ineditta.Application.NotificacaoEventos.Entities;
using Ineditta.Repository.Clausulas.Views.ComparativoMapaSindical;
using Ineditta.Application.InformacoesAdicionais.Cliente.Entities;
using Ineditta.Application.DocumentosLocalizados.Entities;
using Ineditta.Application.GruposEconomicos.Entities;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Ineditta.Application.Sindicatos.Patronais.Entities;
using Ineditta.Application.BasesTerritoriaisPatronais.Entities;
using Ineditta.Repository.Dirigentes.Patronais.Views;
using Ineditta.Repository.Dirigentes.Laborais.Views;
using Ineditta.Application.Sindicatos.Laborais.Entities;
using Ineditta.Application.BasesTerritoriaisLaborais.Entities;
using Ineditta.BuildingBlocks.Core.Database.Interceptors;
using Ineditta.Application.CalendarioSindicais.Usuarios.Entities;
using Ineditta.Application.CalendarioSindicais.Eventos.Entities;
using Ineditta.Application.Comentarios.Entities;
using Ineditta.Application.Etiquetas.Entities;
using Ineditta.Application.TiposEtiquetas.Entities;
using Ineditta.Repository.Comentarios.Views.Comentarios;
using Ineditta.Repository.Sindicatos.Views;
using Ineditta.Application.ClientesMatriz.Entities;
using Ineditta.Application.ModulosClientes.Entities;
using Ineditta.Application.TiposDocumentos.Entities;
using Modulo = Ineditta.Application.Modulos.Entities.Modulo;
using Ineditta.Application.TiposDocumentosClientesMatriz.Entities;
using Ineditta.Repository.AcompanhamentosCct.Views.AcompanhamentosCctsRelatorios;
using Ineditta.Application.TiposEventosCalendarioSindical.Entities;
using Ineditta.Application.SubtiposEventosCalendarioSindical.Entities;
using Ineditta.Application.UsuariosTiposEventosCalendarioSindical.Entities;
using Ineditta.Application.Acompanhamentos.CctsEstabelecimentos.Entities;
using Ineditta.Repository.Acompanhamentos.Ccts.Views.AcompanhamentosCcts;
using Ineditta.Repository.Acompanhamentos.Ccts.Views.AcompanhamentosCctsFuturas;
using Ineditta.Application.Acompanhamentos.CctsSindicatosLaborais.Entities;
using Ineditta.Application.Acompanhamentos.CctsSindicatosPatronais.Entities;
using MediatR;
using CSharpFunctionalExtensions;
using Ineditta.Application.Acompanhamentos.Ccts.Entities;
using Ineditta.Application.CctsFases.Entities;
using Ineditta.Repository.Acompanhamentos.Ccts.Views.AcompanhamentosCctsInclusoes;
using Ineditta.Application.Acompanhamentos.CctsLocalizacoes.Entities;
using Ineditta.Repository.BasesTerritoriaisLaborais.Views.BasesTerritoriaisLaboraisLocalizacoes;
using Ineditta.Application.ClientesUnidadesSindicatosPatronais.Entities;
using Ineditta.Repository.ClassesCnaes.Views;
using Ineditta.Application.Cnaes.Entities;
using Ineditta.Repository.EventosCalendario.Views.EventosCalendariosAssembleiasReunioes;
using Ineditta.Repository.EventosCalendario.Views.EventosCalendariosDescontosPagamentosVencimentos;
using Ineditta.Repository.EventosCalendario.Views.EventosCalendariosTrintidios;
using Ineditta.Repository.EventosCalendario.Views.EventosCalendariosVencimentosDocumentos;
using Ineditta.Repository.EventosCalendario.Views.EventosCalendariosVencimentosMandatosLaborais;
using Ineditta.Repository.EventosCalendario.Views.EventosCalendariosVencimentosMandatosPatronais;
using Ineditta.Repository.EventosCalendario.Views.TiposSubtipos;
using Ineditta.Application.DirigentesPatronais.Entities;
using Localizacao = Ineditta.Application.Localizacoes.Entities.Localizacao;
using Ineditta.Application.InformacoesAdicionais.Sisap.Entities;
using Ineditta.Application.Clausulas.Gerais.Entities;
using Ineditta.Repository.Clausulas.Geral.Views.Clausula;
using Ineditta.Repository.Clausulas.Geral.Views.ClausulaGeral;
using Ineditta.Application.GruposClausulas.Entities;
using Ineditta.Application.Clausulas.Clientes.Entities;
using Ineditta.Application.Emails.StoragesManagers.Entities;
using Ineditta.Application.Emails.CaixasDeSaida.Entities;
using Ineditta.Application.TemplatesEmails.Entities;
using Ineditta.Repository.Audits;
using System.Globalization;
using Newtonsoft.Json;
using Ineditta.Application.AIs.DocumentosSindicais.Entities;
using Ineditta.Application.AIs.Clausulas.Entities;
using Ineditta.Application.Acompanhamentos.CctsAssuntos.Entities;
using Ineditta.Application.Acompanhamentos.CctsStatusOpcoes.Entities;
using Ineditta.Application.Acompanhamentos.CctsEtiquetasOpcoes.Entities;
using Ineditta.Application.Acompanhamentos.CctsEtiquetas.Entities;
using Ineditta.Application.Sinonimos.Entities;
using Ineditta.Application.EstruturasClausulas.GruposEconomicos.Entities;
using Ineditta.Application.EstruturasClausulas.Gerais.Entities;
using Ineditta.Repository.IA.IADocumentosSindicais.Views.IADocumentosSindicais;
using Ineditta.Application.Documentos.Estabelecimentos.Entities;
using Ineditta.Application.Documentos.Localizacoes.Entities;
using Ineditta.Application.Documentos.Sindicais.Entities;
using Ineditta.Application.Documentos.SindicatosLaborais;
using Ineditta.Application.Documentos.SindicatosPatronais;
using Ineditta.Repository.Documentos.Sindicais.Views.DocumentosSindicaisClausulasVw;
using Ineditta.Repository.Documentos.Sindicais.Views.DocumentosSindicaisSisapsVw;
using Ineditta.Repository.Documentos.Sindicais.Views.DocumentosSindicaisVw;
using Ineditta.Repository.Documentos.Sindicais.Views.DocumentosSindicatosEstabelecimentosVw;
using Ineditta.Application.Documentos.AtividadesEconomicas.Entities;

namespace Ineditta.Repository.Contexts;

public partial class InedittaDbContext : DbContext, IDataProtectionKeyContext
{
    static InedittaDbContext()
    {
        DefaultTypeMap.MatchNamesWithUnderscores = true;
        SqlMapper.AddTypeHandler(new DateOnlyTypeHandler());
        SqlMapper.AddTypeHandler(new DateTimeTypeHandler());
    }

    private readonly IUserInfoService _userInfoService;

    public InedittaDbContext()
    {
    }

    public InedittaDbContext(DbContextOptions<InedittaDbContext> options, IUserInfoService userInfoService)
        : base(options)
    {
        _userInfoService = userInfoService;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.AddInterceptors(new TaggedQueryCommandInterceptor());

    public virtual DbSet<AuditTb> AuditTb { get; set; }
    public virtual DbSet<AbrangDocsind> AbrangDocsinds { get; set; }
    public virtual DbSet<AbrangenciaDocumento> AbrangenciaDocumentos { get; set; }
    public virtual DbSet<AcompanhamentoCct> AcompanhamentoCct { get; set; }
    public virtual DbSet<AcompanhamentoCctEstabelecimento> AcompanhamentoCctEstabelecimento { get; set; }
    public virtual DbSet<AcompanhamentoCctSinditoLaboral> AcompanhamentoCctSinditoLaboral { get; set; }
    public virtual DbSet<AcompanhamentoCctSinditoPatronal> AcompanhamentoCctSinditoPatronal { get; set; }
    public virtual DbSet<AcompanhamentoCctLocalizacao> AcompanhamentoCctLocalizacao { get; set; }
    public virtual DbSet<AcompanhamentoCctAssunto> AcompanhamentoCctAssunto { get; set; }
    public virtual DbSet<AcompanhamentoCctStatus> AcompanhamentoCctStatusOpcao { get; set; }
    public virtual DbSet<AcompanhamentoCctEtiqueta> AcompanhamentoCctEtiqueta { get; set; }
    public virtual DbSet<AcompanhamentoCctEtiquetaOpcao> AcompanhamentoCctEtiquetaOpcao { get; set; }
    public virtual DbSet<ComentarioSindicatoVw> ComentarioSindicatoVw { get; set; }
    public virtual DbSet<ComentariosVw> ComentariosVw { get; set; }
    public virtual DbSet<IADocumentoSindicalVw> IADocumentoSindicalVw { get; set; }
    public virtual DbSet<AcompanhamentoCctVw> AcompanhamentoCctVws { get; set; }
    public virtual DbSet<AcompanhamentoCctInclusaoVw> AcompanhamentoCctInclusaoVw { get; set; }
    public virtual DbSet<AcompanhamentoCctFuturasVw> AcompanhamentoCctFuturasVws { get; set; }
    public virtual DbSet<AcompanhamentoCctRelatorioVw> AcompanhamentoCctRelatorioVw { get; set; }
    public virtual DbSet<EmailStorageManager> EmailStorageManager { get; set; }
    public virtual DbSet<EmailCaixaDeSaida> EmailCaixaDeSaida { get; set; }
    public virtual DbSet<AdTipoinformacaoadicional> AdTipoinformacaoadicionals { get; set; }
    public virtual DbSet<AnuenciaInicial> AnuenciaInicials { get; set; }
    public virtual DbSet<AnuenciaUsuario> AnuenciaUsuarios { get; set; }
    public virtual DbSet<Associacao> Associacoes { get; set; }
    public virtual DbSet<Assunto> Assuntos { get; set; }
    public virtual DbSet<Atividade> Atividades { get; set; }
    public virtual DbSet<AtividadesComentario> AtividadesComentarios { get; set; }
    public virtual DbSet<BaseTerritorialLaboral> BaseTerritorialsindemps { get; set; }
    public virtual DbSet<BaseTerritorialPatronal> BaseTerritorialsindpatros { get; set; }
    public virtual DbSet<BasesTerritoriaisLaboraisLocalizacoesVw> BasesTerritoriaisLaboraisLocalizacoesVw { get; set; }
    public virtual DbSet<BdLegadoBuscaRapidum> BdLegadoBuscaRapida { get; set; }
    public virtual DbSet<BdLegadoClausula> BdLegadoClausulas { get; set; }
    public virtual DbSet<CadastroCliente> CadastroClientes { get; set; }
    public virtual DbSet<CalendarioGeralNovo> CalendarioGeralNovos { get; set; }
    public virtual DbSet<CalendarioSindicalUsuario> CalendariosSindicaisUsuario { get; set; }
    public virtual DbSet<CentralSindical> CentralSindicals { get; set; }
    public virtual DbSet<Cnae> ClasseCnaes { get; set; }
    public virtual DbSet<ClasseCnaeDocSind> ClasseCnaeDocSinds { get; set; }
    public virtual DbSet<ClausulaGeral> ClausulaGerals { get; set; }
    public virtual DbSet<ClausulaCliente> ClausulaCliente { get; set; }
    public virtual DbSet<ClausulaGeralVw> ClausulaGeraisVw { get; set; }
    public virtual DbSet<EstruturaClausulaGrupoEconomico> EstruturaClausulaGrupoEconomico { get; set; }
    public virtual DbSet<InformacaoAdicionalSisap> InformacaoAdicionalSisap { get; set; }
    public virtual DbSet<GrupoEconomico> GrupoEconomico { get; set; }
    public virtual DbSet<ClienteMatriz> ClienteMatrizes { get; set; }
    public virtual DbSet<ClienteUnidade> ClienteUnidades { get; set; }
    public virtual DbSet<ClienteUnidadeSindicatoPatronal> ClientesUnidadesSindicatosPatronais { get; set; }
    public virtual DbSet<CnaeEmp> CnaeEmps { get; set; }
    public virtual DbSet<DadosSdf> DadosSdfs { get; set; }
    public virtual DbSet<DiretoriaSindLaboral> DiretoriaSindLaborals { get; set; }
    public virtual DbSet<IADocumentoSindical> IADocumentoSindical { get; set; }
    public virtual DbSet<DocumentoSindical> DocSinds { get; set; }
    public virtual DbSet<DocumentoSindicatoLaboral> DocumentosSindicatosLaborais { get; set; }
    public virtual DbSet<DocumentoSindicatoPatronal> DocumentosSindicatosPatronais { get; set; }
    public virtual DbSet<DocumentoAtividadeEconomica> DocumentoAtividadeEconomica { get; set; }
    public virtual DbSet<DocumentoSindicalSisapVw> DocSindsSisap { get; set; }

    public virtual DbSet<DocSindClienteUnidade> DocSindClienteUnidades { get; set; }

    public virtual DbSet<DocSindReferencium> DocSindReferencia { get; set; }

    public virtual DbSet<DocSindSindEmp> DocSindSindEmps { get; set; }

    public virtual DbSet<DocSindSindPatr> DocSindSindPatrs { get; set; }

    public virtual DbSet<Documento> Documentos { get; set; }

    public virtual DbSet<DocumentoAssunto> DocumentoAssuntos { get; set; }

    public virtual DbSet<DocumentosAbrangencium> DocumentosAbrangencia { get; set; }

    public virtual DbSet<DocumentosCnae> DocumentosCnaes { get; set; }

    public virtual DbSet<DocumentosEmpresa> DocumentosEmpresas { get; set; }

    public virtual DbSet<DocumentoEstabelecimento> DocumentosEstabelecimentos { get; set; }

    public virtual DbSet<DocumentoLocalizacao> DocumentosLocalizacoes { get; set; }

    public virtual DbSet<DocumentoLocalizado> DocumentosLocalizados { get; set; }

    public virtual DbSet<EstruturaClausula> EstruturaClausula { get; set; }

    public virtual DbSet<EstruturaClausulasAdTipoinformacaoadicional> EstruturaClausulasAdTipoinformacaoadicionals { get; set; }

    public virtual DbSet<CalendarioSindical> Eventos { get; set; }
    public virtual DbSet<EventoCalendarioVencimentoDocumentoVw> EventoCalendarioVencimentoDocumentoVw { get; set; }
    public virtual DbSet<EventoCalendarioVencimentoMandatoPatronalVw> EventoCalendarioVencimentoMandatoPatronalVw { get; set; }
    public virtual DbSet<EventoCalendarioVencimentoMandatoLaboralVw> EventoCalendarioVencimentoMandatoLaboralVw { get; set; }
    public virtual DbSet<EventoCalendarioTrintidioVw> EventoCalendarioTrintidioVw { get; set; }
    public virtual DbSet<EventoCalendarioAssembleiaReuniaoVw> EventoCalendarioAssembleiaReuniaoVw { get; set; }
    public virtual DbSet<EventoCalendarioDescontoPagamentoVencimentoVw> EventoCalendarioDescontoPagamentoVencimentoVw { get; set; }

    public virtual DbSet<FasesCct> FasesCct { get; set; }

    public virtual DbSet<Feriado> Feriados { get; set; }

    public virtual DbSet<FiltroCsv> FiltroCsvs { get; set; }

    public virtual DbSet<FormularioFolha> FormularioFolhas { get; set; }

    public virtual DbSet<FormularioGrupo> FormularioGrupos { get; set; }

    public virtual DbSet<GrupoClausula> GrupoClausula { get; set; }

    public virtual DbSet<Helpdesk> Helpdesks { get; set; }

    public virtual DbSet<IAClausula> IAClausulas { get; set; }

    public virtual DbSet<Indecon> Indecons { get; set; }

    public virtual DbSet<IndeconReal> IndeconReals { get; set; }

    public virtual DbSet<InformacaoAdicionalCliente> InformacaoAdicionalCliente { get; set; }

    public virtual DbSet<InformacaoAdicionalCombo> InformacaoAdicionalCombos { get; set; }

    public virtual DbSet<InformacaoAdicionalGrupo> InformacaoAdicionalGrupos { get; set; }

    public virtual DbSet<InformacoesAdicionai> InformacoesAdicionais { get; set; }

    public virtual DbSet<InformacoesEstabelecimentosVw> InformacoesEstabelecimentosVw { get; set; }

    public virtual DbSet<Jfase> Jfases { get; set; }
    public virtual DbSet<JfaseTipoPerguntas> JfaseTipoPerguntas { get; set; }

    public virtual DbSet<Jornada> Jornada { get; set; }

    public virtual DbSet<Localizacao> Localizacoes { get; set; }

    public virtual DbSet<Modulo> Modulos { get; set; }

    public virtual DbSet<ModuloCliente> ModulosClientes { get; set; }

    public virtual DbSet<NegociacaoCalculo> NegociacaoCalculos { get; set; }

    public virtual DbSet<NegociacaoCenario> NegociacaoCenarios { get; set; }

    public virtual DbSet<NegociacaoPautum> NegociacaoPauta { get; set; }

    public virtual DbSet<NegociacaoPremissa> NegociacaoPremissas { get; set; }

    public virtual DbSet<NegociacaoRodadum> NegociacaoRodada { get; set; }

    public virtual DbSet<NegociacaoScript> NegociacaoScripts { get; set; }

    public virtual DbSet<Notificacao> Notificacoes { get; set; }

    public virtual DbSet<Ninfoadicionai> Ninfoadicionais { get; set; }

    public virtual DbSet<Comentario> Comentarios { get; set; }
    public virtual DbSet<Etiqueta> Etiquetas { get; set; }
    public virtual DbSet<TipoEtiqueta> TiposEtiquetas { get; set; }

    public virtual DbSet<RestritoUsuario> RestritoUsuarios { get; set; }

    public virtual DbSet<OrganizacaoPatronalVw> OrganizacaoPatronalVws { get; set; }

    public virtual DbSet<SindDiremp> SindDiremps { get; set; }

    public virtual DbSet<DirigentePatronal> SindDirpatros { get; set; }

    public virtual DbSet<SindicatoLaboral> SindEmps { get; set; }

    public virtual DbSet<SindicatoPatronal> SindPatrs { get; set; }

    public virtual DbSet<Sinonimo> Sinonimos { get; set; }

    public virtual DbSet<SubtipoEventoCalendarioSindical> SubtiposEventosCalendarioSindical { get; set; }

    public virtual DbSet<Tarefa> Tarefas { get; set; }

    public virtual DbSet<TemplateEmail> TemplatesEmails { get; set; }

    public virtual DbSet<TemporarioClausulageral> TemporarioClausulagerals { get; set; }

    public virtual DbSet<TipoDocumento> TipoDocs { get; set; }

    public virtual DbSet<TipoDocumentoClienteMatriz> TiposDocumentosClientesMatriz { get; set; }

    public virtual DbSet<TipounidadeCliente> TipounidadeClientes { get; set; }

    public virtual DbSet<TipoEventoCalendarioSindical> TiposEventosCalendarioSindical { get; set; }

    public virtual DbSet<TiposSubtiposVw> TiposSubtiposVw { get; set; }

    public virtual DbSet<Usuario> UsuarioAdms { get; set; }

    public virtual DbSet<UsuariosAdmsVw> UsuariosAdmsVw { get; set; }

    public virtual DbSet<UsuarioTipoEventoCalendarioSindical> UsuariosTiposEventosCalendarioSindical { get; set; }

    public virtual DbSet<EstabelecimentosVw> EstabelecimentosVw { get; set; }

    public virtual DbSet<ClausulaVw> ClausulasVw { get; set; }

    public virtual DbSet<SindicatoVw> SindicatoVw { get; set; }

    public virtual DbSet<MapaSindicalGerarExcelVw> MapaSindicalGerarExcelVw { get; set; }

    public virtual DbSet<WhCalendario> WhCalendarios { get; set; }

    public DbSet<DocumentoSindicalVw> DocumentosSindicaisVw { get; set; }

    public DbSet<DocumentoSindicalClausulasVw> DocumentoSindicalClausulasVw { get; set; }

    public DbSet<IdempotentModel> IdempotentTb { get; set; }

    public DbSet<ClausulaGeralInformacaoAdicionalVw> ClausulaGeralInformacaoAdicionalVw { get; set; }

    public DbSet<DocumentoMapaSindicalVw> DocumentoMapaSindicalVw { get; set; }

    public DbSet<ComparativoMapaSindicalPrincipalVw> ComparativoMapaSindicalPrincipalVw { get; set; }

    public DbSet<ComparativoMapaSindicalItemVw> ComparativoMapaSindicalItemVw { get; set; }

    public DbSet<ClasseCnaeVw> ClassesCnaesVw { get; set; }

    public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }
    public DbSet<DocumentoSindicatoEstabelecimentoVw> DocumentoSindicatoEstabelecimentoVw { get; set; }

    public DbSet<DirigentePatronalVw> DirigentesPatronaisVw { get; set; }

    public DbSet<DirigenteLaboralVw> DirigentesLaboraisVw { get; set; }

    public DbSet<DocumentosSindicatosMaisRecentesUsuarios> DocumentosSindicatosMaisRecentesUsuarios { get; set; }

    public async ValueTask<IEnumerable<TResult>> SelectFromRawSqlAsync<TResult>(string sql, Dictionary<string, object> parameters)
        where TResult : class
    {
        var param = new ExpandoObject() as IDictionary<string, object>;

        parameters.ToList().ForEach(x => param.Add(x.Key, x.Value));
        return await Database.GetDbConnection().QueryAsync<TResult>(sql, parameters);
    }

    public async Task GetUltimoDocumentoClausulaDoSindicatoAsync(long? usuarioId)
    {
        Dictionary<string, object> parameters = new Dictionary<string, object>();
        parameters.Add("@usuarioId", usuarioId!);

        await SelectFromRawSqlAsync<dynamic>(@"CALL obter_clausulas_ultimo_ano_sindicatos_laborais( @usuarioId );", parameters);

        return;
    }

    public async Task<bool> InserirCnaesDocumentos()
    {
        await SelectFromRawSqlAsync<dynamic>(@"CALL inserir_cnaes_documentos();", new Dictionary<string, object>());

        return true;
    }

    public async ValueTask<bool> InserirEstabelecimentosAcompanhamentoProcedure()
    {
        await SelectFromRawSqlAsync<dynamic>(@"CALL inserir_estabelecimentos_acompanhamento();", new Dictionary<string, object>());

        return true;
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var usuario = await UsuarioAdms.AsNoTracking().SingleOrDefaultAsync(uat => uat.Email.Valor == _userInfoService.GetEmail(), cancellationToken);

        var entries = ChangeTracker
        .Entries()
        .Where(e => e.Entity is IAuditable && (
                e.State == EntityState.Added
                || e.State == EntityState.Modified
                || e.State == EntityState.Deleted));

        if (!(entries?.Any() ?? false) || usuario is null)
        {
            return await base.SaveChangesAsync(cancellationToken);
        }

        OnBeforeSaveChanges(usuario.Id);

        foreach (var entityEntry in entries)
        {
            if (entityEntry.State == EntityState.Deleted) continue;

            if (entityEntry.State == EntityState.Added)
            {
                entityEntry.Property("UsuarioInclusaoId").CurrentValue = usuario!.Id;
                entityEntry.Property("DataInclusao").CurrentValue = DateTime.Now;

                continue;
            }

            entityEntry.Property("UsuarioAlteracaoId").CurrentValue = usuario!.Id;
            entityEntry.Property("DataAlteracao").CurrentValue = DateTime.Now;
        }

        return await base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");


        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UsuarioAdmsMap).Assembly);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(IdempotentModelMap).Assembly);

        modelBuilder.Entity<AbrangDocsind>(entity =>
        {
            entity.HasKey(e => e.IdAbrgdocsind).HasName("PRIMARY");

            entity
                .ToTable("abrang_docsind")
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_general_ci");

            entity.HasIndex(e => e.DocSindIdDocumento, "fk_abrang_docsind_doc_sind1_idx");

            entity.HasIndex(e => e.LocalizacaoIdLocalizacao, "fk_abrang_docsind_localizacao1_idx");

            entity.Property(e => e.IdAbrgdocsind).HasColumnName("id_abrgdocsind");
            entity.Property(e => e.DocSindIdDocumento).HasColumnName("doc_sind_id_documento");
            entity.Property(e => e.LocalizacaoIdLocalizacao).HasColumnName("localizacao_id_localizacao");

            entity.HasOne<DocumentoSindical>(d => d.DocSindIdDocumentoNavigation).WithMany()
                .HasForeignKey(d => d.DocSindIdDocumento)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("abrang_docsind_ibfk_2");

            entity.HasOne(d => d.LocalizacaoIdLocalizacaoNavigation).WithMany()
                .HasForeignKey(d => d.LocalizacaoIdLocalizacao)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("abrang_docsind_ibfk_1");
        });

        modelBuilder.Entity<AbrangenciaDocumento>(entity =>
        {
            entity.HasKey(e => e.IdabrangenciaDocumento).HasName("PRIMARY");

            entity
                .ToTable("abrangencia_documento")
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_general_ci");

            entity.Property(e => e.IdabrangenciaDocumento).HasColumnName("idabrangencia_documento");
            entity.Property(e => e.DocumentosIddocumentos).HasColumnName("documentos_iddocumentos");
            entity.Property(e => e.LocalizacaoIdLocalizacao).HasColumnName("localizacao_id_localizacao");
        });

        modelBuilder.Entity<AdTipoinformacaoadicional>(entity =>
        {
            entity.HasKey(e => e.Cdtipoinformacaoadicional).HasName("PRIMARY");

            entity.ToTable("ad_tipoinformacaoadicional");

            entity.Property(e => e.Cdtipoinformacaoadicional).HasColumnName("cdtipoinformacaoadicional");
            entity.Property(e => e.Calendario)
                .HasMaxLength(2)
                .HasColumnName("calendario");
            entity.Property(e => e.ClasseIa)
                .HasMaxLength(2)
                .HasColumnName("classe_ia");
            entity.Property(e => e.ComboOptions)
                .HasMaxLength(5000)
                .HasColumnName("combo_options");
            entity.Property(e => e.DadoMs)
                .HasMaxLength(2)
                .HasColumnName("dado_ms");
            entity.Property(e => e.Dtultatualizacao).HasColumnName("dtultatualizacao");
            entity.Property(e => e.Idtipodado)
                .HasMaxLength(10)
                .HasColumnName("idtipodado");
            entity.Property(e => e.Nmtipoinformacaoadicional)
                .HasMaxLength(200)
                .HasColumnName("nmtipoinformacaoadicional");
            entity.Property(e => e.SorthName)
                .HasMaxLength(45)
                .HasColumnName("sorth_name");
            entity.Property(e => e.TarefaInf)
                .HasMaxLength(2)
                .HasColumnName("tarefa_inf");
            entity.Property(e => e.UsoDado)
                .HasMaxLength(2)
                .HasColumnName("uso_dado");
        });

        modelBuilder.Entity<AnuenciaInicial>(entity =>
        {
            entity.HasKey(e => e.IdanuenciaInicial).HasName("PRIMARY");

            entity
                .ToTable("anuencia_inicial")
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_general_ci");

            entity.Property(e => e.IdanuenciaInicial).HasColumnName("idanuencia_inicial");
            entity.Property(e => e.DataAnuencia)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("data_anuencia");
            entity.Property(e => e.UsuarioAdmEmail)
                .HasMaxLength(45)
                .HasColumnName("usuario_adm_email");
        });

        modelBuilder.Entity<AnuenciaUsuario>(entity =>
        {
            entity.HasKey(e => e.IdanuenciaUsuarios).HasName("PRIMARY");

            entity
                .ToTable("anuencia_usuarios")
                .UseCollation("utf8mb4_0900_as_ci");

            entity.HasIndex(e => e.DocumentosIddocumentos, "ibfk_documentos_anuencia_idx");

            entity.Property(e => e.IdanuenciaUsuarios).HasColumnName("idanuencia_usuarios");
            entity.Property(e => e.DatadocAnuencia)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("datadoc_anuencia");
            entity.Property(e => e.DocumentosIddocumentos).HasColumnName("documentos_iddocumentos");
            entity.Property(e => e.UsuarioAdmIdUser).HasColumnName("usuario_adm_id_user");

            entity.HasOne(d => d.DocumentosIddocumentosNavigation).WithMany()
                .HasForeignKey(d => d.DocumentosIddocumentos)
                .HasConstraintName("ibfk_documentos_anuencia");
        });

        modelBuilder.Entity<Associacao>(entity =>
        {
            entity.HasKey(e => e.IdAssociacao).HasName("PRIMARY");

            entity
                .ToTable("associacao")
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_general_ci");

            entity.Property(e => e.IdAssociacao).HasColumnName("id_associacao");
            entity.Property(e => e.AreaGeoeconomica)
                .HasMaxLength(10)
                .HasColumnName("area_geoeconomica");
            entity.Property(e => e.Bairro)
                .HasMaxLength(95)
                .HasColumnName("bairro");
            entity.Property(e => e.Categoria)
                .HasColumnType("text")
                .HasColumnName("categoria");
            entity.Property(e => e.Cep)
                .HasMaxLength(20)
                .HasColumnName("cep");
            entity.Property(e => e.Classe)
                .HasMaxLength(22)
                .HasColumnName("classe");
            entity.Property(e => e.Cnpj)
                .HasMaxLength(20)
                .HasColumnName("cnpj");
            entity.Property(e => e.Complemento)
                .HasMaxLength(95)
                .HasColumnName("complemento");
            entity.Property(e => e.Ddd)
                .HasMaxLength(10)
                .HasColumnName("ddd");
            entity.Property(e => e.Email)
                .HasMaxLength(85)
                .HasColumnName("email");
            entity.Property(e => e.Email2)
                .HasMaxLength(45)
                .HasColumnName("email2");
            entity.Property(e => e.Email3)
                .HasMaxLength(45)
                .HasColumnName("email3");
            entity.Property(e => e.Estado)
                .HasMaxLength(45)
                .HasColumnName("estado");
            entity.Property(e => e.Facebook)
                .HasMaxLength(45)
                .HasColumnName("facebook");
            entity.Property(e => e.Grau)
                .HasMaxLength(25)
                .HasColumnName("grau");
            entity.Property(e => e.Grupo)
                .HasMaxLength(20)
                .HasColumnName("grupo");
            entity.Property(e => e.Instagram)
                .HasMaxLength(45)
                .HasColumnName("instagram");
            entity.Property(e => e.Logradouro)
                .HasMaxLength(145)
                .HasColumnName("logradouro");
            entity.Property(e => e.Municipio)
                .HasMaxLength(145)
                .HasColumnName("municipio");
            entity.Property(e => e.Nome)
                .HasMaxLength(400)
                .HasColumnName("nome");
            entity.Property(e => e.Numero)
                .HasMaxLength(20)
                .HasColumnName("numero");
            entity.Property(e => e.Ramal)
                .HasMaxLength(45)
                .HasColumnName("ramal");
            entity.Property(e => e.Sigla)
                .HasMaxLength(45)
                .HasColumnName("sigla");
            entity.Property(e => e.Status)
                .HasMaxLength(10)
                .HasColumnName("status");
            entity.Property(e => e.Telefone)
                .HasMaxLength(20)
                .HasColumnName("telefone");
            entity.Property(e => e.Twitter)
                .HasMaxLength(45)
                .HasColumnName("twitter");
            entity.Property(e => e.Website)
                .HasMaxLength(45)
                .HasColumnName("website");
        });

        modelBuilder.Entity<Assunto>(entity =>
        {
            entity.HasKey(e => e.Idassunto).HasName("PRIMARY");

            entity
                .ToTable("assunto")
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_general_ci");

            entity.Property(e => e.Idassunto).HasColumnName("idassunto");
            entity.Property(e => e.Assunto1)
                .HasMaxLength(55)
                .HasColumnName("assunto");
        });

        modelBuilder.Entity<Atividade>(entity =>
        {
            entity.HasKey(e => e.IdAtividades).HasName("PRIMARY");

            entity
                .ToTable("atividades")
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_general_ci");

            entity.HasIndex(e => e.TarefasIdTarefas, "tarefas_id_tarefas");

            entity.Property(e => e.IdAtividades).HasColumnName("id_atividades");
            entity.Property(e => e.Alerta)
                .HasMaxLength(45)
                .HasColumnName("alerta");
            entity.Property(e => e.CaminhoDocumentos)
                .HasMaxLength(255)
                .HasColumnName("caminho_documentos");
            entity.Property(e => e.DataAbertura).HasColumnName("data_abertura");
            entity.Property(e => e.DataEvento).HasColumnName("data_evento");
            entity.Property(e => e.DataFinal).HasColumnName("data_final");
            entity.Property(e => e.DataInicial).HasColumnName("data_inicial");
            entity.Property(e => e.NomeAtividade)
                .HasMaxLength(45)
                .HasColumnName("nome_atividade");
            entity.Property(e => e.Recorrencia)
                .HasMaxLength(45)
                .HasColumnName("recorrencia");
            entity.Property(e => e.Status)
                .HasMaxLength(15)
                .HasColumnName("status");
            entity.Property(e => e.StatusEtapa)
                .HasMaxLength(45)
                .HasColumnName("status_etapa");
            entity.Property(e => e.TarefasIdTarefas).HasColumnName("tarefas_id_tarefas");
            entity.Property(e => e.UsuarioAdmIdUser).HasColumnName("usuario_adm_id_user");

            entity.HasOne(d => d.TarefasIdTarefasNavigation).WithMany(p => p.Atividades)
                .HasForeignKey(d => d.TarefasIdTarefas)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("tarefas_id_tarefas");
        });

        modelBuilder.Entity<AtividadesComentario>(entity =>
        {
            entity.HasKey(e => e.IdatividadesComentarios).HasName("PRIMARY");

            entity
                .ToTable("atividades_comentarios")
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_general_ci");

            entity.HasIndex(e => e.AtividadesIdAtividades, "atividades_id_atividades");

            entity.Property(e => e.IdatividadesComentarios).HasColumnName("idatividades_comentarios");
            entity.Property(e => e.AtividadesIdAtividades).HasColumnName("atividades_id_atividades");
            entity.Property(e => e.Comentario)
                .HasColumnType("mediumtext")
                .HasColumnName("comentario");
            entity.Property(e => e.DataComentario).HasColumnName("data_comentario");

            entity.HasOne(d => d.AtividadesIdAtividadesNavigation).WithMany(p => p.AtividadesComentarios)
                .HasForeignKey(d => d.AtividadesIdAtividades)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("atividades_id_atividades");
        });

        modelBuilder.Entity<BdLegadoBuscaRapidum>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("_bd_legado_busca_rapida", tb => tb.HasComment("Tabela contendo informações de Busca Rápida importadas do Banco de dados do Sistema Legado"))
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_general_ci");

            entity.HasIndex(e => e.CodigoInternoBuscaRapida, "codigo_interno_busca_rapida_UNIQUE").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CategoriaSindical)
                .HasMaxLength(45)
                .HasColumnName("categoria_sindical");
            entity.Property(e => e.Cidade)
                .HasMaxLength(145)
                .HasColumnName("cidade");
            entity.Property(e => e.CnpjDoSindicato)
                .HasMaxLength(20)
                .HasColumnName("cnpj_do_sindicato");
            entity.Property(e => e.CodigoInternoBuscaRapida).HasColumnName("codigo_interno_busca_rapida");
            entity.Property(e => e.CodigoInternoSindicato).HasColumnName("codigo_interno_sindicato");
            entity.Property(e => e.DataDeInclusao).HasColumnName("data_de_inclusao");
            entity.Property(e => e.Database)
                .HasMaxLength(10)
                .HasColumnName("database");
            entity.Property(e => e.Documento)
                .HasMaxLength(100)
                .HasColumnName("documento");
            entity.Property(e => e.Estado)
                .HasMaxLength(45)
                .HasColumnName("estado");
            entity.Property(e => e.Grupo)
                .HasMaxLength(100)
                .HasColumnName("grupo");
            entity.Property(e => e.NomeDaBuscaRapida)
                .HasMaxLength(100)
                .HasColumnName("nome_da_busca_rapida");
            entity.Property(e => e.NomeDoSindicato)
                .HasMaxLength(350)
                .HasColumnName("nome_do_sindicato");
            entity.Property(e => e.PeriodoFinal).HasColumnName("periodo_final");
            entity.Property(e => e.PeriodoInicial).HasColumnName("periodo_inicial");
            entity.Property(e => e.TextoBuscaRapida)
                .HasColumnType("mediumtext")
                .HasColumnName("texto_busca_rapida");
        });

        modelBuilder.Entity<BdLegadoClausula>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("_bd_legado_clausulas", tb => tb.HasComment("Tabela contendo informações de Cláusulas importadas do Banco de dados do Sistema Legado"))
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_general_ci");

            entity.HasIndex(e => e.CodigoInternoClausula, "codigo_interno_clausula_UNIQUE").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CategoriaSindical)
                .HasMaxLength(45)
                .HasColumnName("categoria_sindical");
            entity.Property(e => e.Cidade)
                .HasMaxLength(145)
                .HasColumnName("cidade");
            entity.Property(e => e.CnpjDoSindicato)
                .HasMaxLength(20)
                .HasColumnName("cnpj_do_sindicato");
            entity.Property(e => e.CodigoInternoClausula).HasColumnName("codigo_interno_clausula");
            entity.Property(e => e.CodigoInternoSindicato).HasColumnName("codigo_interno_sindicato");
            entity.Property(e => e.DataDeInclusao).HasColumnName("data_de_inclusao");
            entity.Property(e => e.Database)
                .HasMaxLength(10)
                .HasColumnName("database");
            entity.Property(e => e.Documento)
                .HasMaxLength(100)
                .HasColumnName("documento");
            entity.Property(e => e.Estado)
                .HasMaxLength(45)
                .HasColumnName("estado");
            entity.Property(e => e.Grupo)
                .HasMaxLength(100)
                .HasColumnName("grupo");
            entity.Property(e => e.NomeDaClausula)
                .HasMaxLength(100)
                .HasColumnName("nome_da_clausula");
            entity.Property(e => e.NomeDoSindicato)
                .HasMaxLength(350)
                .HasColumnName("nome_do_sindicato");
            entity.Property(e => e.PeriodoFinal).HasColumnName("periodo_final");
            entity.Property(e => e.PeriodoInicial).HasColumnName("periodo_inicial");
            entity.Property(e => e.TextoClausula)
                .HasColumnType("mediumtext")
                .HasColumnName("texto_clausula");
        });

        modelBuilder.Entity<CadastroCliente>(entity =>
        {
            entity.HasKey(e => e.IdcadClientes).HasName("PRIMARY");

            entity
                .ToTable("cadastro_clientes")
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_general_ci");

            entity.Property(e => e.IdcadClientes).HasColumnName("idcad_clientes");
            entity.Property(e => e.CadSuperior)
                .HasMaxLength(45)
                .HasColumnName("cad_superior");
            entity.Property(e => e.EpCodigoempresa)
                .HasMaxLength(25)
                .HasColumnName("ep_codigoempresa");
            entity.Property(e => e.EpNomeempresa)
                .HasMaxLength(125)
                .HasColumnName("ep_nomeempresa");
            entity.Property(e => e.EstBairro)
                .HasMaxLength(85)
                .HasColumnName("est_bairro");
            entity.Property(e => e.EstCep)
                .HasMaxLength(20)
                .HasColumnName("est_cep");
            entity.Property(e => e.EstCnaemp)
                .HasColumnType("json")
                .HasColumnName("est_cnaemp");
            entity.Property(e => e.EstCnpj)
                .HasMaxLength(20)
                .HasColumnName("est_cnpj");
            entity.Property(e => e.EstCodsindlaboral)
                .HasMaxLength(35)
                .HasColumnName("est_codsindlaboral");
            entity.Property(e => e.EstCodsindpatronal)
                .HasMaxLength(35)
                .HasColumnName("est_codsindpatronal");
            entity.Property(e => e.EstCodunidade)
                .HasMaxLength(45)
                .HasColumnName("est_codunidade");
            entity.Property(e => e.EstDatainativacao).HasColumnName("est_datainativacao");
            entity.Property(e => e.EstDatainclusao).HasColumnName("est_datainclusao");
            entity.Property(e => e.EstLocalizacao).HasColumnName("est_localizacao");
            entity.Property(e => e.EstLogradouro)
                .HasMaxLength(155)
                .HasColumnName("est_logradouro");
            entity.Property(e => e.EstNomeunidade)
                .HasMaxLength(75)
                .HasColumnName("est_nomeunidade");
            entity.Property(e => e.EstRegional)
                .HasMaxLength(45)
                .HasColumnName("est_regional");
            entity.Property(e => e.EstTipounidcliente)
                .HasMaxLength(35)
                .HasColumnName("est_tipounidcliente");
            entity.Property(e => e.GeDatafopag)
                .HasMaxLength(4)
                .HasColumnName("ge_datafopag");
            entity.Property(e => e.GeSlaentrega)
                .HasMaxLength(2)
                .HasColumnName("ge_slaentrega");
            entity.Property(e => e.GeSlaprioridade)
                .HasMaxLength(25)
                .HasColumnName("ge_slaprioridade");
            entity.Property(e => e.GeTipodocumento)
                .HasMaxLength(15)
                .HasColumnName("ge_tipodocumento");
            entity.Property(e => e.GeTipoprocessamento)
                .HasMaxLength(30)
                .HasColumnName("ge_tipoprocessamento");
            entity.Property(e => e.LogoGrupo).HasColumnName("logo_grupo");
            entity.Property(e => e.Modulos)
                .HasColumnType("json")
                .HasColumnName("modulos");
            entity.Property(e => e.NomeCliente)
                .HasColumnType("text")
                .HasColumnName("nome_cliente");
            entity.Property(e => e.TipoCliente)
                .HasMaxLength(20)
                .HasColumnName("tipo_cliente");
        });

        modelBuilder.Entity<CalendarioGeralNovo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("calendario_geral_novo")
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_general_ci");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AcompanhamentoCctId).HasColumnName("acompanhamento_cct_id");
            entity.Property(e => e.ClausulaGeralIdClau).HasColumnName("clausula_geral_id_clau");
            entity.Property(e => e.DocSindIdDoc).HasColumnName("doc_sind_id_doc");
            entity.Property(e => e.SindDirempId).HasColumnName("sind_diremp_id");
            entity.Property(e => e.SindDirpatroId).HasColumnName("sind_dirpatro_id");
        });

        modelBuilder.Entity<CentralSindical>(entity =>
        {
            entity.HasKey(e => e.IdCentralsindical).HasName("PRIMARY");

            entity
                .ToTable("central_sindical")
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_general_ci");

            entity.Property(e => e.IdCentralsindical).HasColumnName("id_centralsindical");
            entity.Property(e => e.Bairro)
                .HasMaxLength(45)
                .HasColumnName("bairro");
            entity.Property(e => e.Cep)
                .HasMaxLength(20)
                .HasColumnName("cep");
            entity.Property(e => e.Cnpj)
                .HasMaxLength(20)
                .HasColumnName("cnpj");
            entity.Property(e => e.Complemento)
                .HasMaxLength(45)
                .HasColumnName("complemento");
            entity.Property(e => e.Ddd)
                .HasMaxLength(20)
                .HasColumnName("ddd");
            entity.Property(e => e.Email)
                .HasMaxLength(45)
                .HasColumnName("email");
            entity.Property(e => e.Email2)
                .HasMaxLength(45)
                .HasColumnName("email2");
            entity.Property(e => e.Email3)
                .HasMaxLength(45)
                .HasColumnName("email3");
            entity.Property(e => e.Estado)
                .HasMaxLength(45)
                .HasColumnName("estado");
            entity.Property(e => e.Facebook)
                .HasMaxLength(45)
                .HasColumnName("facebook");
            entity.Property(e => e.Instagram)
                .HasMaxLength(45)
                .HasColumnName("instagram");
            entity.Property(e => e.Logradouro)
                .HasMaxLength(45)
                .HasColumnName("logradouro");
            entity.Property(e => e.Municipio)
                .HasMaxLength(45)
                .HasColumnName("municipio");
            entity.Property(e => e.NomeCentralsindical)
                .HasMaxLength(95)
                .HasColumnName("nome_centralsindical");
            entity.Property(e => e.Numero)
                .HasMaxLength(20)
                .HasColumnName("numero");
            entity.Property(e => e.Sigla)
                .HasMaxLength(20)
                .HasColumnName("sigla");
            entity.Property(e => e.Status)
                .HasMaxLength(45)
                .HasColumnName("status");
            entity.Property(e => e.Telefone)
                .HasMaxLength(20)
                .HasColumnName("telefone");
            entity.Property(e => e.Twitter)
                .HasMaxLength(45)
                .HasColumnName("twitter");
            entity.Property(e => e.Website)
                .HasMaxLength(45)
                .HasColumnName("website");
        });

        modelBuilder.Entity<ClasseCnaeDocSind>(entity =>
        {
            entity.HasKey(e => e.IdcnaeDoc).HasName("PRIMARY");

            entity
                .ToTable("classe_cnae_doc_sind")
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_general_ci");

            entity.Property(e => e.IdcnaeDoc).HasColumnName("idcnae_doc");
            entity.Property(e => e.ClasseCnaeIdCnae).HasColumnName("classe_cnae_id_cnae");
            entity.Property(e => e.DocSindIdDoc).HasColumnName("doc_sind_id_doc");
        });

        modelBuilder.Entity<CnaeEmp>(entity =>
        {
            entity.HasKey(e => e.IdcnaeEmp).HasName("PRIMARY");

            entity
                .ToTable("cnae_emp")
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_general_ci");

            entity.HasIndex(e => e.ClasseCnaeIdclasseCnae, "classe_cnae_idclasse_cnae");

            entity.HasIndex(e => e.ClienteUnidadesIdUnidade, "cliente_unidades_id_unidade");

            entity.Property(e => e.IdcnaeEmp).HasColumnName("idcnae_emp");
            entity.Property(e => e.ClasseCnaeIdclasseCnae).HasColumnName("classe_cnae_idclasse_cnae");
            entity.Property(e => e.ClienteUnidadesIdUnidade).HasColumnName("cliente_unidades_id_unidade");
            entity.Property(e => e.DataFinal).HasColumnName("data_final");
            entity.Property(e => e.DataInicial).HasColumnName("data_inicial");

            entity.HasOne<Cnae>()
                .WithMany()
                .HasForeignKey(d => d.ClasseCnaeIdclasseCnae)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("cnae_emp_ibfk_1");

            entity.HasOne(d => d.ClienteUnidadesIdUnidadeNavigation)
                  .WithMany()
                .HasForeignKey(d => d.ClienteUnidadesIdUnidade)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("cnae_emp_ibfk_2");
        });

        modelBuilder.Entity<DadosSdf>(entity =>
        {
            entity.HasKey(e => e.IdFeriados).HasName("PRIMARY");

            entity
                .ToTable("dados_sdf")
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_general_ci");

            entity.HasIndex(e => e.DocumentosIddocumentos, "documentos_iddocumentos_ibfk3_idx");

            entity.HasIndex(e => e.FeriadosIdFeriado, "feriados_id_feriados_ibfk1");

            entity.HasIndex(e => e.LocalizacaoIdLocalizacao, "localizacao_id_localizaco_ibfk2");

            entity.Property(e => e.IdFeriados).HasColumnName("id_feriados");
            entity.Property(e => e.DocumentosIddocumentos).HasColumnName("documentos_iddocumentos");
            entity.Property(e => e.FeriadosIdFeriado).HasColumnName("feriados_id_feriado");
            entity.Property(e => e.LocalizacaoIdLocalizacao).HasColumnName("localizacao_id_localizacao");

            entity.HasOne(d => d.DocumentosIddocumentosNavigation).WithMany(p => p.DadosSdfs)
                .HasForeignKey(d => d.DocumentosIddocumentos)
                .HasConstraintName("documentos_iddocumentos_ibfk3");

            entity.HasOne(d => d.FeriadosIdFeriadoNavigation).WithMany(p => p.DadosSdfs)
                .HasForeignKey(d => d.FeriadosIdFeriado)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("feriados_id_feriado_ibfk1");

            entity.HasOne(d => d.LocalizacaoIdLocalizacaoNavigation).WithMany()
                .HasForeignKey(d => d.LocalizacaoIdLocalizacao)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("localizacao_id_localizacao_ibfk2");
        });

        modelBuilder.Entity<DiretoriaSindLaboral>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("diretoria_sind_laboral");

            entity.Property(e => e.ClienteMatrizIdEmpresa).HasColumnName("cliente_matriz_id_empresa");
            entity.Property(e => e.ClienteUnidadesIdUnidade).HasColumnName("cliente_unidades_id_unidade");
            entity.Property(e => e.DirigenteE)
                .HasMaxLength(145)
                .HasColumnName("dirigente_e")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.FuncaoE)
                .HasMaxLength(45)
                .HasColumnName("funcao_e")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.IdDiretoriae).HasColumnName("id_diretoriae");
            entity.Property(e => e.InicioMandatoe).HasColumnName("inicio_mandatoe");
            entity.Property(e => e.SindEmpIdSinde).HasColumnName("sind_emp_id_sinde");
            entity.Property(e => e.SituacaoE)
                .HasMaxLength(45)
                .HasColumnName("situacao_e")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.TerminoMandatoe).HasColumnName("termino_mandatoe");
        });

        modelBuilder.Entity<DocSindClienteUnidade>(entity =>
        {
            entity.HasKey(e => e.IdDocsindClienteunidades).HasName("PRIMARY");

            entity.ToTable("doc_sind_cliente_unidades");

            entity.HasIndex(e => e.DocSindIdDoc, "doc_sind_cliente_unidades_ibfk3");

            entity.HasIndex(e => e.ClienteUnidadesIdUnidade, "id_cliente_unidades_ibfk_2");

            entity.Property(e => e.IdDocsindClienteunidades).HasColumnName("id_docsind_clienteunidades");
            entity.Property(e => e.ClienteUnidadesIdUnidade).HasColumnName("cliente_unidades_id_unidade");
            entity.Property(e => e.DocSindIdDoc).HasColumnName("doc_sind_id_doc");

            entity.HasOne(d => d.ClienteUnidadesIdUnidadeNavigation)
                .WithMany()
                .HasForeignKey(d => d.ClienteUnidadesIdUnidade)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("id_cliente_unidades_ibfk_2");

            entity.HasOne<DocumentoSindical>(d => d.DocSindIdDocNavigation).WithMany()
                .HasForeignKey(d => d.DocSindIdDoc)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("doc_sind_cliente_unidades_ibfk3");
        });

        modelBuilder.Entity<DocSindReferencium>(entity =>
        {
            entity.HasKey(e => e.IddocSindReferencia).HasName("PRIMARY");

            entity
                .ToTable("doc_sind_referencia")
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_general_ci");

            entity.Property(e => e.IddocSindReferencia).HasColumnName("iddoc_sind_referencia");
            entity.Property(e => e.DocSindIdDoc).HasColumnName("doc_sind_id_doc");
            entity.Property(e => e.EstruturaClausulaIdEstruturaclausula).HasColumnName("estrutura_clausula_id_estruturaclausula");
        });

        modelBuilder.Entity<DocSindSindEmp>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("doc_sind_sind_emp");

            entity.HasIndex(e => e.SindEmpIdSinde, "doc_sind_emp_ibfk_1");

            entity.HasIndex(e => e.DocSindIdDoc, "doc_sind_id_ibfk_2");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DocSindIdDoc).HasColumnName("doc_sind_id_doc");
            entity.Property(e => e.SindEmpIdSinde).HasColumnName("sind_emp_id_sinde");

            entity.HasOne<DocumentoSindical>(d => d.DocSindIdDocNavigation).WithMany()
                .HasForeignKey(d => d.DocSindIdDoc)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("doc_sind_id_ibfk_2");

            entity.HasOne(d => d.SindEmpIdSindeNavigation).WithMany()
                .HasForeignKey(d => d.SindEmpIdSinde)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("doc_sind_emp_ibfk_1");
        });

        modelBuilder.Entity<DocSindSindPatr>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("doc_sind_sind_patr");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DocSindIdDoc).HasColumnName("doc_sind_id_doc");
            entity.Property(e => e.SindPatrIdSindp).HasColumnName("sind_patr_id_sindp");
        });

        modelBuilder.Entity<Documento>(entity =>
        {
            entity.HasKey(e => e.Iddocumentos).HasName("PRIMARY");

            entity.ToTable("documentos");

            entity.HasIndex(e => e.TipoDocIdtipoDoc, "tipo_doc_idtipo_doc_idx");

            entity.Property(e => e.Iddocumentos).HasColumnName("iddocumentos");
            entity.Property(e => e.Anuencia)
                .HasMaxLength(3)
                .HasColumnName("anuencia");
            entity.Property(e => e.CaminhoArquivo)
                .HasMaxLength(255)
                .HasColumnName("caminho_arquivo");
            entity.Property(e => e.ComentarioLegislacao)
                .HasColumnType("mediumtext")
                .HasColumnName("comentario_legislacao");
            entity.Property(e => e.DataUpload)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("data_upload");
            entity.Property(e => e.DescricaoDocumento)
                .HasMaxLength(65)
                .HasColumnName("descricao_documento");
            entity.Property(e => e.DocumentoRestrito)
                .HasMaxLength(3)
                .HasColumnName("documento_restrito");
            entity.Property(e => e.FonteWeb)
                .HasMaxLength(65)
                .HasColumnName("fonte_web");
            entity.Property(e => e.NumeroLei)
                .HasMaxLength(45)
                .HasColumnName("numero_lei");
            entity.Property(e => e.OrigemDocumento)
                .HasMaxLength(45)
                .HasColumnName("origem_documento");
            entity.Property(e => e.SindEmpIdSinde).HasColumnName("sind_emp_id_sinde");
            entity.Property(e => e.SindPatrIdSindp).HasColumnName("sind_patr_id_sindp");
            entity.Property(e => e.Status)
                .HasMaxLength(15)
                .HasColumnName("status");
            entity.Property(e => e.TipoDocIdtipoDoc).HasColumnName("tipo_doc_idtipo_doc");
            entity.Property(e => e.UsuarioAdmIdUser).HasColumnName("usuario_adm_id_user");
            entity.Property(e => e.VigenciaFinal).HasColumnName("vigencia_final");
            entity.Property(e => e.VigenciaInicial).HasColumnName("vigencia_inicial");

            entity.HasOne<TipoDocumento>().WithMany()
                .HasForeignKey(d => d.TipoDocIdtipoDoc)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ibfk_tipo");
        });

        modelBuilder.Entity<DocumentoAssunto>(entity =>
        {
            entity.HasKey(e => e.IddocumentoAssunto).HasName("PRIMARY");

            entity
                .ToTable("documento_assunto")
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_general_ci");

            entity.HasIndex(e => e.DocumentosIddocumentos, "ibfk_doc_abrang_idx");

            entity.HasIndex(e => e.EstruturaClausulaIdEstruturaclausula, "ibfk_doc_assunto_idx");

            entity.Property(e => e.IddocumentoAssunto).HasColumnName("iddocumento_assunto");
            entity.Property(e => e.DocumentosIddocumentos).HasColumnName("documentos_iddocumentos");
            entity.Property(e => e.EstruturaClausulaIdEstruturaclausula).HasColumnName("estrutura_clausula_id_estruturaclausula");

            entity.HasOne(d => d.DocumentosIddocumentosNavigation).WithMany(p => p.DocumentoAssuntos)
                .HasForeignKey(d => d.DocumentosIddocumentos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ibfk_doc_abrang");
        });

        modelBuilder.Entity<DocumentosAbrangencium>(entity =>
        {
            entity.HasKey(e => e.IdAbrangDoc).HasName("PRIMARY");

            entity
                .ToTable("documentos_abrangencia")
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_general_ci");

            entity.HasIndex(e => e.DocumentosIddocumentos, "ibfk_doc_idx");

            entity.HasIndex(e => e.LocalizacaoIdLocalizacao, "ibfk_local_abrang_idx");

            entity.Property(e => e.IdAbrangDoc).HasColumnName("id_abrang_doc");
            entity.Property(e => e.DocumentosIddocumentos).HasColumnName("documentos_iddocumentos");
            entity.Property(e => e.LocalizacaoIdLocalizacao).HasColumnName("localizacao_id_localizacao");

            entity.HasOne(d => d.DocumentosIddocumentosNavigation).WithMany(p => p.DocumentosAbrangencia)
                .HasForeignKey(d => d.DocumentosIddocumentos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ibfk_doc");

            entity.HasOne(d => d.LocalizacaoIdLocalizacaoNavigation).WithMany()
                .HasForeignKey(d => d.LocalizacaoIdLocalizacao)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ibfk_local_abrang");
        });

        modelBuilder.Entity<DocumentosCnae>(entity =>
        {
            entity.HasKey(e => e.IddocumentosCnae).HasName("PRIMARY");

            entity
                .ToTable("documentos_cnae")
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_general_ci");

            entity.Property(e => e.IddocumentosCnae).HasColumnName("iddocumentos_cnae");
            entity.Property(e => e.ClasseCnaeIdCnae).HasColumnName("classe_cnae_id_cnae");
            entity.Property(e => e.DocumentosIddocumentos).HasColumnName("documentos_iddocumentos");
        });

        modelBuilder.Entity<DocumentosEmpresa>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("documentos_empresa")
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_general_ci");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ClienteUnidadesIdUnidade)
                .HasMaxLength(45)
                .HasColumnName("cliente_unidades_id_unidade");
            entity.Property(e => e.DocumentosIdDocumento)
                .HasMaxLength(45)
                .HasColumnName("documentos_id_documento");
        });

        modelBuilder.Entity<EstruturaClausulasAdTipoinformacaoadicional>(entity =>
        {
            entity.HasKey(e => e.IdEstruturaTagsClausulasAdTipoinformacaoadicional).HasName("PRIMARY");

            entity.ToTable("estrutura_clausulas_ad_tipoinformacaoadicional");

            entity.HasIndex(e => e.AdTipoinformacaoadicionalCdtipoinformacaoadicional, "cdtipoinformacaoadicional");

            entity.HasIndex(e => e.EstruturaClausulaIdEstruturaclausula, "fkid_estrutura_tags_clausulas");

            entity.Property(e => e.IdEstruturaTagsClausulasAdTipoinformacaoadicional).HasColumnName("id_estrutura_tags_clausulas_ad_tipoinformacaoadicional");
            entity.Property(e => e.AdTipoinformacaoadicionalCdtipoinformacaoadicional).HasColumnName("ad_tipoinformacaoadicional_cdtipoinformacaoadicional");
            entity.Property(e => e.EstruturaClausulaIdEstruturaclausula).HasColumnName("estrutura_clausula_id_estruturaclausula");

            entity.HasOne(d => d.AdTipoinformacaoadicionalCdtipoinformacaoadicionalNavigation).WithMany(p => p.EstruturaClausulasAdTipoinformacaoadicionals)
                .HasForeignKey(d => d.AdTipoinformacaoadicionalCdtipoinformacaoadicional)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("estrutura_clausulas_ad_tipoinformacaoadicional_ibfk_2");
        });

        modelBuilder.Entity<Feriado>(entity =>
        {
            entity.HasKey(e => e.IdFeriado).HasName("PRIMARY");

            entity.ToTable("feriados");

            entity.Property(e => e.IdFeriado).HasColumnName("id_feriado");
            entity.Property(e => e.AbrangFeriado)
                .HasMaxLength(55)
                .HasColumnName("abrang_feriado")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.Dia)
                .HasMaxLength(2)
                .HasColumnName("dia")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.Localizacao).HasColumnName("localizacao");
            entity.Property(e => e.Mes)
                .HasMaxLength(2)
                .HasColumnName("mes")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.NomeFeriado)
                .HasMaxLength(155)
                .HasColumnName("nome_feriado")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.Regra)
                .HasMaxLength(45)
                .HasColumnName("regra")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
        });

        modelBuilder.Entity<FiltroCsv>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("filtro_csv");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Filtro)
                .HasColumnType("text")
                .HasColumnName("filtro");
            entity.Property(e => e.Usuario)
                .HasMaxLength(45)
                .HasColumnName("usuario");
        });

        modelBuilder.Entity<FormularioFolha>(entity =>
        {
            entity.HasKey(e => e.IdformularioFolha).HasName("PRIMARY");

            entity
                .ToTable("formulario_folha")
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_general_ci");

            entity.Property(e => e.IdformularioFolha).HasColumnName("idformulario_folha");
            entity.Property(e => e.AtividadeEconomica).HasColumnName("atividade_economica");
            entity.Property(e => e.Caminho)
                .HasMaxLength(45)
                .HasColumnName("caminho");
            entity.Property(e => e.CodSindcliente)
                .HasMaxLength(35)
                .HasColumnName("cod_sindcliente");
            entity.Property(e => e.DataAprovacao).HasColumnName("data_aprovacao");
            entity.Property(e => e.DataBase)
                .HasMaxLength(10)
                .HasColumnName("data_base");
            entity.Property(e => e.DocSind).HasColumnName("doc_sind");
            entity.Property(e => e.Formulario)
                .HasMaxLength(10)
                .HasColumnName("formulario");
            entity.Property(e => e.NomeArqExcell)
                .HasMaxLength(45)
                .HasColumnName("nome_arq_excell");
            entity.Property(e => e.NomeDocumento).HasColumnName("nome_documento");
            entity.Property(e => e.SiglaLaboral).HasColumnName("sigla_laboral");
            entity.Property(e => e.SiglaPatronal).HasColumnName("sigla_patronal");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasColumnName("status");
            entity.Property(e => e.UltimaAtualizacao).HasColumnName("ultima_atualizacao");
            entity.Property(e => e.Usuario).HasColumnName("usuario");
            entity.Property(e => e.ValidadeFinal).HasColumnName("validade_final");
        });

        modelBuilder.Entity<FormularioGrupo>(entity =>
        {
            entity.HasKey(e => e.IdFormulariogrupo).HasName("PRIMARY");

            entity.ToTable("formulario_grupo");

            entity.HasIndex(e => new { e.ClienteGrupoIdGrupoEconomico, e.DocSindIdDoc }, "idx_duplicate_check").IsUnique();

            entity.Property(e => e.IdFormulariogrupo).HasColumnName("id_formulariogrupo");
            entity.Property(e => e.ClienteGrupoIdGrupoEconomico).HasColumnName("cliente_grupo_id_grupo_economico");
            entity.Property(e => e.DocSindIdDoc).HasColumnName("doc_sind_id_doc");
            entity.Property(e => e.FormularioComunicado)
                .HasColumnType("json")
                .HasColumnName("formulario_comunicado");
        });

        modelBuilder.Entity<Helpdesk>(entity =>
        {
            entity.HasKey(e => e.Idhelpdesk).HasName("PRIMARY");

            entity
                .ToTable("helpdesk")
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_general_ci");

            entity.Property(e => e.Idhelpdesk).HasColumnName("idhelpdesk");
            entity.Property(e => e.CaminhoArquivo)
                .HasMaxLength(255)
                .HasColumnName("caminho_arquivo");
            entity.Property(e => e.Clausula).HasColumnName("clausula");
            entity.Property(e => e.ComentarioChamado)
                .HasColumnType("json")
                .HasColumnName("comentario_chamado");
            entity.Property(e => e.DataAbertura)
                .HasColumnType("timestamp(6)")
                .HasColumnName("data_abertura");
            entity.Property(e => e.DataFechado)
                .HasColumnType("timestamp(6)")
                .HasColumnName("data_fechado");
            entity.Property(e => e.DataVencimento).HasColumnName("data_vencimento");
            entity.Property(e => e.Estabelecimento).HasColumnName("estabelecimento");
            entity.Property(e => e.IdUserC).HasColumnName("id_userC");
            entity.Property(e => e.IdUserR).HasColumnName("id_userR");
            entity.Property(e => e.InicioResposta)
                .HasColumnType("timestamp(6)")
                .HasColumnName("inicio_resposta");
            entity.Property(e => e.SindLaboral).HasColumnName("sind_laboral");
            entity.Property(e => e.SindPatronal).HasColumnName("sind_patronal");
            entity.Property(e => e.StatusChamado)
                .HasMaxLength(15)
                .HasColumnName("status_chamado");
            entity.Property(e => e.TipoChamado)
                .HasMaxLength(115)
                .HasColumnName("tipo_chamado");
        });

        modelBuilder.Entity<Indecon>(entity =>
        {
            entity.HasKey(e => e.IdIndecon).HasName("PRIMARY");

            entity
                .ToTable("indecon")
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_general_ci");

            entity.Property(e => e.IdIndecon).HasColumnName("id_indecon");
            entity.Property(e => e.ClienteGrupoIdGrupoEconomico).HasColumnName("cliente_grupo_id_grupo_economico");
            entity.Property(e => e.CriadoEm)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("criado_em");
            entity.Property(e => e.DadoProjetado).HasColumnName("dado_projetado");
            entity.Property(e => e.DadoReal).HasColumnName("dado_real");
            entity.Property(e => e.Data)
                .HasMaxLength(45)
                .HasColumnName("data");
            entity.Property(e => e.Fonte)
                .HasMaxLength(45)
                .HasColumnName("fonte");
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.Indicador)
                .HasMaxLength(5)
                .HasColumnName("indicador");
            entity.Property(e => e.Origem)
                .HasMaxLength(45)
                .HasColumnName("origem");
        });

        modelBuilder.Entity<IndeconReal>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("indecon_real");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CriadoEm)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("criado_em");
            entity.Property(e => e.DadoReal).HasColumnName("dado_real");
            entity.Property(e => e.Indicador)
                .HasMaxLength(45)
                .HasColumnName("indicador");
            entity.Property(e => e.PeriodoData).HasColumnName("periodo_data");
        });

        modelBuilder.Entity<InformacaoAdicionalCombo>(entity =>
        {
            entity.HasKey(e => e.IdCombo).HasName("PRIMARY");

            entity.ToTable("informacao_adicional_combo");

            entity.HasIndex(e => e.AdTipoinformacaoadicionalId, "ad_tipoinfo_id");

            entity.Property(e => e.IdCombo).HasColumnName("id_combo");
            entity.Property(e => e.AdTipoinformacaoadicionalId).HasColumnName("ad_tipoinformacaoadicional_id");
            entity.Property(e => e.Options)
                .HasMaxLength(5000)
                .HasColumnName("options");

            entity.HasOne(d => d.AdTipoinformacaoadicional).WithMany(p => p.InformacaoAdicionalCombos)
                .HasForeignKey(d => d.AdTipoinformacaoadicionalId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ad_tipoinfo_id");
        });

        modelBuilder.Entity<InformacaoAdicionalGrupo>(entity =>
        {
            entity.HasKey(e => e.IdGrupo).HasName("PRIMARY");

            entity.ToTable("informacao_adicional_grupo");

            entity.HasIndex(e => e.InformacaoadicionalNoGrupo, "info_adic_no_grupo");

            entity.HasIndex(e => e.AdTipoinformacaoadicionalId, "info_adic_tipo_grupo");

            entity.Property(e => e.IdGrupo).HasColumnName("id_grupo");
            entity.Property(e => e.AdTipoinformacaoadicionalId).HasColumnName("ad_tipoinformacaoadicional_id");
            entity.Property(e => e.InformacaoadicionalNoGrupo).HasColumnName("informacaoadicional_no_grupo");
            entity.Property(e => e.Sequencia).HasColumnName("sequencia");

            entity.HasOne(d => d.AdTipoinformacaoadicional).WithMany(p => p.InformacaoAdicionalGrupoAdTipoinformacaoadicionals)
                .HasForeignKey(d => d.AdTipoinformacaoadicionalId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("info_adic_tipo_grupo");

            entity.HasOne(d => d.InformacaoadicionalNoGrupoNavigation).WithMany(p => p.InformacaoAdicionalGrupoInformacaoadicionalNoGrupoNavigations)
                .HasForeignKey(d => d.InformacaoadicionalNoGrupo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("info_adic_no_grupo");

            entity.Property(e => e.ExibeComparativoMapaSindical)
                .HasColumnName("exibe_comparativo_mapa_sindical")
                .HasColumnType("TINYINT(0)");
        });

        modelBuilder.Entity<InformacoesAdicionai>(entity =>
        {
            entity.HasKey(e => e.IdinformacoesAdicionais).HasName("PRIMARY");

            entity
                .ToTable("informacoes_adicionais")
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_general_ci");

            entity.Property(e => e.IdinformacoesAdicionais).HasColumnName("idinformacoes_adicionais");
            entity.Property(e => e.Categoria)
                .HasMaxLength(45)
                .HasColumnName("categoria");
            entity.Property(e => e.ClausulaGeral)
                .HasMaxLength(45)
                .HasColumnName("clausula_geral");
            entity.Property(e => e.CnpjSindlaboral)
                .HasMaxLength(45)
                .HasColumnName("cnpj_sindlaboral");
            entity.Property(e => e.CnpjSindpatronal)
                .HasMaxLength(45)
                .HasColumnName("cnpj_sindpatronal");
            entity.Property(e => e.DataAprovacao)
                .HasMaxLength(45)
                .HasColumnName("data_aprovacao");
            entity.Property(e => e.DataBase)
                .HasMaxLength(45)
                .HasColumnName("data_base");
            entity.Property(e => e.DocSindical)
                .HasMaxLength(45)
                .HasColumnName("doc_sindical");
            entity.Property(e => e.GrupoClausula)
                .HasMaxLength(45)
                .HasColumnName("grupo_clausula");
            entity.Property(e => e.NomeClausula)
                .HasMaxLength(45)
                .HasColumnName("nome_clausula");
            entity.Property(e => e.NomeDocumento)
                .HasMaxLength(45)
                .HasColumnName("nome_documento");
            entity.Property(e => e.SindLaboral)
                .HasMaxLength(45)
                .HasColumnName("sind_laboral");
            entity.Property(e => e.SindPatronal)
                .HasMaxLength(45)
                .HasColumnName("sind_patronal");
            entity.Property(e => e.UfSindlaboral)
                .HasMaxLength(45)
                .HasColumnName("uf_sindlaboral");
            entity.Property(e => e.ValidadeFinal)
                .HasMaxLength(45)
                .HasColumnName("validade_final");
            entity.Property(e => e.ValidadeInicial)
                .HasMaxLength(45)
                .HasColumnName("validade_inicial");
        });

        modelBuilder.Entity<Jfase>(entity =>
        {
            entity.HasKey(e => e.IdJfase).HasName("PRIMARY");

            entity
                .ToTable("jfase")
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_general_ci");

            entity.Property(e => e.IdJfase).HasColumnName("id_jfase");
            entity.Property(e => e.Fase)
                .HasColumnType("json")
                .HasColumnName("fase");
        });

        modelBuilder.Entity<JfaseTipoPerguntas>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("jfase_tipodados_perguntas_legendas")
                .HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_unicode_ci");

            entity.Property(e => e.Id).HasColumnName("idjfase_tipodados_legendas");
            entity.Property(e => e.TipoId).HasColumnName("id_tipodado");
            entity.Property(e => e.Descricao).HasColumnName("descricao_tipodado");
        });

        modelBuilder.Entity<NegociacaoCalculo>(entity =>
        {
            entity.HasKey(e => e.IdnegociacaoCalculadora).HasName("PRIMARY");

            entity
                .ToTable("negociacao_calculo")
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_general_ci");

            entity.Property(e => e.IdnegociacaoCalculadora).HasColumnName("idnegociacao_calculadora");
            entity.Property(e => e.AdTipoinformacaoadicionalCdtipoinformacaoadicional).HasColumnName("ad_tipoinformacaoadicional_cdtipoinformacaoadicional");
            entity.Property(e => e.Combo)
                .HasMaxLength(45)
                .HasColumnName("combo");
            entity.Property(e => e.EstruturaClausulaIdEstruturaclausula).HasColumnName("estrutura_clausula_id_estruturaclausula");
            entity.Property(e => e.IdInfoTipoGrupo).HasColumnName("id_info_tipo_grupo");
            entity.Property(e => e.Numerico).HasColumnName("numerico");
            entity.Property(e => e.Percentual).HasColumnName("percentual");
            entity.Property(e => e.Sequencia).HasColumnName("sequencia");
        });

        modelBuilder.Entity<NegociacaoCenario>(entity =>
        {
            entity.HasKey(e => e.IdnegociacaoPremissas).HasName("PRIMARY");

            entity
                .ToTable("negociacao_cenarios")
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_general_ci");

            entity.Property(e => e.IdnegociacaoPremissas).HasColumnName("idnegociacao_premissas");
            entity.Property(e => e.AdTipoinformacaoadicionalCdtipoinformacaoadicional).HasColumnName("ad_tipoinformacaoadicional_cdtipoinformacaoadicional");
            entity.Property(e => e.Combo)
                .HasMaxLength(45)
                .HasColumnName("combo");
            entity.Property(e => e.EstruturaClausulaIdEstruturaclausula).HasColumnName("estrutura_clausula_id_estruturaclausula");
            entity.Property(e => e.IdInfoTipoGrupo).HasColumnName("id_info_tipo_grupo");
            entity.Property(e => e.NegociacaoIdnegociacao).HasColumnName("negociacao_idnegociacao");
            entity.Property(e => e.Numerico).HasColumnName("numerico");
            entity.Property(e => e.Percentual).HasColumnName("percentual");
            entity.Property(e => e.Sequencia).HasColumnName("sequencia");
        });

        modelBuilder.Entity<NegociacaoPautum>(entity =>
        {
            entity.HasKey(e => e.IdnegociacaoPauta).HasName("PRIMARY");

            entity
                .ToTable("negociacao_pauta")
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_general_ci");

            entity.Property(e => e.IdnegociacaoPauta).HasColumnName("idnegociacao_pauta");
            entity.Property(e => e.DataHora)
                .HasColumnType("datetime")
                .HasColumnName("data_hora");
            entity.Property(e => e.NegociacaoIdnegociacao).HasColumnName("negociacao_idnegociacao");
            entity.Property(e => e.Pauta).HasColumnName("pauta");
        });

        modelBuilder.Entity<NegociacaoPremissa>(entity =>
        {
            entity.HasKey(e => e.IdnegociacaoPremissa).HasName("PRIMARY");

            entity
                .ToTable("negociacao_premissas")
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_general_ci");

            entity.Property(e => e.IdnegociacaoPremissa).HasColumnName("idnegociacao_premissa");
            entity.Property(e => e.Aproveitamento)
                .HasMaxLength(45)
                .HasColumnName("aproveitamento");
            entity.Property(e => e.Atual)
                .HasMaxLength(45)
                .HasColumnName("atual");
            entity.Property(e => e.Comentários)
                .HasMaxLength(45)
                .HasColumnName("comentários");
            entity.Property(e => e.Idnegociacao).HasColumnName("idnegociacao");
            entity.Property(e => e.Objetivo)
                .HasMaxLength(45)
                .HasColumnName("objetivo");
            entity.Property(e => e.Premissa)
                .HasMaxLength(45)
                .HasColumnName("premissa");
            entity.Property(e => e.Resultado)
                .HasMaxLength(45)
                .HasColumnName("resultado");
            entity.Property(e => e.TipoPremissa)
                .HasMaxLength(45)
                .HasColumnName("tipo_premissa");
        });

        modelBuilder.Entity<NegociacaoRodadum>(entity =>
        {
            entity.HasKey(e => e.IdnegociacaoRodada).HasName("PRIMARY");

            entity
                .ToTable("negociacao_rodada")
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_general_ci");

            entity.Property(e => e.IdnegociacaoRodada).HasColumnName("idnegociacao_rodada");
            entity.Property(e => e.Aproveitamento)
                .HasMaxLength(45)
                .HasColumnName("aproveitamento");
            entity.Property(e => e.DataRodada).HasColumnName("data_rodada");
            entity.Property(e => e.FaseCctIdFase).HasColumnName("fase_cct_id_fase");
            entity.Property(e => e.NegociacaoIdnegociacao).HasColumnName("negociacao_idnegociacao");
            entity.Property(e => e.NumeroRodada)
                .HasMaxLength(4)
                .HasColumnName("numero_rodada");
        });

        modelBuilder.Entity<NegociacaoScript>(entity =>
        {
            entity.HasKey(e => e.IdnegociacaoScript).HasName("PRIMARY");

            entity
                .ToTable("negociacao_script")
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_general_ci");

            entity.Property(e => e.IdnegociacaoScript).HasColumnName("idnegociacao_script");
            entity.Property(e => e.DataScript).HasColumnName("data_script");
            entity.Property(e => e.NegociacaoIdnegociacao).HasColumnName("negociacao_idnegociacao");
            entity.Property(e => e.TextoScript)
                .HasColumnType("json")
                .HasColumnName("texto_script");
        });

        modelBuilder.Entity<Ninfoadicionai>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("ninfoadicionais")
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_general_ci");

            entity.Property(e => e.AdTipoinformacaoadicionalCdtipoinformacaoadicional).HasColumnName("ad_tipoinformacaoadicional_cdtipoinformacaoadicional");
            entity.Property(e => e.ClausulaGeralIdClau).HasColumnName("clausula_geral_id_clau");
            entity.Property(e => e.Combo)
                .HasColumnType("text")
                .HasColumnName("combo");
            entity.Property(e => e.Data)
                .HasColumnType("text")
                .HasColumnName("data");
            entity.Property(e => e.Descricao)
                .HasColumnType("text")
                .HasColumnName("descricao");
            entity.Property(e => e.DocSindIdDoc).HasColumnName("doc_sind_id_doc");
            entity.Property(e => e.EstruturaClausulaIdEstruturaclausula).HasColumnName("estrutura_clausula_id_estruturaclausula");
            entity.Property(e => e.GrupoDados).HasColumnName("grupo_dados");
            entity.Property(e => e.Hora)
                .HasColumnType("text")
                .HasColumnName("hora");
            entity.Property(e => e.IdClausulageralEstruturaClausula).HasColumnName("id_clausulageral_estrutura_clausula");
            entity.Property(e => e.IdInfoTipoGrupo).HasColumnName("id_info_tipo_grupo");
            entity.Property(e => e.NomeInformacao).HasColumnName("nome_informacao");
            entity.Property(e => e.Numerico)
                .HasColumnType("text")
                .HasColumnName("numerico");
            entity.Property(e => e.Percentual)
                .HasColumnType("text")
                .HasColumnName("percentual");
            entity.Property(e => e.Sequencia).HasColumnName("sequencia");
            entity.Property(e => e.Texto)
                .HasColumnType("text")
                .HasColumnName("texto");
        });

        modelBuilder.Entity<RestritoUsuario>(entity =>
        {
            entity.HasKey(e => e.IdrestritoUsuario).HasName("PRIMARY");

            entity
                .ToTable("restrito_usuario")
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_general_ci");

            entity.Property(e => e.IdrestritoUsuario).HasColumnName("idrestrito_usuario");
            entity.Property(e => e.DocumentosIddocumentos).HasColumnName("documentos_iddocumentos");
            entity.Property(e => e.UsuarioAdmIdUser).HasColumnName("usuario_adm_id_user");
        });

        modelBuilder.Entity<SindDiremp>(entity =>
        {
            entity.HasKey(e => e.IdDiretoriae).HasName("PRIMARY");

            entity
                .ToTable("sind_diremp")
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_general_ci");

            entity.HasIndex(e => e.SindEmpIdSinde, "fk_sind_diremp_sind_emp1");

            entity.Property(e => e.IdDiretoriae).HasColumnName("id_diretoriae");
            entity.Property(e => e.ClienteMatrizIdEmpresa).HasColumnName("cliente_matriz_id_empresa");
            entity.Property(e => e.ClienteUnidadesIdUnidade).HasColumnName("cliente_unidades_id_unidade");
            entity.Property(e => e.DirigenteE)
                .HasMaxLength(145)
                .HasColumnName("dirigente_e");
            entity.Property(e => e.FuncaoE)
                .HasMaxLength(45)
                .HasColumnName("funcao_e");
            entity.Property(e => e.InicioMandatoe).HasColumnName("inicio_mandatoe");
            entity.Property(e => e.SindEmpIdSinde).HasColumnName("sind_emp_id_sinde");
            entity.Property(e => e.SituacaoE)
                .HasMaxLength(45)
                .HasColumnName("situacao_e");
            entity.Property(e => e.TerminoMandatoe).HasColumnName("termino_mandatoe");

            entity.HasOne(d => d.SindEmpIdSindeNavigation).WithMany()
                .HasForeignKey(d => d.SindEmpIdSinde)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("sind_diremp_ibfk_1");
        });

        modelBuilder.Entity<Tarefa>(entity =>
        {
            entity.HasKey(e => e.IdTarefas).HasName("PRIMARY");

            entity
                .ToTable("tarefas")
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_general_ci");

            entity.Property(e => e.IdTarefas).HasColumnName("id_tarefas");
            entity.Property(e => e.Assunto).HasColumnName("assunto");
            entity.Property(e => e.CalendarGeralIdcalendarGeral).HasColumnName("calendar_geral_idcalendar_geral");
            entity.Property(e => e.DataAbertura).HasColumnName("data_abertura");
            entity.Property(e => e.DataEvento).HasColumnName("data_evento");
            entity.Property(e => e.DataFinal).HasColumnName("data_final");
            entity.Property(e => e.DataInicial).HasColumnName("data_inicial");
            entity.Property(e => e.NomeTarefa)
                .HasMaxLength(45)
                .HasColumnName("nome_tarefa");
            entity.Property(e => e.StatusTarefa)
                .HasMaxLength(15)
                .HasColumnName("status_tarefa");
            entity.Property(e => e.UsuarioAdmIdUser).HasColumnName("usuario_adm_id_user");
        });

        modelBuilder.Entity<TemporarioClausulageral>(entity =>
        {
            entity.HasKey(e => e.IdtemporarioClausulageral).HasName("PRIMARY");

            entity
                .ToTable("temporario_clausulageral")
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_general_ci");

            entity.Property(e => e.IdtemporarioClausulageral).HasColumnName("idtemporario_clausulageral");
            entity.Property(e => e.DocSindIdDocumento).HasColumnName("doc_sind_id_documento");
            entity.Property(e => e.EstruturaIdEstruturaclausula).HasColumnName("estrutura_id_estruturaclausula");
            entity.Property(e => e.TexClau).HasColumnName("tex_clau");
        });

        modelBuilder.Entity<TipounidadeCliente>(entity =>
        {
            entity.HasKey(e => e.IdTiponegocio).HasName("PRIMARY");

            entity
                .ToTable("tipounidade_cliente")
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_general_ci");

            entity.Property(e => e.IdTiponegocio).HasColumnName("id_tiponegocio");
            entity.Property(e => e.TipoNegocio)
                .HasMaxLength(25)
                .HasColumnName("tipo_negocio");
        });

        modelBuilder.Entity<WhCalendario>(entity =>
        {
            entity.HasKey(e => e.IdcalendarGeral).HasName("PRIMARY");

            entity
                .ToTable("wh_calendario")
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_general_ci");

            entity.Property(e => e.IdcalendarGeral).HasColumnName("idcalendar_geral");
            entity.Property(e => e.Abrangencia)
                .HasColumnType("json")
                .HasColumnName("abrangencia");
            entity.Property(e => e.AdTipoinformacaoadicionalCdtipoinformacaoadicional).HasColumnName("ad_tipoinformacaoadicional_cdtipoinformacaoadicional");
            entity.Property(e => e.Comentario)
                .HasColumnType("mediumtext")
                .HasColumnName("comentario");
            entity.Property(e => e.Dados)
                .HasColumnType("json")
                .HasColumnName("dados");
            entity.Property(e => e.DataFinal).HasColumnName("data_final");
            entity.Property(e => e.DataInicial).HasColumnName("data_inicial");
            entity.Property(e => e.HoraEvento)
                .HasColumnType("time")
                .HasColumnName("hora_evento");
            entity.Property(e => e.IdClausula).HasColumnName("id_clausula");
            entity.Property(e => e.IdCnaes)
                .HasColumnType("json")
                .HasColumnName("id_cnaes");
            entity.Property(e => e.IdSinde)
                .HasColumnType("json")
                .HasColumnName("id_sinde");
            entity.Property(e => e.IdSindp)
                .HasColumnType("json")
                .HasColumnName("id_sindp");
            entity.Property(e => e.NomeClausula)
                .HasMaxLength(80)
                .HasColumnName("nome_clausula");
            entity.Property(e => e.NomeEvento)
                .HasMaxLength(125)
                .HasColumnName("nome_evento");
            entity.Property(e => e.NomeGrupoclausula)
                .HasMaxLength(80)
                .HasColumnName("nome_grupoclausula");
            entity.Property(e => e.Origem)
                .HasMaxLength(45)
                .HasColumnName("origem");
            entity.Property(e => e.UsuarioAdmIdUser).HasColumnName("usuario_adm_id_user");
        });

        modelBuilder.Entity<DocumentosSindicatosMaisRecentesUsuarios>(entity =>
        {
            entity.HasNoKey();

            entity.ToTable("documento_sindicato_mais_recente_usuario_tb");

            entity.Property(e => e.SindicatoLaboralId).HasColumnName("sindicato_laboral_id");

            entity.Property(e => e.AnoMesValidadeInicial).HasColumnName("ano_mes_validade_inicial");

            entity.Property(e => e.AnoMesValidadeFinal).HasColumnName("ano_mes_validade_final");

            entity.Property(e => e.DocumentoSindicalId).HasColumnName("documento_sindical_id");

            entity.Property(e => e.RowNum).HasColumnName("row_num");

            entity.Property(e => e.UsuarioId).HasColumnName("usuario_id");

            entity.Property(e => e.SindicatoPatronalId).HasColumnName("sindicato_patronal_id");

            entity.Property(e => e.Id).HasColumnName("id");

            entity.HasKey(e => e.Id).HasName("PRIMARY");
        });

        MapAudittable(modelBuilder);
    }

    internal static void MapAudittable(ModelBuilder modelBuilder)
    {
        if (!(modelBuilder?.Model?.GetEntityTypes()?.Any()) ?? false)
        {
            return;
        }

#pragma warning disable S3267 // Loops should be simplified with "LINQ" expressions
        foreach (var entityType in modelBuilder!.Model!.GetEntityTypes())
        {
            if (typeof(IAuditable).IsAssignableFrom(entityType.ClrType))
            {
                modelBuilder.Entity(entityType.ClrType)
                    .Property<DateTime?>("DataInclusao")
                    .HasColumnName("data_inclusao")
                    .HasColumnType("DATETIME");

                modelBuilder.Entity(entityType.ClrType)
                    .Property<DateTime?>("DataAlteracao")
                    .HasColumnName("data_alteracao")
                    .HasColumnType("DATETIME");

                modelBuilder.Entity(entityType.ClrType)
                    .Property<int?>("UsuarioInclusaoId")
                    .HasColumnName("usuario_inclusao_id");

                modelBuilder.Entity(entityType.ClrType)
                    .Property<int?>("UsuarioAlteracaoId")
                    .HasColumnName("usuario_alteracao_id");
            }
        }
#pragma warning restore S3267 // Loops should be simplified with "LINQ" expressions
    }

    protected void OnBeforeSaveChanges(int userId)
    {
        ChangeTracker.DetectChanges();

        var auditEntries = new List<AuditEntry>();

        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.Entity is AuditTb || entry.State == EntityState.Detached || entry.State == EntityState.Unchanged || !(entry.Entity is IAuditable))
                continue;

            var auditEntry = new AuditEntry(entry)
            {
                TableName = entry.Entity.GetType().Name,
                UserId = userId.ToString(CultureInfo.InvariantCulture)
            };

            auditEntries.Add(auditEntry);
            foreach (var property in entry.Properties)
            {
                string propertyName = property.Metadata.Name;
                if (property.Metadata.IsPrimaryKey())
                {
                    auditEntry.KeyValues[propertyName] = property.CurrentValue ?? new object();
                    continue;
                }

                if ((JsonConvert.SerializeObject(property.CurrentValue ?? "") == JsonConvert.SerializeObject(property.OriginalValue ?? "") ||
                    property.CurrentValue == property.OriginalValue) && entry.State != EntityState.Added && entry.State != EntityState.Deleted
                ) continue;

                switch (entry.State)
                {
                    case EntityState.Added:
                        auditEntry.AuditType = AuditType.Create;
                        auditEntry.NewValues[propertyName] = property.CurrentValue ?? new object();
                        break;
                    case EntityState.Deleted:
                        auditEntry.AuditType = AuditType.Delete;
                        auditEntry.OldValues[propertyName] = property.OriginalValue ?? new object();
                        break;
                    case EntityState.Modified:
                        if (property.IsModified)
                        {
                            auditEntry.ChangedColumns.Add(propertyName);
                            auditEntry.AuditType = AuditType.Update;
                            auditEntry.OldValues[propertyName] = property.OriginalValue ?? new object();
                            auditEntry.NewValues[propertyName] = property.CurrentValue ?? new object();
                        }
                        break;
                }
            }
        }

        foreach (var auditEntry in auditEntries)
        {
            AuditTb.Add(auditEntry.ToAudit());
        }
    }
}
