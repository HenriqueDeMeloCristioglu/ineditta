using Ineditta.Application.CalendarioSindicais.Eventos.Dtos;

namespace Ineditta.Application.CalendarioSindicais.Eventos.Repositories
{
    public interface IEventoTrintidioRepository
    {
        ValueTask<IEnumerable<EventoTrintidioViewModel>?> ObterEventosTrintidioAsync();
        ValueTask<IEnumerable<string>> ObterListaEmailPorDocumentoId(int documentoId);
    }
}
