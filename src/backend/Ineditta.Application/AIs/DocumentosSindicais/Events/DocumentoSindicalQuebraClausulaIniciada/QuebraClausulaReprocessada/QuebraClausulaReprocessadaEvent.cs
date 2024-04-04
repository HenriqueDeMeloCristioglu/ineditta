using Ineditta.BuildingBlocks.Core.Bus;

namespace Ineditta.Application.AIs.DocumentosSindicais.Events.DocumentoSindicalQuebraClausulaIniciada.QuebraClausulaReprocessada
{
    public class QuebraClausulaReprocessadaEvent : Message
    {
        public QuebraClausulaReprocessadaEvent(long iADocumentoSindicalId)
        {
            IADocumentoSindicalId = iADocumentoSindicalId;
        }

        public long IADocumentoSindicalId { get; set; }
    }
}
