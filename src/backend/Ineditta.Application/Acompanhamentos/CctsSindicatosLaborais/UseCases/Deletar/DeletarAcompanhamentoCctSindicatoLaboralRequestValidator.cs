using FluentValidation;

namespace Ineditta.Application.Acompanhamentos.CctsSindicatosLaborais.UseCases.Deletar
{
    public class DeletarAcompanhamentoCctSindicatoLaboralRequestValidator : AbstractValidator<DeletarAcompanhamentoCctSindicatoLaboralRequest>
    {
        public DeletarAcompanhamentoCctSindicatoLaboralRequestValidator()
        {
            RuleFor(a => a.AcompanhamentoCctSinditoLaboral)
                .NotEmpty()
                .WithMessage("O Sindito Laboral não pode ser nulo");
        }
    }
}
