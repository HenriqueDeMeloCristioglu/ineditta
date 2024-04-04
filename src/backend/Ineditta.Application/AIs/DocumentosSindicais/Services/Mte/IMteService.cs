using CSharpFunctionalExtensions;

using Ineditta.BuildingBlocks.Core.Domain.Models;

namespace Ineditta.Application.AIs.DocumentosSindicais.Services.Mte
{
    public interface IMteService
    {
        ValueTask<Result<string, Error>> ObterHtmlContrato(string numeroSolicitacaoMte, CancellationToken cancellationToken = default);
    }
}
