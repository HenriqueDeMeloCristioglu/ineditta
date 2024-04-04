using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FluentValidation;

namespace Ineditta.Application.CalendarioSindicais.Usuarios.UseCases.Upsert
{
    public class UpsertCalendarioSindicalUsuarioRequestValidator : AbstractValidator<UpsertCalendarioSindicalUsuarioRequest>
    {
        public UpsertCalendarioSindicalUsuarioRequestValidator()
        {
            RuleFor(p => p.Titulo)
                .NotEmpty()
                .WithMessage("Você precisa fornecer um título para o evento");

            RuleFor(p => p.DataHora)
                .NotEmpty()
                .WithMessage("Você precisa fornecer a data e hora do evento.")
                .GreaterThan(DateTime.Now)
                .WithMessage("Você não pode criar um evento para o passado.");
        }
    }
}
