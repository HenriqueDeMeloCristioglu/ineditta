using FluentValidation;

namespace Ineditta.Application.InformacoesAdicionais.Cliente.UseCases.Upsert
{
    public class UpsertInformacaoAdicionalClienteValidator : AbstractValidator<UpsertInformacaoAdicionalClienteRequest>
    {
        public UpsertInformacaoAdicionalClienteValidator()
        {
            RuleFor(p => p.DocumentoSindicalId)
                .NotEmpty()
                .WithMessage("Informe o documento sindical");
        }
    }
}
