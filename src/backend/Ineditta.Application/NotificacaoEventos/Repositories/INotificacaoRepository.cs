using Ineditta.Application.NotificacaoEventos.Entities;

namespace Ineditta.Application.NotificacaoEventos.Repositories
{
    public interface INotificacaoRepository
    {
        ValueTask IncluirAsync(Notificacao notificacao);
        ValueTask<Notificacao?> ObterPorIdAsync(long id);
    }
}
