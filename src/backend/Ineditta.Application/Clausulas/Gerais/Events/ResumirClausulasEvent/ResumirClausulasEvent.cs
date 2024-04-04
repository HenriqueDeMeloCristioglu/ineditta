using Ineditta.BuildingBlocks.Core.Bus;

namespace Ineditta.Application.Clausulas.Gerais.Events.ResumirClausulasEvent
{
    public class ResumirClausulasEvent : Message
    {
        public int DocumentoId { get; set; }
        public int UsuarioId { get; set; }
    }
}
