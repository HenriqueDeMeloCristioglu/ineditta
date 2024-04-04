using CSharpFunctionalExtensions;

using Ineditta.Application.Acompanhamentos.CctsLocalizacoes.Entities;
using Ineditta.Application.Acompanhamentos.CctsLocalizacoes.Repositories;
using Ineditta.BuildingBlocks.Core.Database;
using Ineditta.BuildingBlocks.Core.Domain.Handlers;

using MediatR;

namespace Ineditta.Application.Acompanhamentos.CctsLocalizacoes.UseCases.Upsert
{
    public class IncluirAcompanhamentoCctLocalizacaoRequestHandler : BaseCommandHandler, IRequestHandler<IncluirAcompanhamentoCctLocalizacaoRequest, Result>
    {
        private readonly IAcompanhamentoCctLocalizacaoRepository _acompanhamentoCctLocalizacaoRepository;

        public IncluirAcompanhamentoCctLocalizacaoRequestHandler(IUnitOfWork unitOfWork, IAcompanhamentoCctLocalizacaoRepository acompanhamentoCctLocalizacaoRepository) : base(unitOfWork)
        {
            _acompanhamentoCctLocalizacaoRepository = acompanhamentoCctLocalizacaoRepository;
        }

        public async Task<Result> Handle(IncluirAcompanhamentoCctLocalizacaoRequest request, CancellationToken cancellationToken)
        {
            var acompanhamentoCctLocalizacao = AcompanhamentoCctLocalizacao.Criar(request.AcompanhamentoCctId, request.LocalizacaoId);

            if (acompanhamentoCctLocalizacao.IsFailure)
            {
                return acompanhamentoCctLocalizacao;
            }

            await _acompanhamentoCctLocalizacaoRepository.IncluirAsync(acompanhamentoCctLocalizacao.Value);

            await CommitAsync(cancellationToken);

            return Result.Success();
        }
    }
}
