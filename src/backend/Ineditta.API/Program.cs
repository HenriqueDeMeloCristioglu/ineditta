using HealthChecks.UI.Client;

using Ineditta.API.Extensions;
using Ineditta.BuildingBlocks.BackgroundProcessing.Configuration;
using Ineditta.BuildingBlocks.Core.Web.API.Middlewares;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var pathBase = builder.Configuration["PATH_BASE"];

builder.Services.ConfigureCors(builder.Configuration);
builder.Services.ConfigureCaching();
builder.Services.ConfigureDbContext(builder.Configuration);
builder.Services.ConfigureDataProtection();
builder.Services.ConfigureFileStorage(builder.Configuration);
builder.Services.ConfigureTemplateExcel(builder.Configuration);
builder.Services.ConfigureControllers();
builder.Services.ConfigureAPIVersion();
builder.Services.ConfigureOpenAPI();
builder.Services.ConfigureMediatr();
builder.Services.ConfigureIdempotent();
builder.Services.ConfigureEmail(builder.Configuration, builder.Environment);
builder.Services.ConfigureOptions(builder.Configuration);
builder.Services.ConfigureFluentValidations();
builder.Services.ConfigureHealthCheck(pathBase);
builder.Services.ConfigureAuthorization(builder.Configuration, builder.Environment);
builder.Services.ConfigureHtmlRenderer();
builder.Services.ConfigureHttp();
builder.Services.ConfigureDependecyInjection();
builder.Services.ConfigureFeatureManagement();
builder.Services.ConfigureHostedServices();
builder.Services.ConfigurePhysicalFileStorage();
builder.Services.ConfigureGestaoDeChamados(builder.Configuration);
builder.Services.ConfigureOcr(builder.Configuration);
builder.Services.ConfigureAI(builder.Configuration);
builder.Services.ConfigureMte(builder.Configuration);
builder.Services.ConfigureBackgroundProcessing(builder.Configuration.GetConnectionString("InedittaConnection") ?? string.Empty);
builder.Services.ConfigureBus();

var app = builder.Build();

app.UseMiddleware<EnableRequestBodyBufferingMiddleware>();
app.UseMiddleware<ReadRequestBodyMiddleware>();

if (!string.IsNullOrEmpty(pathBase))
{
    app.UsePathBase(pathBase);

    app.UseForwardedHeaders(new ForwardedHeadersOptions
    {
        ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
    });
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        apiVersionDescriptionProvider?.ApiVersionDescriptions.ToList().ForEach(description =>
        {
            options.SwaggerEndpoint($"{pathBase}/swagger/{description.GroupName}/swagger.json",
                description.GroupName.ToUpperInvariant());
        });
    });
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseBackgroundProcessingDashboard();

app.UseHealthChecks("/health", new HealthCheckOptions
{
    Predicate = p => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.UseHealthChecksUI(config =>
{
    config.ResourcesPath = "/ui/resources";
    config.UIPath = "/dashboard";
});

app.Services.RunMigrations();

app.Run();