using Ineditta.Application.CalendarioSindicais.Usuarios.Entities;

namespace Ineditta.Application.CalendarioSindicais.Usuarios.Repositories
{
    public interface ICalendarioSindicalUsuarioRepository
    {
        ValueTask IncluirAsync(CalendarioSindicalUsuario evento);
        ValueTask AtualizarAsync(CalendarioSindicalUsuario evento);
        ValueTask<CalendarioSindicalUsuario?> ObterPorIdAsync(long id);
    }
}
