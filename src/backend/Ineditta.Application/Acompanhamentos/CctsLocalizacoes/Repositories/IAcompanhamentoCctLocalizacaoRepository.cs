using Ineditta.Application.Acompanhamentos.CctsLocalizacoes.Entities;

namespace Ineditta.Application.Acompanhamentos.CctsLocalizacoes.Repositories
{
    public interface IAcompanhamentoCctLocalizacaoRepository
    {
        ValueTask IncluirAsync(AcompanhamentoCctLocalizacao acompanhamentoCctLocalizacao);
        ValueTask DeletarAsync(AcompanhamentoCctLocalizacao acompanhamentoCctLocalizacao);
        ValueTask<IEnumerable<AcompanhamentoCctLocalizacao>?> ObterPorAcompanhamentoIdAsync(long acompanhamentoId);
    }
}
