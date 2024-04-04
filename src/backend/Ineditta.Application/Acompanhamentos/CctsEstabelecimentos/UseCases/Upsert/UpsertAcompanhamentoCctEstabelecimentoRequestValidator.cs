using FluentValidation;

namespace Ineditta.Application.Acompanhamentos.CctsEstabelecimentos.UseCases.Upsert
{
    public class UpsertAcompanhamentoCctEstabelecimentoRequestValidator : AbstractValidator<UpsertAcompanhamentoCctEstabelecimentoRequest>
    {
        public UpsertAcompanhamentoCctEstabelecimentoRequestValidator()
        {
            RuleFor(a => a.AcompanhamentoCctId)
                .NotEmpty()
                .WithMessage("O Id do acompanhamento cct não pode ser nulo")
                .GreaterThan(0)
                .WithMessage("O Id do acompanhamento cct deve ser maior que 0");

            RuleFor(a => a.GrupoEconomicoId)
                .NotEmpty()
                .WithMessage("O Id do grupo economico não pode ser nulo")
                .GreaterThan(0)
                .WithMessage("O Id do grupo economico deve ser maior que 0");

            RuleFor(a => a.EmpresaId)
                .NotEmpty()
                .WithMessage("O Id da empresa não pode ser nulo")
                .GreaterThan(0)
                .WithMessage("O Id do empresa deve ser maior que 0");

            RuleFor(a => a.EstabelecimentoId)
                .NotEmpty()
                .WithMessage("O Id do estabelecimento não pode ser nulo")
                .GreaterThan(0)
                .WithMessage("O Id do estabelecimento deve ser maior que 0");
        }
    }
}
