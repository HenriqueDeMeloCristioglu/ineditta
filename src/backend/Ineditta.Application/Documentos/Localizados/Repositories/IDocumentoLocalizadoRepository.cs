using Ineditta.Application.DocumentosLocalizados.Entities;

namespace Ineditta.Application.Documentos.Localizados.Repositories
{
    public interface IDocumentoLocalizadoRepository
    {
        ValueTask IncluirAsync(DocumentoLocalizado documentoLocalizado);
        ValueTask AtualizarAsync(DocumentoLocalizado documentoLocalizado);
        ValueTask<DocumentoLocalizado?> ObterPorIdAsync(long id);
        ValueTask DeletarAsync(DocumentoLocalizado documentoLocalizado);
    }
}
