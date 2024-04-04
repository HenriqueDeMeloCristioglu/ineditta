using Ineditta.BuildingBlocks.Core.Idempotency.Web;

namespace Ineditta.Application.Usuarios.UseCases.EnviarEmailBoasVindas
{
    public class EnviarEmailBoasVindasRequest: IdempotentRequest
    {
        public string Email { get; set; } = null!;
    }
}
