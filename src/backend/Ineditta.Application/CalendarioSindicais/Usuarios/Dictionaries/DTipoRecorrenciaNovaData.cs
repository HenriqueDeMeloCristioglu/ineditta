using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Ineditta.Application.CalendarioSindicais.Usuarios.Entities;

namespace Ineditta.Application.CalendarioSindicais.Usuarios.Dictionaries
{
    public static class DTipoRecorrenciaNovaData
    {
        public static Dictionary<TipoRecorrencia, Func<DateTime, DateTime>> Obter()
        {
            Dictionary<TipoRecorrencia, Func<DateTime, DateTime>> dictionary = new()
            {
                { TipoRecorrencia.Semanal, (DateTime data) => data.AddDays(7) },
                { TipoRecorrencia.Quinzenal, (DateTime data) => data.AddDays(15) },
                { TipoRecorrencia.Mensal, (DateTime data) => data.AddMonths(1) },
                { TipoRecorrencia.Bimestral, (DateTime data) => data.AddMonths(2) },
                { TipoRecorrencia.Trimestral, (DateTime data) => data.AddMonths(3) },
                { TipoRecorrencia.Semestral, (DateTime data) => data.AddMonths(6) },
                { TipoRecorrencia.Anual, (DateTime data) => data.AddMonths(12) },
            };

            return dictionary;
        }
    }
}
