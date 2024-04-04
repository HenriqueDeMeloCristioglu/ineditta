using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CSharpFunctionalExtensions;

using Ineditta.BuildingBlocks.Core.Domain.ValueObjects;

namespace Ineditta.Application.NotificacaoEventos.Entities
{
    public class Notificacao : Entity
    {
        private Notificacao(long eventoId, IEnumerable<Email> notificados, DateTime notificadosEm)
        {
            EventoId = eventoId;
            Notificados = notificados;
            NotificadosEm = notificadosEm;
        }

        protected Notificacao() { }

        public long EventoId { get; private set; }
        public IEnumerable<Email> Notificados { get; private set; } = null!;
        public DateTime NotificadosEm { get; private set; }

        public static Result<Notificacao> Criar(long eventoId,  IEnumerable<Email> notificados)
        {
            if (eventoId <= 0) return Result.Failure<Notificacao>("Você deve fornecer um eventoId válido");
            if (notificados is null) return Result.Failure<Notificacao>("Você deve forcenecer a lista de notificados");

            Notificacao notificacao = new Notificacao(eventoId, notificados, DateTime.Now);
            return Result.Success(notificacao);
        }
    }
}
