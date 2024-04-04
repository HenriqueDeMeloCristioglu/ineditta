using FluentValidation;

using Ineditta.Application.Clausulas.Gerais.UseCases.GeraNovasClausulas;

namespace Ineditta.Application.Clausulas.Gerais.UseCases.GeraNovasClausulas
{
    public class GeraNovasClausulasRequestValidator : AbstractValidator<GeraNovasClausulasRequest>
    {
        public GeraNovasClausulasRequestValidator()
        {
            RuleFor(c => c.DocumentoId)
                .NotEmpty()
                .WithMessage("Id do Documento não pode ser nulo")
                .GreaterThan(0)
                .WithMessage("Id do Documento deve ser maior que 0");
        }
    }
}
