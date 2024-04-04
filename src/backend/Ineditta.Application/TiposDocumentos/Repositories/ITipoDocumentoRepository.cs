using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Ineditta.Application.TiposDocumentos.Entities;
using Ineditta.Application.TiposDocumentos.UseCases.Upsert;

namespace Ineditta.Application.TiposDocumentos.Repositories
{
    public interface ITipoDocumentoRepository
    {
        ValueTask IncluirAsync(TipoDocumento tipoDocumento);
        ValueTask AtualizarAsync(TipoDocumento tipoDocumento);
        ValueTask<TipoDocumento?> ObterPorIdAsync(int tipoDocumentoId);
        ValueTask<IEnumerable<TipoDocumento>?> ObterPorListaIds(IEnumerable<int> tiposIds);
    }
}
