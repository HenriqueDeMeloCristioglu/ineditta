using FluentValidation;

namespace Ineditta.Application.Acompanhamentos.CctsSindicatosPatronais.UseCases.Deletar
{
    public class DeletarAcompanhamentoCctSindicatoPatronalRequestValidator : AbstractValidator<DeletarAcompanhamentoCctSindicatoPatronalRequest>
    {
        public DeletarAcompanhamentoCctSindicatoPatronalRequestValidator()
        {
            RuleFor(a => a.AcompanhamentoCctSinditoPatronal)
                .NotEmpty()
                .WithMessage("O Sindito Patronal não pode ser nulo");
        }
    }
}
