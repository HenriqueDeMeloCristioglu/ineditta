using Ineditta.Application.Acompanhamentos.Ccts.UseCases.IncluirEstabelecimentos;
using Ineditta.BuildingBlocks.Core.HostedServices;

namespace Ineditta.API.HostedServices
{
    public class IncluirEstabelecimentosAcompanhamentoHostedService : RequestServiceBase<IncluirEstabelecimentosRequest>
    {
        public IncluirEstabelecimentosAcompanhamentoHostedService(IServiceProvider serviceProvider, ILogger<HostedServiceBase> logger) : base(serviceProvider, logger, "0 0/30  * * * *")
        {
        }
    }
}
