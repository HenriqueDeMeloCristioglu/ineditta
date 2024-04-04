using Ineditta.Application.EstruturasClausulas.Gerais.Entities;

namespace Ineditta.Application.EstruturasClausulas.Gerais.Repositories
{
    public interface IEstruturaClausulaRepository
    {
        ValueTask<IEnumerable<EstruturaClausula>?> ObterTodasNaoContaDocumento(int documentoId);
        ValueTask<IEnumerable<string>> ObterRelacaoClassificacaoAsync();
        ValueTask<EstruturaClausula?> ObterPorIdAsync(int id);
        ValueTask<IEnumerable<EstruturaClausula>?> ObterTodasPorIds(IEnumerable<int> estruturasClausulasIds);
    }
}
