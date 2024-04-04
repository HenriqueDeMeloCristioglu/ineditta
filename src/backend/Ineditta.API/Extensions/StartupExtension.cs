using Ineditta.BuildingBlocks.Core.Web.API.Validators;
using Ineditta.BuildingBlocks.Core.Web.OpenAPI.Configurations;
using Ineditta.Repository.Contexts;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using MediatR;
using Ineditta.BuildingBlocks.Core.Web.API.Pipelines;
using System.Reflection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Ineditta.BuildingBlocks.Core.Web.API.Convetions;
using Ineditta.Application.Sindicatos.Patronais.UseCases.Upsert;
using Ineditta.BuildingBlocks.Core.Web.OpenAPI.Filters;
using Microsoft.IdentityModel.Logging;
using Ineditta.Integration.Auth;
using Ineditta.Repository.IndicadoresEconomicos;
using Ineditta.BuildingBlocks.Core.Web.API.Policies;
using Ineditta.Application.Usuarios.Repositories;
using Scrutor;
using System.Text.Json.Serialization;
using System.Text.Json;
using Ineditta.API.Filters;
using Ineditta.API.Services;
using Ineditta.API.Constants;
using Ineditta.API.Converters.JSON;
using Ineditta.API.Configurations;
using Ineditta.BuildingBlocks.Core.Idempotency.Pipelines;
using Ineditta.BuildingBlocks.Core.Idempotency.Database;
using Ineditta.BuildingBlocks.Core.Renderers.Html;
using Ineditta.Application.SharedKernel.Frontend;
using Ineditta.API.Builders.MapasSindicais;
using Ineditta.API.HostedServices;
using Ineditta.BuildingBlocks.Core.FileStorage;
using Ineditta.BuildingBlocks.Core.FileStorage.Adapters.Physical;
using DinkToPdf.Contracts;
using DinkToPdf;
using Wkhtmltopdf.NetCore;
using Microsoft.AspNetCore.DataProtection;
using Ineditta.BuildingBlocks.Core.Web.API.Filters;
using Ineditta.BuildingBlocks.Core.Tokens;
using Ineditta.Application.SharedKernel.GestaoDeChamados;
using Ineditta.API.Builders.FormulariosAplicacoes;
using Ineditta.API.Factories.Clausulas;
using Ineditta.API.Factories.InformacoesAdicionais.Cliente;
using Microsoft.FeatureManagement;
using Ineditta.Application.Usuarios.Factories;
using Ineditta.API.Builders.Sindicatos;
using Ineditta.API.Factories.Sindicatos;
using Ineditta.BuildingBlocks.Core.Bus;
using Ineditta.BuildingBlocks.BackgroundProcessing.Core;
using Ineditta.API.Builders.AcompanhamentosCcts;
using Ineditta.API.Builders.Clausulas;
using Ineditta.Integration.Email.Configurations;
using Microsoft.AspNetCore.Mvc;
using Ineditta.Integration.Email.Protocols;
using Ineditta.Integration.Email.Providers.Mailhog.Services;
using Ineditta.Application.Emails.StoragesManagers.Services.LimparCaixasDeSaida;
using Ineditta.Integration.Email.Providers.Aws.Services;
using Ineditta.Integration.Mte.Configurations;
using Ineditta.Integration.AI.Configurations;
using Ineditta.Application.AIs.Core.Services;
using Ineditta.Integration.AI.Providers.ChatGPT.Services;
using Ineditta.Integration.Mte.Services;
using Ineditta.Integration.Mte.Services.Http;
using Ineditta.Application.Clausulas.Gerais.Services.ResumirClausulas;
using Ineditta.Application.Acompanhamentos.Ccts.Services.AcompanhamentosCctsSindicatosLaboraisServices;
using Ineditta.Application.Acompanhamentos.Ccts.Services.AcompanhamentosCctsLocalizacoesServices;
using Ineditta.Application.Acompanhamentos.Ccts.Services.AcompanhamentosCctsSindicatosPatronaisServices;
using Ineditta.Application.Acompanhamentos.Ccts.Factories;
using Ineditta.Application.Acompanhamentos.Ccts.Services.AcompanhamentosCctsEventos;
using Ineditta.Application.Acompanhamentos.Ccts.Services.AcompanhamentosCctsEventosServices;
using Ineditta.Application.AIs.DocumentosSindicais.Services.Mte;
using Ineditta.Integration.OCR.Configurations;
using Ineditta.Integration.Ocr.Providers.Aws.Services;
using Ineditta.Application.SharedKernel.Ocr.Services;
using Ineditta.Application.Documentos.Sindicais.Events.DocumentoCriado;
using Ineditta.Application.Documentos.Sindicais.Factories;

