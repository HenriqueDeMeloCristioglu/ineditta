using FluentValidation;

namespace Ineditta.Application.Acompanhamentos.CctsLocalizacoes.UseCases.Deletar
{
    public class DeletarAcompanhamentoCctLocalizacaoRequestValidator : AbstractValidator<DeletarAcompanhamentoCctLocalizacaoRequest>
    {
        public DeletarAcompanhamentoCctLocalizacaoRequestValidator()
        {
            RuleFor(a => a.AcompanhamentoCctLocalizacao)
                .NotEmpty()
                .WithMessage("A Localização não pode ser nulla");
        }
    }
}
