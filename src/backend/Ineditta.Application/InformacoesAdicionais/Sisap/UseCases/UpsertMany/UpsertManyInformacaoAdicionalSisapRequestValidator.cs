using FluentValidation;

namespace Ineditta.Application.InformacoesAdicionais.Sisap.UseCases.UpsertMany
{
    public class UpsertManyInformacaoAdicionalSisapRequestValidator : AbstractValidator<UpsertManyInformacaoAdicionalSisapRequest>
    {
        public UpsertManyInformacaoAdicionalSisapRequestValidator()
        {
            RuleFor(i => i.InformacoesAdicionais)
                .NotEmpty()
                .WithMessage("As Informacoes Adicionais não pode ser nulo");
        }
    }
}