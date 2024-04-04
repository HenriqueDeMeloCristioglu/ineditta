using CSharpFunctionalExtensions;

using MediatR;

namespace Ineditta.Application.Emails.StoragesManagers.UseCases.Incluir
{
    public class IncluirEmailStorageManagerRequest : IRequest<Result>
    {
        public string From { get; set; } = null!;
        public string To { get; set; } = null!;
        public string MessageId { get; set; } = null!;
        public string Assunto { get; set; } = null!;
        public bool? Enviado { get; set; }
        public string RequestData { get; set; } = null!;
    }
}
