using Ineditta.Application.Etiquetas.Entities;

namespace Ineditta.Application.Etiquetas.Respositories
{
    public interface IEtiquetaRepository
    {
        ValueTask IncluirAsync(Etiqueta etiqueta);
        ValueTask AtualizarAsync(Etiqueta etiqueta);
        ValueTask<Etiqueta?> ObterPorIdAsync(long id);
    }
}
