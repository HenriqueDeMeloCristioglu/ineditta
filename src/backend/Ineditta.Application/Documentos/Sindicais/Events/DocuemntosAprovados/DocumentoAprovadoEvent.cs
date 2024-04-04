using Ineditta.BuildingBlocks.Core.Bus;

namespace Ineditta.Application.Documentos.Sindicais.Events.DocuemntosAprovados
{
    public class DocumentoAprovadoEvent : Message
    {

        public DocumentoAprovadoEvent(long documentoId, long usuarioId)
        {
            DocumentoId = documentoId;
            UsuarioId = usuarioId;
        }

        public long DocumentoId { get; set; }
        public long UsuarioId { get; set; }
    }
}
