using Ineditta.Application.Documentos.Sindicais.Entities;

namespace Ineditta.Application.Documentos.Sindicais.Repositories
{
    public interface IDocumentoSindicalRepository
    {
        ValueTask IncluirAsync(DocumentoSindical documentoSindical, CancellationToken cancellationToken = default);
        ValueTask AtualizarAsync(DocumentoSindical documentoSindical, CancellationToken cancellationToken = default);
        ValueTask<DocumentoSindical?> ObterPorIdAsync(long id);
        ValueTask<bool> ExistePorIdAsync(long id);
    }
}
