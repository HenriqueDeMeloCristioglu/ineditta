using FluentValidation;

namespace Ineditta.Application.Usuarios.UseCases.AtualizacaoCredenciais
{
    public class EnviarEmailAtualizacaoCredenciaisRequestValidator : AbstractValidator<EnviarEmailAtualizacaoCredenciaisRequest>
    {
        public EnviarEmailAtualizacaoCredenciaisRequestValidator()
        {
            RuleFor(p => p.Id)
                .GreaterThan(0)
                .WithMessage("Informe o usuário");
        }
    }
}
