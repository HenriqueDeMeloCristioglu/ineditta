using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FluentValidation;

namespace Ineditta.Application.CalendarioSindicais.Eventos.UseCases.Upsert
{
    public class UpsertEventoRequestValidator : AbstractValidator<UpsertEventoRequest>
    {
        public UpsertEventoRequestValidator()
        {
            RuleFor(p => p.ChaveReferenciaId)
                .NotEmpty()
                .WithMessage("Informe a chave de referência")
                .GreaterThan(0)
                .WithMessage("A chave de referência deve ser maior que 0");

            RuleFor(p => p.TipoEvento)
                .NotEmpty()
                .WithMessage("Informe o tipo de evento");

            RuleFor(p => p.Origem)
                .NotEmpty()
                .WithMessage("Informe a origem do evento (Ineditta ou Cliente)");
        }
    }
}
