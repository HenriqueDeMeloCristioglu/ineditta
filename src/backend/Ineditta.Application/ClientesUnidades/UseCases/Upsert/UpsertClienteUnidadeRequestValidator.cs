using FluentValidation;

namespace Ineditta.Application.ClientesUnidades.UseCases.Upsert
{
    public class UpsertClienteUnidadeRequestValidator : AbstractValidator<UpsertClienteUnidadeRequest>
    {
        public UpsertClienteUnidadeRequestValidator()
        {
            RuleFor(p => p.Codigo)
                .NotEmpty()
                .WithMessage("Informe o código")
                .MaximumLength(45)
                .WithMessage("O código deve ter no máximo 45 caracteres");

            RuleFor(p => p.Nome)
                .NotEmpty()
                .WithMessage("Informe o nome")
                .MaximumLength(45)
                .WithMessage("O nome deve ter no máximo 45 caracteres");

            RuleFor(p => p.Cnpj)
                .NotEmpty()
                .WithMessage("Informe o cnpj")
                .MaximumLength(20)
                .WithMessage("O cnpj deve ter no máximo 20 caracteres");

            RuleFor(p => p.Cep)
                .NotEmpty()
                .WithMessage("Informe o cep")
                .MaximumLength(20)
                .WithMessage("O cep deve ter no máximo 20 caracteres");

            RuleFor(p => p.EmpresaId)
                .NotEmpty()
                .WithMessage("Informe o id da empresa");

            RuleFor(p => p.TipoNegocioId)
                .NotEmpty()
                .WithMessage("Informe o id do tipo de negócio");

            RuleFor(p => p.LocalizacaoId)
                .NotEmpty()
                .WithMessage("Informe o id da localização");
        }
    }
}
