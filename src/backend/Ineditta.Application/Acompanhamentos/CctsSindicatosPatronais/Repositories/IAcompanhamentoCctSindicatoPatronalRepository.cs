using Ineditta.Application.Acompanhamentos.CctsSindicatosPatronais.Entities;

namespace Ineditta.Application.Acompanhamentos.CctsSindicatosPatronais.Repositories
{
    public interface IAcompanhamentoCctSindicatoPatronalRepository
    {
        ValueTask IncluirAsync(AcompanhamentoCctSinditoPatronal acompanhamentoCctSinditoPatronal);
        ValueTask DeletarAsync(AcompanhamentoCctSinditoPatronal acompanhamentoCctSinditoPatronal);
        ValueTask<IEnumerable<AcompanhamentoCctSinditoPatronal>?> ObterPorAcompanhamentoIdAsync(long acompanhamentoId);
    }
}
