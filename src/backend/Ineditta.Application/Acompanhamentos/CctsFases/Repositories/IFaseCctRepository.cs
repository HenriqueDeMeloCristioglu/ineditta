using Ineditta.Application.CctsFases.Entities;

namespace Ineditta.Application.Acompanhamentos.CctsFases.Repositories
{
    public interface IFaseCctRepository
    {
        ValueTask<FasesCct?> ObterPorIdAsync(long id);
    }
}
