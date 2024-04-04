using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FluentValidation;

namespace Ineditta.Application.TiposDocumentos.UseCases.Upsert
{
    public class UpsertTipoDocumentoRequestValidator : AbstractValidator<UpsertTipoDocumentoRequest>
    {
        public UpsertTipoDocumentoRequestValidator()
        {
            When(p => p.Id is not null, () =>
            {
                RuleFor(p => p.Id)
                    .GreaterThan(0)
                    .WithMessage("O id do tipo do documento fornecido deve ser maior que 0");
            });
        }
    }
}
