using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CSharpFunctionalExtensions;

using Ineditta.Application.CalendarioSindicais.Usuarios.Dictionaries;
using Ineditta.Application.CalendarioSindicais.Usuarios.Entities;

namespace Ineditta.Application.CalendarioSindicais.Usuarios.Factories
{
    public static class EventoAgendaProximaDataFactory
    {
        public static Result<DateTime> Criar(TipoRecorrencia recorrencia, DateTime data, int diaBase)
        {
            var funcaoProximaData = DTipoRecorrenciaNovaData.Obter().GetValueOrDefault(recorrencia);

            if (funcaoProximaData == null)
            {
                return Result.Failure<DateTime>("Tipo de recorrencia inválido");
            }

            var proximaData = funcaoProximaData(data);

            if (recorrencia == TipoRecorrencia.Semanal || recorrencia == TipoRecorrencia.Quinzenal)
            {
                return Result.Success(proximaData);
            }

            var anoProximaData = proximaData.Year;
            var mesProximaData = proximaData.Month;
            var horaProximaData = proximaData.Hour;
            var minutoProximaData = proximaData.Minute;
            var diasNoMesProximaData = DateTime.DaysInMonth(anoProximaData, mesProximaData);

            if (diaBase <= diasNoMesProximaData && diaBase != proximaData.Day)
            {
                proximaData = new DateTime(anoProximaData, mesProximaData, diaBase, horaProximaData, minutoProximaData, 0, DateTimeKind.Utc);
            }

            return Result.Success(proximaData);
        }
    }
}
