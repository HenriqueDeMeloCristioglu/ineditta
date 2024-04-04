using FluentValidation;

namespace Ineditta.Application.Clausulas.Gerais.UseCases.Upsert
{
    public class UpsertClausulaRequestValidator : AbstractValidator<UpsertClausulaRequest>
    {
        public UpsertClausulaRequestValidator()
        {
            RuleFor(e => e.EstruturaClausulaId)
                .NotEmpty()
                .WithMessage("Id da Estrutura de cláusula não pode ser nulo");

            RuleFor(e => e.DocumentoSindicalId)
                .NotEmpty()
                .WithMessage("Id do Documento não pode ser nulo");
        }
    }
}
