using FluentValidation;

namespace Ineditta.Application.Cnaes.UseCases.Upsert
{
    public class UpsertCnaeRequestValidator : AbstractValidator<UpsertCnaeRequest>
    {
        public UpsertCnaeRequestValidator()
        {
            RuleFor(p => p.Divisao)
                .NotEmpty()
                .WithMessage("Informe a Divisão");

            RuleFor(p => p.DescricaoDivisao)
                .MaximumLength(500)
                .WithMessage("Descrição da divisão deve ter no máximo 500 caracteres");

            RuleFor(p => p.SubClasse)
                .NotEmpty()
                .WithMessage("Informe a Sub Classe");

            RuleFor(p => p.DescricaoSubClasse)
                .MaximumLength(500)
                .WithMessage("Descrição da Sub Classe deve ter no máximo 500 caracteres");

            RuleFor(p => p.Categoria)
                .NotEmpty()
                .WithMessage("Informe uma categoria")
                .MaximumLength(200)
                .WithMessage("Categoria deve ter no máximo 200 caracteres");
        }
    }
}
