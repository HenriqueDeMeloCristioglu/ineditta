using FluentValidation;

namespace Ineditta.Application.Documentos.Sindicais.UseCases.NotificarCriacao
{
    public class NotificarCriacaoRequestValidator : AbstractValidator<NotificarCriacaoRequest>
    {
        public NotificarCriacaoRequestValidator()
        {
            RuleFor(p => p.DocumentoId)
                .NotNull()
                .WithMessage("Você deve fornecer um documento id")
                .GreaterThan(0)
                .WithMessage("O id deve ser maior que 0");

            RuleFor(p => p.UsuariosParaNotificarIds)
                .NotNull()
                .WithMessage("Você deve fornecer a lista de usuários para notificação");

            When(p => p.UsuariosParaNotificarIds is not null, () =>
            {
                RuleFor(p => p.UsuariosParaNotificarIds.Count())
                    .GreaterThan(0)
                    .WithMessage("A lista de usuários não pode estar vazia.");
            });
        }
    }
}
