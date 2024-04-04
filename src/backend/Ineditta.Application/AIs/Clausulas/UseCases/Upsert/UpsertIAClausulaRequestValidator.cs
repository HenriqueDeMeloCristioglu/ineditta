using FluentValidation;

using Microsoft.EntityFrameworkCore;

namespace Ineditta.Application.AIs.Clausulas.UseCases.Upsert
{
    public class UpsertIAClausulaRequestValidator : AbstractValidator<UpsertIAClausulaRequest>
    {
        public UpsertIAClausulaRequestValidator()
        {
            RuleFor(x => x.Texto)
                .NotEmpty()
                .WithMessage("Você deve fornecer um texto para a clausula");

            RuleFor(x => x.DocumentoSindicalId)
                .GreaterThan(0)
                .WithMessage("O id documento sindical da clausula deve ser maior que 0");

            RuleFor(x => x.EstruturaClausulaId)
                .GreaterThan(0)
                .WithMessage("O id estrutura da clausula deve ser maior que 0");

            RuleFor(x => x.Numero)
                .GreaterThan(0)
                .WithMessage("O numero da clausula deve ser maior que 0");

            RuleFor(x => x.SinonimoId)
                .GreaterThan(0)
                .WithMessage("O id do sinônimo da cláusula deve ser maior que 0");
        }
    }
}
