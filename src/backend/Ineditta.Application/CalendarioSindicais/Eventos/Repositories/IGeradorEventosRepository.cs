using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CSharpFunctionalExtensions;

using Ineditta.Application.CalendarioSindicais.Eventos.Dtos;

namespace Ineditta.Application.CalendarioSindicais.Eventos.Repositories
{
    public interface IGeradorEventosRepository
    {
        ValueTask<IResult<IEnumerable<EventoClausulaBaseDto>?>> ObterListaEventosBaseClausulasPeriodicos();
        ValueTask<IResult> GerarEventosNaoPeriodicos();
        ValueTask<IResult> GerarEventosPeriodicos();
    }
}
