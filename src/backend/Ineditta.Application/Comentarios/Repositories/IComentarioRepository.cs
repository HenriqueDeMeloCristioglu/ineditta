using Ineditta.Application.Comentarios.Entities;

namespace Ineditta.Application.Comentarios.Repositories
{
    public interface IComentarioRepository
    {
        ValueTask IncluirAsync(Comentario comentario);
        ValueTask AtualizarAsync(Comentario comentario);
        ValueTask<Comentario?> ObterPorIdAsync(long id);
    }
}
