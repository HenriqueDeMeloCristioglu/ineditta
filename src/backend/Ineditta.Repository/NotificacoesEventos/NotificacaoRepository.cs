using Ineditta.Application.NotificacaoEventos.Entities;
using Ineditta.Application.NotificacaoEventos.Repositories;
using Ineditta.Repository.Contexts;

namespace Ineditta.Repository.NotificacoesEventos
{
    public class NotificacaoRepository : INotificacaoRepository
    {
        private readonly InedittaDbContext _context;

        public NotificacaoRepository(InedittaDbContext context)
        {
            _context = context;
        }

        public async ValueTask IncluirAsync(Notificacao notificacao)
        {
            _context.Notificacoes.Add(notificacao);

            await Task.CompletedTask;
        }

        public async ValueTask<Notificacao?> ObterPorIdAsync(long id)
        {
            return await _context.Notificacoes.FindAsync(id);
        }
    }
}
