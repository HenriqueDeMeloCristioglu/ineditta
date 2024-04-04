using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Ineditta.BuildingBlocks.Core.HostedServices
{
    public abstract class HostedServiceBase : BackgroundService
    {
#pragma warning disable CA1051 // Não declarar campos de instância visíveis
        protected readonly ILogger<HostedServiceBase> _logger;
        protected readonly IServiceProvider _serviceProvider;
#pragma warning restore CA1051 // Não declarar campos de instância visíveis

        protected HostedServiceBase(IServiceProvider serviceProvider, ILogger<HostedServiceBase> logger)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Worker {Name} iniciado", GetType().Name);
            
            await base.StartAsync(cancellationToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Worker {Name} encerrado", GetType().Name);

            await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);

            await base.StopAsync(cancellationToken);
        }
    }
}
