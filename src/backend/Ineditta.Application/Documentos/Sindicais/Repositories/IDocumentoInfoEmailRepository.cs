using Ineditta.Application.Documentos.Sindicais.Dtos;

namespace Ineditta.Application.Documentos.Sindicais.Repositories
{
    public interface IDocumentoInfoEmailRepository
    {
        ValueTask<InfoDocumentoCriadoDto?> ObterInfoDocumentoCriadoEmail(long idDocumento);
    }
}
