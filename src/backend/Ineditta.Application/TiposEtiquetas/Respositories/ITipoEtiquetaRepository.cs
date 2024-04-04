using Ineditta.Application.TiposEtiquetas.Entities;

namespace Ineditta.Application.TiposEtiquetas.Respositories
{
    public interface ITipoEtiquetaRepository
    {
        ValueTask InlcuirAsync(TipoEtiqueta tipoEtiqueta);
        ValueTask AtualizarAsync(TipoEtiqueta tipoEtiqueta);
        ValueTask<TipoEtiqueta?> ObterPorIdAsync(int id);
    }
}
