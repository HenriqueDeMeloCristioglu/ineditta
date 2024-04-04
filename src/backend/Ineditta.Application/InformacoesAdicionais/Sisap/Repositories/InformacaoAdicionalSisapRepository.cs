using Ineditta.Application.InformacoesAdicionais.Sisap.Entities;

namespace Ineditta.Application.InformacoesAdicionais.Sisap.Repositiories
{
    public interface IInformacaoAdicionalSisapRepository
    {
        ValueTask IncluirMuitosAsync(IEnumerable<InformacaoAdicionalSisap> informacosAdicionaisSisap);
        ValueTask IncluirAsync(InformacaoAdicionalSisap informacaoAdicionalSisap);
        ValueTask AtualizarAsync(InformacaoAdicionalSisap informacaoAdicionalSisap);
        ValueTask ExcluirAsync(InformacaoAdicionalSisap informacaoAdicionalSisap);
        ValueTask<List<InformacaoAdicionalSisap>?> ObterPorClausulaId(int clausulaId);
    }
}
