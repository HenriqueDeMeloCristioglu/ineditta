using Ineditta.Application.Acompanhamentos.CctsSindicatosLaborais.Entities;

namespace Ineditta.Application.Acompanhamentos.CctsSindicatosLaborais.Repositories
{
    public interface IAcompanhamentoCctSindicatoLaboralRepository
    {
        ValueTask IncluirAsync(AcompanhamentoCctSinditoLaboral acompanhamentoCctSinditoLaboral);
        ValueTask DeletarAsync(AcompanhamentoCctSinditoLaboral acompanhamentoCctSinditoLaboral);
        ValueTask<IEnumerable<AcompanhamentoCctSinditoLaboral>?> ObterPorAcompanhamentoIdAsync(long acompanhamentoId);
    }
}
