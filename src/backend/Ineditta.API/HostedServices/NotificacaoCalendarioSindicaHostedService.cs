using Ineditta.Application.CalendarioSindicais.Eventos.UseCases.Notificar;
using Ineditta.BuildingBlocks.Core.HostedServices;

namespace Ineditta.API.HostedServices
{
    public class NotificacaoCalendarioSindicaHostedService : RequestServiceBase<NotificarCalendarioSindicalRequest>
    {
        public NotificacaoCalendarioSindicaHostedService(IServiceProvider serviceProvider, ILogger<HostedServiceBase> logger) : base(serviceProvider, logger, "0 0/30  * * * *")
        { 
        }
    }
}
