using FluentValidation;

namespace Ineditta.Application.Usuarios.UseCases.EnviarEmailBoasVindas
{
    public class EnviarEmailBoasVindasRequestValidator:AbstractValidator<EnviarEmailBoasVindasRequest>
    {
        public EnviarEmailBoasVindasRequestValidator()
        {
            RuleFor(p => p.Email)
                .NotEmpty()
                .WithMessage("Informe o email")
                .EmailAddress()
                .WithMessage("Email inválido");
        }
    }
}
