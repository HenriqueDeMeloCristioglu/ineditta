using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CSharpFunctionalExtensions;

using MediatR;

namespace Ineditta.Application.NotificacaoEventos.UseCases.Upsert
{
    public class UpsertNotificacaoRequest : IRequest<Result>
    {
        public long? Id { get; set; }
        public long EventoId { get; set; }
        public IEnumerable<string> Notificados { get; set; } = null!;
        public DateTime? NotificadosEm { get; set; }
    }
}
