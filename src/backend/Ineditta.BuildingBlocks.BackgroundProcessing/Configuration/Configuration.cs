using System.Transactions;

using Hangfire;
using Hangfire.MySql;

using Ineditta.BuildingBlocks.BackgroundProcessing.Configuration.DependencyInjection;
using Ineditta.BuildingBlocks.BackgroundProcessing.Core;
using Ineditta.BuildingBlocks.Core.Bus;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ineditta.BuildingBlocks.BackgroundProcessing.Configuration
{
    public static class Configuration
    {
        public static IServiceCollection ConfigureBackgroundProcessing(this IServiceCollection services, string connectionString)
        {
            services.AddHangfire((serviceProvider, config) =>
            {
                config.UseRecommendedSerializerSettings();
                config.UseActivator(new HangfireJobActivator(serviceProvider.GetRequiredService<IServiceScopeFactory>()));
                config.UseStorage(new MySqlStorage(connectionString, new MySqlStorageOptions 
                {
                    TransactionIsolationLevel = IsolationLevel.ReadCommitted,
                    QueuePollInterval = TimeSpan.FromSeconds(15),
                    JobExpirationCheckInterval = TimeSpan.FromHours(1),
                    CountersAggregateInterval = TimeSpan.FromMinutes(5),
                    PrepareSchemaIfNecessary = true,
                    DashboardJobListLimit = 50000,
                    TransactionTimeout = TimeSpan.FromMinutes(1),
                    TablesPrefix = "hangfire"
                }));
            });

            GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute { Attempts = 5, DelaysInSeconds = new int[] { 180 } });

            GlobalJobFilters.Filters.Add(new DisableConcurrentExecutionAttribute(300));

            services.AddHangfireServer();

            return services;
        }

        public static IApplicationBuilder UseBackgroundProcessingDashboard(this IApplicationBuilder app)
        {
            return app.UseHangfireDashboard();
        }
    }
}
