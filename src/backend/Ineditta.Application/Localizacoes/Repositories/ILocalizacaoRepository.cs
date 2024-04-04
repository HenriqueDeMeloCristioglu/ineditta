using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Ineditta.Application.Localizacoes.Entities;

namespace Ineditta.Application.Localizacoes.Repositories
{
    public interface ILocalizacaoRepository
    {
        ValueTask<Localizacao?> ObterPorIdAsync(int localizacaoId);
        ValueTask<IEnumerable<Localizacao>?> ObterPorListaIdAsync(IEnumerable<int> ids);
    }
}
