using Ineditta.Application.Federacoes.Entities;
using Ineditta.BuildingBlocks.Core.Domain.ValueObjects;

namespace Ineditta.Application.Federacoes.Repositories
{
    public interface IFederacaoRepository
    {
        ValueTask IncluirAsync(Federacao federacao);
        ValueTask<Federacao> ObterAsync(int id);
        ValueTask<bool> ExisteAsync(CNPJ cnpj);
    }
}
