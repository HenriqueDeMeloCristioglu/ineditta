using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FluentValidation;

namespace Ineditta.Application.NotificacaoEventos.UseCases.Upsert
{
    public class UpsertNotificacaoRequestValidator : AbstractValidator<UpsertNotificacaoRequest>
    {
        public UpsertNotificacaoRequestValidator() 
        {
            RuleFor(p => p.EventoId)
                .NotEmpty()
                .WithMessage("Você deve informar o evento associado")
                .GreaterThan(0)
                .WithMessage("O id do evento associado deve ser maior que 0");

            RuleFor(p => p.Notificados)
                .NotEmpty()
                .WithMessage("Você deve informar a lista de notificados");
        }
    }
}
