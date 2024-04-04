using Ineditta.Application.DirigentesPatronais.Entities;

namespace Ineditta.Application.DirigentesPatronais.Repositories
{
    public interface IDirigentePatronalRepository
    {
        ValueTask IncluirAsync(DirigentePatronal dirigentePatronal);
        ValueTask AtualizarAsync(DirigentePatronal dirigentePatronal);
        ValueTask<DirigentePatronal?> ObterPorIdAsync(long dirigenteId);
    }
}
