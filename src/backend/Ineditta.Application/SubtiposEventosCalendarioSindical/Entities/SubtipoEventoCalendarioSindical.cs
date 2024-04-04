using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CSharpFunctionalExtensions;

namespace Ineditta.Application.SubtiposEventosCalendarioSindical.Entities
{
    public class SubtipoEventoCalendarioSindical : Entity<int>
    {
        private SubtipoEventoCalendarioSindical(string nome, int tipoEventoId)
        {
            Nome = nome;
            TipoEventoId = tipoEventoId;
        }

        protected SubtipoEventoCalendarioSindical() { }

        public string Nome { get; private set; } = null!;
        public int TipoEventoId { get; private set; }

        public static Result<SubtipoEventoCalendarioSindical> Criar(string nome, int tipoEventoId)
        {
            if (string.IsNullOrEmpty(nome)) return Result.Failure<SubtipoEventoCalendarioSindical>("Você precisa fornecer o nome do tipo de evento");
            if (nome.Length > 120) return Result.Failure<SubtipoEventoCalendarioSindical>("O nome do subtipo de evento deve ser menor ou igual a 120 caracteres.");
            if (tipoEventoId <= 0) return Result.Failure<SubtipoEventoCalendarioSindical>("O id do tipo de evento deve ser maior que 0");

            var tipoEvento = new SubtipoEventoCalendarioSindical(nome, tipoEventoId);
            return Result.Success(tipoEvento);
        }
    }
}
