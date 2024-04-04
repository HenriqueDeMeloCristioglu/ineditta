using Ineditta.BuildingBlocks.Core.Idempotency.Web;

namespace Ineditta.Application.AcompanhamentosCcts.UseCases.EnviarEmail
{
    public class EnviarEmailContatoRequest : IdempotentRequest
    {
        public IEnumerable<string> Emails { get; set; } = null!;
        public string Assunto { get; set; } = null!;
        public string Template { get; set; } = null!;
    }
}
