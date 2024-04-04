using Ineditta.BuildingBlocks.Core.Idempotency.Web;

namespace Ineditta.Application.Clausulas.Gerais.UseCases.EnviarEmailClausulasAprovadas
{
    public class EnviarEmailClausulasAprovadasRequest : IdempotentRequest
    {
        public long DocumentoId { get; set; }
        public IEnumerable<int>? UsuariosIds { get; set; }
    }
}
