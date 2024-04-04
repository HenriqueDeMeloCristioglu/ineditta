using FluentValidation;

namespace Ineditta.Application.Usuarios.UseCases.AtualizarPermissoes
{
    public class AtualizarPermissaoRequestValidator : AbstractValidator<AtualizarPermissaoRequest>
    {
        public AtualizarPermissaoRequestValidator()
        {
            RuleFor(p => p.Id)
                .GreaterThan(0)
                .WithMessage("Informe o usuário");
        }
    }
}
