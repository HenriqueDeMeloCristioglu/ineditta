using Ineditta.Application.CalendarioSindicais.Eventos.Dtos;

namespace Ineditta.Application.CalendarioSindicais.Eventos.Repositories
{
    public interface IEventoVencimentoMandatoPatronalRepository
    {
        ValueTask<IEnumerable<VencimentoMandatoPatronalViewModel>> ObterNotificacoesVencimentoMandatoPatronal();
        ValueTask<IEnumerable<string>> ObterListaEmailNotificacao(int sindicatoPatronalId);
    }
}
