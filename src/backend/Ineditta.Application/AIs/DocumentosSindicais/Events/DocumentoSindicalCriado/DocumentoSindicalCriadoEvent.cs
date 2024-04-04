using Ineditta.BuildingBlocks.Core.Bus;

namespace Ineditta.Application.AIs.DocumentosSindicais.Events.DocumentoSindicalCriado
{
    public class DocumentoSindicalCriadoEvent : Message
    {
        public DocumentoSindicalCriadoEvent(long iADocumentoSindicalId)
        {
            IADocumentoSindicalId = iADocumentoSindicalId;
        }

        public long IADocumentoSindicalId { get; set; }
    }
}
