using FluentValidation;

namespace Ineditta.Application.SharedKernel.Requests.Logradouros
{
    public class LogradouroRequestValidator : AbstractValidator<LogradouroRequest>
    {
        public LogradouroRequestValidator()
        {
            RuleFor(p => p.Regiao)
                .MaximumLength(500)
                .WithMessage("Informe a região")
                .When(p => !string.IsNullOrEmpty(p.Regiao));

            RuleFor(p => p.Cep)
                .NotEmpty()
                .WithMessage("Informe o cep")
                .MinimumLength(8)
                .WithMessage("CEP deve conter no mínimo 8 caracteres")
                .MaximumLength(9)
                .WithMessage("CEP deve conter no máximo 9 caracteres");

            RuleFor(p => p.Endereco)
                .MaximumLength(1000)
                .WithMessage("Endereço deve ter no máximo 1000 caracteres")
                .When(p => !string.IsNullOrEmpty(p.Endereco));

            RuleFor(p => p.Bairro)
                .NotEmpty()
                .WithMessage("Informe o bairro")
                .MaximumLength(500)
                .WithMessage("Bairro deve ter no máximo 500 caracteres");
        }
    }
}
