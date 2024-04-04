using Ineditta.BuildingBlocks.Core.Bus;

namespace Ineditta.Application.Clausulas.Gerais.Events.ClausulasAprovadasEmails
{
    public class ClausulasAprovadasEmailsEvent : Message
    {
        public ClausulasAprovadasEmailsEvent(long documentoId, long usuarioId)
        {
            DocumentoId = documentoId;
            UsuarioId = usuarioId;
        }

        public long DocumentoId { get; set; }
        public long UsuarioId { get; set; }
    }
}
