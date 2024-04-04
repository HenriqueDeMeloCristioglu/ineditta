using Ineditta.BuildingBlocks.Core.Bus;

namespace Ineditta.Application.Documentos.Sindicais.Events.DocumentoCriado
{
    public class DocumentoCriadoEvent : Message
    {
        public DocumentoCriadoEvent(long documentoSindicalId, long usuarioId)
        {
            DocumentoSindicalId = documentoSindicalId;
            UsuarioId = usuarioId;
        }

        public long DocumentoSindicalId { get; set; }
        public long UsuarioId { get; set; }
    }
}
