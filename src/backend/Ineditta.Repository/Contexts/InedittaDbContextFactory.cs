using System.Reflection;

using Ineditta.BuildingBlocks.Core.Auth;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Ineditta.Repository.Contexts
{
    public class InedittaDbContextFactory : IDesignTimeDbContextFactory<InedittaDbContext>
    {
        private readonly string _connectionString;

        public InedittaDbContextFactory()
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? string.Empty;

            // Build config
            var configuration = new ConfigurationBuilder()
             .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../Ineditta.API"))
             .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).AddJsonFile($"appsettings.{environment}.json", optional: true).Build();

            _connectionString = configuration.GetConnectionString("InedittaConnection") ?? string.Empty;
        }

        public InedittaDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<InedittaDbContext>();

            var migrationsAssembly = typeof(InedittaDbContext).GetTypeInfo().Assembly.GetName().Name;

            var serverVersion = new MySqlServerVersion(new Version(8, 0, 31));

            builder.UseMySql(_connectionString, serverVersion, options =>
            {
                options
                .UseMicrosoftJson(MySqlCommonJsonChangeTrackingOptions.FullHierarchyOptimizedFast)
                .MaxBatchSize(1000)
                .EnableRetryOnFailure()
                .MigrationsAssembly(migrationsAssembly)
                .CommandTimeout(120);
            });

            var userInfoService = new UserInfoService(default!);

            return new InedittaDbContext(builder.Options, userInfoService);
        }
    }
}
