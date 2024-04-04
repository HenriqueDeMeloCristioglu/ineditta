using FluentValidation;

namespace Ineditta.Application.Clausulas.Clientes.UseCases.Upsert
{
    public class IncluirClausulaClienteRequestValidator : AbstractValidator<IncluirClausulaClienteRequest>
    {
        public IncluirClausulaClienteRequestValidator()
        {
            RuleFor(c => c.ClausulaId)
                .NotEmpty()
                .WithMessage("Id da Cláusula não pode ser nulo")
                .GreaterThan(0)
                .WithMessage("O Id da Cláusula deve ser maior que 0");

            RuleFor(c => c.Texto)
                .NotEmpty()
                .WithMessage("Texto não pode ser nulo");
        }
    }
}
