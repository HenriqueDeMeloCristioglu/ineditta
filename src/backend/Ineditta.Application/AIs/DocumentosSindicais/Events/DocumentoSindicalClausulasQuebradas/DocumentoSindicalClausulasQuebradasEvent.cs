using Ineditta.BuildingBlocks.Core.Bus;

namespace Ineditta.Application.AIs.DocumentosSindicais.Events.DocumentoSindicalClausulasQuebradas
{
    public class DocumentoSindicalClausulasQuebradasEvent : Message
    {
        public DocumentoSindicalClausulasQuebradasEvent(long iAdocumentoSindicalId)
        {
            IADocumentoSindicalId = iAdocumentoSindicalId;
        }

        public long IADocumentoSindicalId { get; set; }
    }
}
