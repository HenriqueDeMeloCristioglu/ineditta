using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FluentValidation;

namespace Ineditta.Application.Documentos.Localizados.UseCases.Aprovar
{
    public class AprovarDocumentoLocalizadoRequestValidator : AbstractValidator<AprovarDocumentoLocalizadoRequest>
    {
        public AprovarDocumentoLocalizadoRequestValidator()
        {
            RuleFor(p => p.Id)
                .GreaterThan(0)
                .WithMessage("O id deve ser maior que 0");
        }
    }
}
