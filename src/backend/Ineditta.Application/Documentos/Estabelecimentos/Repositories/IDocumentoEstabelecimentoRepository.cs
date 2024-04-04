using Ineditta.Application.Documentos.Estabelecimentos.Entities;

namespace Ineditta.Application.Documentos.Estabelecimentos.Repositories
{
    public interface IDocumentoEstabelecimentoRepository
    {
        ValueTask InserirAsync(DocumentoEstabelecimento documentoEstabelecimento);
        ValueTask RemoverAsync(DocumentoEstabelecimento documentoEstabelecimento);
        ValueTask RemoverTudoPorDocumentoId(int documentoId);
    }
}
