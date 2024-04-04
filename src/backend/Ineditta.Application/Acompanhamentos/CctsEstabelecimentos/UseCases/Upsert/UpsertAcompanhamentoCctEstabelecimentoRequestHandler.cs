
using CSharpFunctionalExtensions;

using Ineditta.Application.Acompanhamentos.CctsEstabelecimentos.Entities;
using Ineditta.Application.Acompanhamentos.CctsEstabelecimentos.Repositories;
using Ineditta.BuildingBlocks.Core.Database;
using Ineditta.BuildingBlocks.Core.Domain.Handlers;

using MediatR;

namespace Ineditta.Application.Acompanhamentos.CctsEstabelecimentos.UseCases.Upsert
{
    public class UpsertAcompanhamentoCctEstabelecimentoRequestHandler : BaseCommandHandler, IRequestHandler<UpsertAcompanhamentoCctEstabelecimentoRequest, Result>
    {
        private readonly IAcompanhamentoCctEstabelecimentoRepository _acompanhamentoCctEstabelecimentoRepository;

        public UpsertAcompanhamentoCctEstabelecimentoRequestHandler(IUnitOfWork unitOfWork, IAcompanhamentoCctEstabelecimentoRepository acompanhamentoCctEstabelecimentoRepository) : base(unitOfWork)
        {
            _acompanhamentoCctEstabelecimentoRepository = acompanhamentoCctEstabelecimentoRepository;
        }

        public async Task<Result> Handle(UpsertAcompanhamentoCctEstabelecimentoRequest request, CancellationToken cancellationToken)
        {
            var acompanhamentoCctEstabelecimento = AcompanhamentoCctEstabelecimento.Criar(request.AcompanhamentoCctId, request.GrupoEconomicoId, request.EmpresaId, request.EstabelecimentoId);

            await _acompanhamentoCctEstabelecimentoRepository.IncluirTask(acompanhamentoCctEstabelecimento.Value);

            return Result.Success();
        }
    }
}
