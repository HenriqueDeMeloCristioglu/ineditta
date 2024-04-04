using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Ineditta.Application.Usuarios.Entities;
using Ineditta.Application.UsuariosTiposEventosCalendarioSindical.Entities;

namespace Ineditta.Application.UsuariosTiposEventosCalendarioSindical.Repositories
{
    public interface IUsuarioTipoEventoCalendarioSindicalRepository
    {
        ValueTask IncluirAsync(UsuarioTipoEventoCalendarioSindical usuarioTipoEventoCalendarioSindical);
        ValueTask AtualizarAsync(UsuarioTipoEventoCalendarioSindical usuarioTipoEventoCalendarioSindical);
        ValueTask<IEnumerable<UsuarioTipoEventoCalendarioSindical>> ObterTodosPorUsuarioIdAsync(long usuarioId);
    }
}
