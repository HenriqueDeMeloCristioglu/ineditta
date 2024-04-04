using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CSharpFunctionalExtensions;
using CSharpFunctionalExtensions.ValueTasks;

namespace Ineditta.Application.TiposEventosCalendarioSindical.Entities
{
    public class TipoEventoCalendarioSindical : Entity<int>
    {
        public static readonly TipoEventoCalendarioSindical VencimentoDocumento = new (1, "Vencimento de Documento");
        public static readonly TipoEventoCalendarioSindical VencimentoMandatoSindicalLaboral = new(2, "Vencimento Mandato Sindical Laboral");
        public static readonly TipoEventoCalendarioSindical VencimentoMandatoSindicalPatronal = new(3, "Vencimento Mandato Sindical Patronal");
        public static readonly TipoEventoCalendarioSindical Trintidio = new(4, "Trintidio");
        public static readonly TipoEventoCalendarioSindical EventoClausula = new(5, "Eventos de Cláusulas");
        public static readonly TipoEventoCalendarioSindical AgendaEventos = new(6, "Agenda de Eventos");
        public static readonly TipoEventoCalendarioSindical AssembleiaPatronal = new(7, "Assembléia Patronal");
        public static readonly TipoEventoCalendarioSindical ReuniaoEntrePartes = new(8, "Reunião entre as partes");
        private static readonly Dictionary<int, TipoEventoCalendarioSindical> _tiposDicionario = new() { 
            { 1, VencimentoDocumento },
            { 2, VencimentoMandatoSindicalLaboral },
            { 3, VencimentoMandatoSindicalPatronal },
            { 4, Trintidio },
            { 5, EventoClausula },
            { 6, AgendaEventos },
            { 7, AssembleiaPatronal },
            { 8, ReuniaoEntrePartes }
        };
        private TipoEventoCalendarioSindical(int id, string nome): base(id)
        {
            Nome = nome;
        }

        protected TipoEventoCalendarioSindical() { }

        public string Nome { get; private set; } = null!;

        public static Result<TipoEventoCalendarioSindical> Criar(string nome)
        {
            if (string.IsNullOrEmpty(nome)) return Result.Failure<TipoEventoCalendarioSindical>("Você precisa fornecer o nome do tipo de evento");
            if (nome.Length > 120) return Result.Failure<TipoEventoCalendarioSindical>("O nome do tipo de evento deve ser menor ou igual a 120 caracteres.");
            
            var tipoEvento = new TipoEventoCalendarioSindical(0, nome);
            return Result.Success(tipoEvento);
        }

        public static Result<TipoEventoCalendarioSindical> ObterPorId(int id)
        {   
            try
            {
                TipoEventoCalendarioSindical tipoEvento = _tiposDicionario[id];
                return Result.Success(tipoEvento);
            }
            catch (Exception ex)
            {
                return Result.Failure<TipoEventoCalendarioSindical>(ex.Message);
            }
        }
    }
}
