using FluentValidation;

namespace Ineditta.Application.Acompanhamentos.CctsSindicatosLaborais.UseCases.Incluir
{
    public class InlcuirAcompanhamentoCctSindicatoLaboralRequestValidator : AbstractValidator<IncluirAcompanhamentoCctSindicatoLaboralRequest>
    {
        public InlcuirAcompanhamentoCctSindicatoLaboralRequestValidator()
        {
            RuleFor(a => a.AcompanhamentoCctId)
                .NotEmpty()
                .WithMessage("O Id do acompanhamento cct não pode ser nulo")
                .GreaterThan(0)
                .WithMessage("O Id do acompanhamento cct deve ser maior que 0");

            RuleFor(a => a.SindicatoId)
                .NotEmpty()
                .WithMessage("O Id do sindicato não pode ser nulo")
                .GreaterThan(0)
                .WithMessage("O Id do sindicato deve ser maior que 0");
        }
    }
}
