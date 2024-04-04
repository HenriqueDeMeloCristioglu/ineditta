using Ineditta.Application.CalendarioSindicais.Eventos.Dtos;

namespace Ineditta.Application.CalendarioSindicais.Eventos.Repositories
{
    public interface IEventoVencimentoMandatoLaboralRepository
    {
        ValueTask<IEnumerable<VencimentoMandatoLaboralViewModel>> ObterNotificacoesVencimentoMandatoLaboral();
        ValueTask<IEnumerable<string>> ObterListaEmailNotificacao(int sindicatoLaboralId);
    }
}
