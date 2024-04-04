using FluentValidation;

namespace Ineditta.Application.Comentarios.UseCases.Upsert
{
    public class UpsertComentarioRequestValidator : AbstractValidator<UpsertComentarioRequest>
    {
        public UpsertComentarioRequestValidator()
        {
            RuleFor(e => e.Tipo)
                .NotNull()
                .WithMessage("O Tipo de comentario não pode ser nulo");

            RuleFor(e => e.ReferenciaId)
                .NotNull()
                .WithMessage("A Referência não pode ser nula")
                .GreaterThan(0)
                .WithMessage("A Referência tem que ser maior que 0");

            RuleFor(e => e.TipoUsuarioDestino)
                .NotNull()
                .WithMessage("O Tipo de Destino do Usuário não pode ser nulo");

            RuleFor(e => e.UsuarioDestionoId)
                .NotNull()
                .WithMessage("O Destino do usuário naão pode ser nulo")
                .GreaterThan(0)
                .WithMessage("O Destino do usuário tem que ser maior que 0");

            RuleFor(e => e.Valor)
                .NotEmpty()
                .WithMessage("O Valor do comentário não pode ser nulo");

            RuleFor(e => e.EtiquetaId)
                .NotNull()
                .WithMessage("A Etiqueta não pode ser nula")
                .GreaterThan(0)
                .WithMessage("A Etiqueta do usuário tem que ser maior que 0");

            RuleFor(e => e.TipoNotificacao)
                .NotNull()
                .WithMessage("O Tipo de Notificação não pode ser nula");

            When((e) => e.TipoNotificacao == Entities.TipoNotificacao.Temporaria, () =>
            {
                RuleFor(e => e.DataValidade)
                .NotNull()
                .WithMessage("O campo Data Validade não pode ser nulo para comentário temporário");
            });
        }
    }
}
