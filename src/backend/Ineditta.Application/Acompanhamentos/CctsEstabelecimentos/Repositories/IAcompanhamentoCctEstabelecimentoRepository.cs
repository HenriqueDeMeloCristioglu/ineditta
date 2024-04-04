using Ineditta.Application.Acompanhamentos.CctsEstabelecimentos.Entities;

namespace Ineditta.Application.Acompanhamentos.CctsEstabelecimentos.Repositories
{
    public interface IAcompanhamentoCctEstabelecimentoRepository
    {
        ValueTask IncluirTask(AcompanhamentoCctEstabelecimento acompanhamentoCctEstabelecimento);
    }
}
