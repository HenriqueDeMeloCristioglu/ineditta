using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CSharpFunctionalExtensions;
using MediatR;

namespace Ineditta.Application.Confederacoes.UseCases.Upsert
{
    public class UpsertConfederacaoRequestHandler : IRequestHandler<UpsertConfederacaoRequest, Result>
    {
        public async Task<Result> Handle(UpsertConfederacaoRequest request, CancellationToken cancellationToken)
        {
            var result = Result.Success();

            await Task.CompletedTask;

            return result;
        }
    }
}
