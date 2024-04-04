using CSharpFunctionalExtensions;

namespace Ineditta.Application.Documentos.AtividadesEconomicas.Repositories
{
    public interface IDocumentoAtividadeEconomicaRepository
    {
        ValueTask<Result> IncluirAtividadeEconomicaDocumento();
    }
}
