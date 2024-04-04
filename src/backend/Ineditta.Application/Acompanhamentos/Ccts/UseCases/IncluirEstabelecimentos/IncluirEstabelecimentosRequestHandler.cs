using CSharpFunctionalExtensions;

using Ineditta.Application.Acompanhamentos.Ccts.Repositories;
using Ineditta.Application.Acompanhamentos.Ccts.UseCases.IncluirEstabelecimentos;
using Ineditta.BuildingBlocks.Core.Database;
using Ineditta.BuildingBlocks.Core.Domain.Handlers;
using Ineditta.BuildingBlocks.Core.Domain.Models;

using MediatR;

namespace Ineditta.Application.Acompanhamentos.Ccts.UseCases.IncluirEstabelecimentosSindicatos
{
    public class IncluirEstabelecimentosRequestHandler : BaseCommandHandler, IRequestHandler<IncluirEstabelecimentosRequest, Result<Unit, Error>>
    {
        private readonly IAcompanhamentoCctRepository _acompanhamentoCctRepository;

        public IncluirEstabelecimentosRequestHandler(IUnitOfWork unitOfWork, IAcompanhamentoCctRepository acompanhamentoCctRepository) : base(unitOfWork)
        {
            _acompanhamentoCctRepository = acompanhamentoCctRepository;
        }

        public async Task<Result<Unit, Error>> Handle(IncluirEstabelecimentosRequest request, CancellationToken cancellationToken)
        {
            var result = await _acompanhamentoCctRepository.IncluirEstabelecimentos();

            if (result.IsFailure)
            {
                return Result.Failure<Unit, Error>(Error.Create("400", "Erro ao incluir estabelecimentos e sindicatos"));
            }

            return Result.Success<Unit, Error>(Unit.Value);
        }
    }
}
