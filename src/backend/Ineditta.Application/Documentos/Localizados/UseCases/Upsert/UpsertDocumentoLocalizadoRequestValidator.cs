using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FluentValidation;

namespace Ineditta.Application.Documentos.Localizados.UseCases.Upsert
{
    public class UpsertDocumentoLocalizadoRequestValidator : AbstractValidator<UpsertDocumentoLocalizadoRequest>
    {
        public UpsertDocumentoLocalizadoRequestValidator()
        {
            RuleFor(p => p.IdLegado)
                .GreaterThan(0)
                .WithMessage("O idLegado fornecido deve ser maior que 0");

            RuleFor(p => p.Uf)
                .Length(2)
                .WithMessage("A UF deve ser fornecida no formato de dois caracteres");
        }
    }
}
