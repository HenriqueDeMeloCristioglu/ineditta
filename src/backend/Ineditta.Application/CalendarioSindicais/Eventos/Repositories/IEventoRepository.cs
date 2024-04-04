using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ineditta.Application.ClientesUnidades.Entities;
using Ineditta.Application.CalendarioSindicais.Eventos.Dtos;
using Ineditta.Application.CalendarioSindicais.Eventos.Entities;

namespace Ineditta.Application.CalendarioSindicais.Eventos.Repositories
{
    public interface IEventoRepository
    {
        ValueTask IncluirAsync(CalendarioSindical evento);
        ValueTask AtualizarAsync(CalendarioSindical evento);
        ValueTask<CalendarioSindical?> ObterPorIdAsync(long id);
        ValueTask<IEnumerable<EventoClausulaViewModel>?> ObterNotificacoesEventosClausulas();
        ValueTask<IEnumerable<string>?> ObterListaEmailEventoPorDocumentoId(int documentoId, int tipoEventoId, int? subtipoEventoId = null);
        ValueTask<IEnumerable<AgendaEventosViewModel>?> ObterNotificacoesAgendaEventos();
        ValueTask<IEnumerable<AssembleiaReuniaoViewModelBase>?> ObterAssembleiaPatronalReunioesSindicaisAsync(int tipoEvento);
        ValueTask<IEnumerable<string>?> ObterListaEmailAssembleiaPatronalReuniaoAsync(AssembleiaReuniaoViewModelBase assembleiaPatronalReuniao, int tipoEventoId);
    }
}