namespace Ineditta.API.Extensions
{
    internal static class StartupExtension
    {
        internal const string DEFAULT_CORS_POLICY_NAME = "Ineddita";

        internal static IServiceCollection ConfigureHealthCheck(this IServiceCollection services, string? basePath)
        {
            services.AddHealthChecks();

            services.AddHealthChecksUI(options =>
            {
                options.SetEvaluationTimeInSeconds(5);
                options.MaximumHistoryEntriesPerEndpoint(10);

                var uri = string.IsNullOrEmpty(basePath) ? "/health" : "http://localhost/health";

                options.AddHealthCheckEndpoint("API com Health Checks", uri);
            })
            .AddInMemoryStorage();

            return services;
        }

        internal static IServiceCollection ConfigureAPIVersion(this IServiceCollection services)
        {
            return services
                .AddApiVersioning(o =>
                {
                    o.AssumeDefaultVersionWhenUnspecified = true;
                    o.DefaultApiVersion = new ApiVersion(1, 0);
                    o.ReportApiVersions = true;
                })
                .AddVersionedApiExplorer(
                options =>
                {
                    options.GroupNameFormat = "'v'VVV";
                    options.SubstituteApiVersionInUrl = true;
                });
        }

        internal static IServiceCollection ConfigureOpenAPI(this IServiceCollection services)
        {
            return services.AddEndpointsApiExplorer()
                .AddSwaggerGen(c =>
                {
                    c.OperationFilter<SwaggerResponseMimeTypeOperationFilter>();
                    c.CustomSchemaIds(x => x.FullName);
                    c.SchemaFilter<NamespaceSchemaFilter>();
                })
                .ConfigureOptions<ConfigureSwaggerOptions>();
        }

        internal static IServiceCollection ConfigureCors(this IServiceCollection services, IConfiguration configuration)
        {
            var origins = configuration.GetSection("Origins")?.Get<string[]>() ?? Array.Empty<string>();

            services.AddCors(options => options.AddPolicy(name: DEFAULT_CORS_POLICY_NAME,
                                  policy => policy.WithOrigins(origins).WithMethods("PUT", "POST", "PATCH", "DELETE", "GET", "OPTIONS").AllowAnyHeader()));

            return services;
        }

        internal static IServiceCollection ConfigureFluentValidations(this IServiceCollection services)
        {
            return services.AddValidatorsFromAssemblyContaining(typeof(UpsertSindicatoPatronalRequestValidator));
        }

        internal static IServiceCollection ConfigureCaching(this IServiceCollection services)
        {
            return services.AddOutputCache(options =>
                options.AddPolicy(ApiConstant.Policies.ExpireAfter5Minutes,
                    builder =>
                    {
                        builder.Expire(TimeSpan.FromMinutes(5));
                    }));
        }

