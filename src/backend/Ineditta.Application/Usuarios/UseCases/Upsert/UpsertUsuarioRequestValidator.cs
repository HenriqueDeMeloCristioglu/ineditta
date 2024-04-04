using FluentValidation;

namespace Ineditta.Application.Usuarios.UseCases.Upsert
{
    public class UpsertUsuarioRequestValidator : AbstractValidator<UpsertUsuarioRequest>
    {
        public UpsertUsuarioRequestValidator()
        {
            RuleFor(p => p.Nome)
                .NotEmpty()
                .WithMessage("Informe o nome do usuário")
                .MaximumLength(500)
                .WithMessage("O nome do usuário deve ter no máximo 500 caracteres");

            RuleFor(p => p.Username)
                .NotEmpty()
                .WithMessage("Informe o username")
                .MaximumLength(200)
                .WithMessage("O username deve ter no máximo 200 caracteres");

            RuleFor(p => p.Email)
                .NotEmpty()
                .WithMessage("Informe o email")
                .MaximumLength(200)
                .WithMessage("O email deve ter no máximo 200 caracteres")
                .EmailAddress()
                .WithMessage("Email inválido");
        }
    }
}