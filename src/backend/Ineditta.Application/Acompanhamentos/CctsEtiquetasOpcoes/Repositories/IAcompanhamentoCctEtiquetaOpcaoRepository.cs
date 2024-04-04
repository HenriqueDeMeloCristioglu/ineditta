using Ineditta.Application.Acompanhamentos.CctsEtiquetasOpcoes.Entities;

namespace Ineditta.Application.Acompanhamentos.CctsEtiquetasOpcoes.Repositories
{
    public interface IAcompanhamentoCctEtiquetaOpcaoRepository
    {
        ValueTask<AcompanhamentoCctEtiquetaOpcao?> ObterPorId(long id);
    }
}
