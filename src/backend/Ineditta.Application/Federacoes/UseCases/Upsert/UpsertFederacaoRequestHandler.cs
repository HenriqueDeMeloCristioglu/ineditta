using CSharpFunctionalExtensions;

using MediatR;

namespace Ineditta.Application.Federacoes.UseCases.Upsert
{
    public class UpsertFederacaoRequestHandler : IRequestHandler<UpsertFederacaoRequest, Result>
    {
        public async Task<Result> Handle(UpsertFederacaoRequest request, CancellationToken cancellationToken)
        {
            var result = Result.Success();

            await Task.CompletedTask;

            return result;
        }
    }
}
