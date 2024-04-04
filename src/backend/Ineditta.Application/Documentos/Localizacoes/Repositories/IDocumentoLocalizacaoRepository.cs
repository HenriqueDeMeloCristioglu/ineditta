using Ineditta.Application.Documentos.Localizacoes.Entities;

namespace Ineditta.Application.Documentos.Localizacoes.Repositories
{
    public interface IDocumentoLocalizacaoRepository
    {
        ValueTask InserirAsync(DocumentoLocalizacao documentoAbrangencia);
        ValueTask AtualizarAsync(DocumentoLocalizacao documentoAbrangencia);
        ValueTask RemoverTudoPorDocumentoIdAsync(int documentoId);
    }
}
