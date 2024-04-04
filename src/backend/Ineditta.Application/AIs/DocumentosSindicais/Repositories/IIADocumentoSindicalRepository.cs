using Ineditta.Application.AIs.DocumentosSindicais.Entities;

namespace Ineditta.Application.AIs.DocumentosSindicais.Repositories
{
    public interface IIADocumentoSindicalRepository
    {
        ValueTask IncluirAsync(IADocumentoSindical iaDocumentoSindical);
        ValueTask AtualizarAsync(IADocumentoSindical iaDocumentoSindical);
        ValueTask<IADocumentoSindical?> ObterPorIdAsync(long id);
        ValueTask<bool> ExistePorDocumentoReferenciaIdAsync(int documentoReferenciaId);
        ValueTask LockAsync(long iADocumentoSindicalId, CancellationToken cancellationToken = default);
    }
}
