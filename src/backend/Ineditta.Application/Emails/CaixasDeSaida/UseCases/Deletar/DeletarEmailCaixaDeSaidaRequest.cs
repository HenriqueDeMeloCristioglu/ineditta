using CSharpFunctionalExtensions;

using Ineditta.Application.Emails.CaixasDeSaida.Entities;

using MediatR;

namespace Ineditta.Application.Emails.CaixasDeSaida.UseCases.Deletar
{
    public class DeletarEmailCaixaDeSaidaRequest : IRequest<Result>
    {
        public EmailCaixaDeSaida EmailCaixaDeSaida { get; set; } = null!;
    }
}
