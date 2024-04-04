using Ineditta.Application.Documentos.AtividadesEconomicas.UseCases.IncluirCnaesDocmentos;
using Ineditta.BuildingBlocks.Core.HostedServices;

namespace Ineditta.API.HostedServices
{
    public class IncluirCnaesDocumentosHostedService : RequestServiceBase<IncluirCnaesDocumentosRequest>
    {
        public IncluirCnaesDocumentosHostedService(IServiceProvider serviceProvider, ILogger<HostedServiceBase> logger) : base(serviceProvider, logger, "0 0/30  * * * *")
        {
        }
    }
}
