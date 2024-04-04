using Hangfire;

using Microsoft.Extensions.DependencyInjection;

namespace Ineditta.BuildingBlocks.BackgroundProcessing.Configuration.DependencyInjection
{
    public class HangfireJobActivator : JobActivator
    {
        private readonly IServiceScopeFactory _container;

        public HangfireJobActivator(IServiceScopeFactory container)
        {
            _container = container;
        }

        public override object ActivateJob(Type jobType)
        {
            using var scope = _container.CreateScope();

            return scope.ServiceProvider.GetService(jobType)!;
        }
    }
}