        internal static IServiceCollection ConfigureMediatr(this IServiceCollection services)
        {
            return services
                    .AddMediatR(cfg =>
                    {
                        cfg.RegisterServicesFromAssembly(typeof(UpsertSindicatoPatronalRequest).Assembly);
                        cfg.AddOpenBehavior(typeof(IdempotentPipelineBehavior<,>));
                    })
                    .AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>));
        }

        internal static IServiceCollection ConfigureIdempotent(this IServiceCollection services)
        {
            return services.AddScoped<IIdempotentRepository>(sp =>
                new IdempotentRepository(sp.GetRequiredService<InedittaDbContext>()));
        }

        internal static IServiceCollection ConfigureDependecyInjection(this IServiceCollection services)
        {
            services.AddScoped<RequestStateValidator>();

            services.Scan(scan => scan
                          .FromAssemblies(typeof(RequestStateValidator).Assembly,
                                          typeof(AuthService).Assembly,
                                          typeof(IndicadorEconomicoRepository).Assembly,
                                          typeof(IUsuarioRepository).Assembly
                                          )// 1. Find the concrete classes
                          .AddClasses()    //    to register
                          .UsingRegistrationStrategy(RegistrationStrategy.Skip)
                          //.AsSelfWithInterfaces()
                          .AsMatchingInterface()
                          .WithScopedLifetime()); // 3. Set the lifetime for the services

            services.Scan(scan => scan
                .FromAssembliesOf(typeof(DocumentoCriadoEventHandler))
                .AddClasses(classes => classes.AssignableTo(typeof(BuildingBlocks.Core.Bus.IRequestHandler<>)))
                .AsImplementedInterfaces()
                .WithScopedLifetime());

            services.AddHttpContextAccessor();

            services.AddScoped<HttpRequestItemService>();
            services.AddScoped<UserInfoActionFilter>();
            services.AddScoped<ValidationTokenFilter>();
            services.AddSingleton<ComparativoMapaSindicalBuilder>();
            services.AddScoped<TemplateExcelConfiguration>();
            services.AddScoped<DocumentoSindicalClausulaFactory>();
            services.AddScoped<DocumentoSindicalClausulaPorIdFactory>();
            services.AddScoped<FormularioApliacacaoBuilder>();
            services.AddScoped<InformacoesAdicionaisClienteFactory>();
            services.AddScoped<GerarLinkArquivoFactory>();
            services.AddScoped<GestaoDeChamadoConfiguration>();
            services.AddScoped<ObterUsuarioLogadoFactory>();
            services.AddScoped<SindicatoBuilder>();
            services.AddScoped<SindicatoLaboralBuilder>();
            services.AddScoped<SindicatoPatronalBuilder>();
            services.AddScoped<SindicatoFactory>();
            services.AddScoped<SindicatoLaboralRelatorioFactory>();
            services.AddScoped<SindicatoPatronalRelatorioFactory>();
            services.AddScoped<SindicatoLaboralFactory>();
            services.AddScoped<SindicatoPatronalFactory>();
            services.AddScoped<RelatorioAcompanhamentoCctBuilder>();
            services.AddScoped<RelatorioClausulaBuilder>();
            services.AddScoped<RelatorioBuscaRapidaPdfBuilder>();
            services.AddScoped<RemoverDaCaixaDeSaidaService>();
            services.AddScoped<RegistarEmailCaixaDeSaidaService>();
            services.AddScoped<IResumirClausulas, ResumirClausulaService>();
            services.AddScoped<IncluirAcompanhamentoCctSindicatoLaboralService>();
            services.AddScoped<IncluirAcompanhamentoCctSindicatoPatronalService>();
            services.AddScoped<IncluirAcompanhamentoCctLocalizacaoService>();
            services.AddScoped<DeletarAcompanhamentoSindicatoLaboralService>();
            services.AddScoped<DeletarAcompanhamentoSindicatoPatronalService>();
            services.AddScoped<DeletarAcompanhamentoCctLocalizacaoService>();
            services.AddScoped<AtualizarAcompanhamentoCctLocalizacaoService>();
            services.AddScoped<AtualizarAcompanhamentoCctSindicatoPatronalService>();
            services.AddScoped<AtualizarAcompanhamentoSindicatoLaboralService>();
            services.AddScoped<AcompanhamentoCctAssuntoFactory>();
            services.AddScoped<AcompanhamentoCctEtiquetaFactory>();
            services.AddScoped<AcompanhamentoCctEventoAssembleiaPatronalService>();
            services.AddScoped<AcompanhamentoCctEventoReuniaoEntreAsPartesService>();

            return services;
        }

        internal static IServiceCollection ConfigureDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("InedittaConnection") ?? string.Empty;

            var migrationsAssembly = typeof(InedittaDbContext).GetTypeInfo().Assembly.GetName().Name;

            var serverVersion = new MySqlServerVersion(new Version(8, 0, 31));

            services.AddDbContext<InedittaDbContext>(
                dbContextOptions => dbContextOptions
                    .UseMySql(connectionString, serverVersion, options =>
                    {
                        options
                        .UseMicrosoftJson(MySqlCommonJsonChangeTrackingOptions.FullHierarchyOptimizedFast)
                        .MaxBatchSize(1000)
                        .EnableRetryOnFailure()
                        .MigrationsAssembly(migrationsAssembly)
                        .CommandTimeout(120);
                    })
                    // The following three options help with debugging, but should
                    // be changed or removed for production.
                    .LogTo(Console.WriteLine, LogLevel.Information)
                    .EnableSensitiveDataLogging()
                    .EnableDetailedErrors()
            );

            return services;
        }

        public static IServiceCollection ConfigureAuthorization(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            var authority = configuration["Keycloack:Authority"]?.ToString();
            var audience = configuration["Keycloack:Audience"]?.ToString();
            var admin = configuration["Keycloack:Admin"]?.ToString();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Authority = authority;
                    options.Audience = audience;
                    options.RequireHttpsMetadata = false;

                    if (webHostEnvironment.IsDevelopment())
                    {
                        IdentityModelEventSource.ShowPII = true;
                    }

                    options.Validate();
                });

            services.AddHttpClient<KeycloakHttpClient>(options => options.BaseAddress = new Uri(admin!))
                .SetHandlerLifetime(TimeSpan.FromMinutes(5))
                .AddPolicyHandler(HttpPolicy.GetRetryPolicy())
                .AddPolicyHandler(HttpPolicy.GetCircuitBreakerPolicy());

            return services;
        }

        public static IServiceCollection ConfigureFileStorage(this IServiceCollection services, IConfiguration configuration)
        {
            return services.Configure<FileStorageConfiguration>(configuration.GetSection("FileStorage"));
        }

        public static IServiceCollection ConfigureGestaoDeChamados(this IServiceCollection services, IConfiguration configuration)
        {
            return services.Configure<GestaoDeChamadoConfiguration>(configuration.GetSection("GestaoDeChamados"));
        }

        public static IServiceCollection ConfigureTemplateExcel(this IServiceCollection services, IConfiguration configuration)
        {
            return services.Configure<TemplateExcelConfiguration>(configuration.GetSection("TemplateExcel"));
        }

        public static IServiceCollection ConfigureControllers(this IServiceCollection services)
        {
            services
                .AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()))
                .AddWkhtmltopdf()
                .AddControllers(config =>
                {
                    config.Conventions.Add(new PluralizeKebabCaseControllerNamesConvention());

                    var policy = new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser()
                        .Build();
                    config.Filters.Add(new AuthorizeFilter(policy));

                })
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;

                options.JsonSerializerOptions.Converters.Add(new NivelEnumConverter());
            });

            return services;
        }

        public static IServiceCollection ConfigureEmail(this IServiceCollection services, IConfiguration configuration, IHostEnvironment hostEnvironment)
        {
            services.Configure<EmailConfiguration>(configuration.GetSection("Email"));

            if (hostEnvironment.IsDevelopment())
            {
                services.AddScoped<MailhogNotificationEmailService>();
                return services.AddScoped<IEmailSender, MailhogEmailSender>();
            }

            return services.ConfigureAmazonSES();
        }

        public static IServiceCollection ConfigureOcr(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<OcrConfiguration>(configuration.GetSection("Ocr"));

            return services.AddScoped<IOcrService, OcrAwsService>();
        }

        public static IServiceCollection ConfigureAI(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<AIConfiguration>(configuration.GetSection("AI"));

            return services.AddScoped<IAIService, ChatGptService>();
        }

        public static IServiceCollection ConfigureMte(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<MteConfiguration>(configuration.GetSection("Mte"));

            services.AddHttpClient<MteHttpClient>()
                    .AddPolicyHandler(HttpPolicy.GetRetryPolicy())
                    .AddPolicyHandler(HttpPolicy.GetCircuitBreakerPolicy());

            services.AddScoped<IMteService, MteService>();

            return services;
        }

        public static void RunMigrations(this IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            using var context = scope.ServiceProvider.GetRequiredService<InedittaDbContext>();
            context.Database.Migrate();
        }

        public static IServiceCollection ConfigureHtmlRenderer(this IServiceCollection services)
        {
            return services.AddSingleton<IHtmlRenderer, HtmlRenderer>(sp => new HtmlRenderer(sp.GetRequiredService<ILogger<HtmlRenderer>>()));
        }

        public static IServiceCollection ConfigureOptions(this IServiceCollection services, IConfiguration configuration)
        {
            return services.Configure<FrontendConfiguration>(configuration.GetSection("Frontend"));
        }

        public static IServiceCollection ConfigureHttp(this IServiceCollection services)
        {
            return services.AddHttpClient();
        }

        internal static IServiceCollection ConfigureHostedServices(this IServiceCollection services)
        {
            services.AddHostedService<GerarCalendarioSindicalLoteHostedService>();
            services.AddHostedService<NotificacaoCalendarioSindicaHostedService>();
            services.AddHostedService<IncluirEstabelecimentosAcompanhamentoHostedService>();
            services.AddHostedService<IncluirCnaesDocumentosHostedService>();

            return services;
        }

        internal static IServiceCollection ConfigurePhysicalFileStorage(this IServiceCollection services)
        {
            services.AddScoped<IFileStorage, PhysicalFileStorage>();

            return services;
        }

        internal static IServiceCollection ConfigureDataProtection(this IServiceCollection services)
        {
            services
                .AddDataProtection()
                .PersistKeysToDbContext<InedittaDbContext>();

            services.AddScoped<ITokenService, TokenService>();

            return services;
        }

        internal static IServiceCollection ConfigureFeatureManagement(this IServiceCollection services)
        {
            services.AddFeatureManagement();

            return services;
        }

        internal static IServiceCollection ConfigureBus(this IServiceCollection services)
        {
            services.AddSingleton<IMessagePublisher, MessagePublisher>();

            services.AddSingleton<IMessageSubscriber, MessageSubscriber>();

            return services;
        }
    }
}