using Ineditta.Application.Documentos.SindicatosLaborais;

namespace Ineditta.Application.Documentos.SindicatosLaborais.Repositories
{
    public interface IDocumentoSindicatoLaboralRepository
    {
        ValueTask InserirAsync(DocumentoSindicatoLaboral documentoSindicatoLaboral);
        ValueTask DeletarPorDocumentoIdAsync(int documentoId);
    }
}
