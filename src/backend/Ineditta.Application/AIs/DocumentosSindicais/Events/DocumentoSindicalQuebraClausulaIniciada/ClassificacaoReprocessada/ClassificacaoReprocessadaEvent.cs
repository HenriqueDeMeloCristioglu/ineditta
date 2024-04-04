using Ineditta.BuildingBlocks.Core.Bus;

namespace Ineditta.Application.AIs.DocumentosSindicais.Events.DocumentoSindicalQuebraClausulaIniciada.ClassificacaoReprocessada
{
    public class ClassificacaoReprocessadaEvent : Message
    {
        public ClassificacaoReprocessadaEvent(long iADocumentoSindicalId)
        {
            IADocumentoSindicalId = iADocumentoSindicalId;
        }

        public long IADocumentoSindicalId { get; set; }
    }
}
