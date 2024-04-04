using Ineditta.Application.Confederacoes.Entities;
using Ineditta.BuildingBlocks.Core.Domain.ValueObjects;

namespace Ineditta.Application.Confederacoes.Repositories
{
    public interface IConfederacaoRepository
    {
        ValueTask IncluirAsync(Confederacao confederacao);
        ValueTask<Confederacao> ObterAsync(int id);
        ValueTask<bool> ExisteAsync(CNPJ cnpj);
    }
}
