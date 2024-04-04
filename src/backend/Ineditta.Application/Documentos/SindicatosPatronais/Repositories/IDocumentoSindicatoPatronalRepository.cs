using Ineditta.Application.Documentos.SindicatosPatronais;

namespace Ineditta.Application.Documentos.SindicatosPatronais.Repositories
{
    public interface IDocumentoSindicatoPatronalRepository
    {
        ValueTask InserirAsync(DocumentoSindicatoPatronal documentoSindicatoPatronal);
        ValueTask DeletarPorDocumentoIdAsync(int documentoId);
    }
}
