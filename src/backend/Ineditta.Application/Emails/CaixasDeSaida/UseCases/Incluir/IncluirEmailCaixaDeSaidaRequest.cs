using CSharpFunctionalExtensions;

using MediatR;

namespace Ineditta.Application.Emails.CaixasDeSaida.UseCases.Incluir
{
    public class IncluirEmailCaixaDeSaidaRequest : IRequest<Result>
    {
        public string Email { get; set; } = null!;
        public string Assunto { get; set; } = null!;
        public string Template { get; set; } = null!;
        public string MessageId { get; set; } = null!;
    }
}
