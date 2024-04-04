using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Ineditta.Application.ModulosClientes.Entities;
using Ineditta.Application.TiposDocumentosClientesMatriz.Entities;

namespace Ineditta.Application.TiposDocumentosClientesMatriz.repositories
{
    public interface ITipoDocumentoClienteMatrizRepository
    {
        ValueTask AtualizarAsync(TipoDocumentoClienteMatriz tipoDocumentoClienteMatriz, CancellationToken cancellationToken = default);
        ValueTask IncluirAsync(TipoDocumentoClienteMatriz tipoDocumentoClienteMatriz, CancellationToken cancellationToken = default);
        ValueTask LimparTodosPorMatrizIdAsync(int matrizId);
    }
}
