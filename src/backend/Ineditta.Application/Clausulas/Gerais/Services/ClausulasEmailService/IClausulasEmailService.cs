using CSharpFunctionalExtensions;

using Ineditta.Application.Clausulas.Gerais.Dtos;
using Ineditta.BuildingBlocks.Core.Domain.Models;

namespace Ineditta.Application.Clausulas.Gerais.Services.ClausulasEmailService
{
    public interface IClausulasEmailService
    {
        ValueTask<Result<bool, Error>> EnviarEmailClausulasAprovadasAsync(DocumentoClausulasLiberadasEmailDto infoEmail, long documentoId, IEnumerable<string> emails, CancellationToken cancellationToken);
    }
}
