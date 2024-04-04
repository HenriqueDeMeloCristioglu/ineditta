using Ineditta.Integration.Email.Protocols;
using Ineditta.Integration.Email.Providers.Aws.Filters;
using Ineditta.Integration.Email.Providers.Aws.Services;

using Microsoft.Extensions.DependencyInjection;

namespace Ineditta.Integration.Email.Configurations
{
    public static class DependencyInjectionExtension
    {
        public static IServiceCollection ConfigureAmazonSES(this IServiceCollection services)
        {
            services.AddScoped<IEmailSender, AwsEmailSender>();

            services.AddTransient<AwsValidationFilter>();

            return services;
        }
    }
}
