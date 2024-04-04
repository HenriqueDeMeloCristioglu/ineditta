using FluentValidation;

using Ineditta.Application.InformacoesAdicionais.Cliente.Aprovar;

namespace Ineditta.Application.InformacoesAdicionais.Cliente.UseCases.Aprovar
{
    public class AprovarInformacaoAdicionalClienteValidator : AbstractValidator<AprovarInformacaoAdicionalClienteRequest>
    {
        public AprovarInformacaoAdicionalClienteValidator()
        {
            RuleFor(p => p.DocumentoSindicalId)
                .NotEmpty()
                .WithMessage("Informe o documento sindical");
        }
    }
}
