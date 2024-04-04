using Ineditta.Application.AIs.Clausulas.Entities;

namespace Ineditta.Application.AIs.Clausulas.Repositories
{
    public interface IIAClausulaRepository
    {
        ValueTask InserirAsync(IAClausula iaClausula);
        ValueTask AtualizarAsync(IAClausula iaClausula);
        ValueTask<IAClausula?> ObterPorIdAsync(long iaClausulaId);
        ValueTask<bool> ExisteClausulaPendenteClassificacaoAsync(long iADocumentoSindicalId, int ignorarId);
        ValueTask DeleteAsync(IAClausula iaClausula);
        ValueTask DeleteAsync(IEnumerable<IAClausula> iaClausula);
        ValueTask<IEnumerable<IAClausula>?> ObterTodosPorIADocumentoIdAsync(long iaDocumentoId, bool somenteNaoClassificado = false);
        ValueTask<bool> ExisteClausulaInconsistenteAsync(long iADocumentoSindicalId);
    }
}
