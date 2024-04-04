using Ineditta.BuildingBlocks.Core.Bus;

namespace Ineditta.Application.AIs.DocumentosSindicais.Events.DocumentoSindicalClassificacaoIniciada
{
    public class DocumentoSindicalClassificacaoIniciadaEvent : Message
    {
        public DocumentoSindicalClassificacaoIniciadaEvent(long iAClausulaId)
        {
            IAClausulaId = iAClausulaId;
        }

        public long IAClausulaId { get; set; }
    }
}
