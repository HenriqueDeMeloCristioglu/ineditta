using Ineditta.Application.CalendarioSindicais.Eventos.UseCases.Gerar;
using Ineditta.BuildingBlocks.Core.HostedServices;

namespace Ineditta.API.HostedServices
{
    public class GerarCalendarioSindicalLoteHostedService : RequestServiceBase<GerarCalendarioSindicalLoteRequest>
    {
        public GerarCalendarioSindicalLoteHostedService(IServiceProvider serviceProvider, ILogger<HostedServiceBase> logger) : base(serviceProvider, logger, "0 0/30 * * * *")
        {
        }
    }
}
