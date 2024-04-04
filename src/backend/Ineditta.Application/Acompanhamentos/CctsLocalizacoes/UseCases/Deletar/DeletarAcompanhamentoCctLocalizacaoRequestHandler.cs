using CSharpFunctionalExtensions;

using Ineditta.Application.Acompanhamentos.CctsLocalizacoes.Repositories;
using Ineditta.BuildingBlocks.Core.Database;
using Ineditta.BuildingBlocks.Core.Domain.Handlers;

using MediatR;

namespace Ineditta.Application.Acompanhamentos.CctsLocalizacoes.UseCases.Deletar
{
    public class DeletarAcompanhamentoCctLocalizacaoRequestHandler : BaseCommandHandler, IRequestHandler<DeletarAcompanhamentoCctLocalizacaoRequest, Result>
    {
        private readonly IAcompanhamentoCctLocalizacaoRepository _acompanhamentoCctLocalizacaoRepository;

        public DeletarAcompanhamentoCctLocalizacaoRequestHandler(IUnitOfWork unitOfWork, IAcompanhamentoCctLocalizacaoRepository acompanhamentoCctLocalizacaoRepository) : base(unitOfWork)
        {
            _acompanhamentoCctLocalizacaoRepository = acompanhamentoCctLocalizacaoRepository;
        }

        public async Task<Result> Handle(DeletarAcompanhamentoCctLocalizacaoRequest request, CancellationToken cancellationToken)
        {
            await _acompanhamentoCctLocalizacaoRepository.DeletarAsync(request.AcompanhamentoCctLocalizacao);

            await CommitAsync(cancellationToken);

            return Result.Success();
        }
    }
}
