using CSharpFunctionalExtensions;

using Ineditta.Application.Cnaes.Entities;
using Ineditta.Application.Cnaes.Repositories;
using Ineditta.BuildingBlocks.Core.Database;
using Ineditta.BuildingBlocks.Core.Domain.Handlers;

using MediatR;

namespace Ineditta.Application.Cnaes.UseCases.Upsert
{
    public class UpsertCnaeRequestHandler : BaseCommandHandler, IRequestHandler<UpsertCnaeRequest, Result>
    {
        private readonly ICnaeRepository _cnaeRepository;

        public UpsertCnaeRequestHandler(IUnitOfWork unitOfWork, ICnaeRepository cnaeRepository) : base(unitOfWork)
        {
            _cnaeRepository = cnaeRepository;
        }

        public async Task<Result> Handle(UpsertCnaeRequest request, CancellationToken cancellationToken)
        {
            return request.Id <= 0 ? await Incluir(request, cancellationToken) : await Atualizar(request, cancellationToken);
        }

        public async Task<Result> Incluir(UpsertCnaeRequest request, CancellationToken cancellationToken)
        {
            var result = Cnae.Criar(request.Divisao, request.DescricaoDivisao, request.SubClasse, request.DescricaoSubClasse, request.Categoria);

            if (result.IsFailure)
            {
                return result;
            }

            await _cnaeRepository.IncluirAsync(result.Value);

            await CommitAsync(cancellationToken);

            return Result.Success();
        }

        public async Task<Result> Atualizar(UpsertCnaeRequest request, CancellationToken cancellationToken)
        {
            var cnae = await _cnaeRepository.ObterPorIdAsync(request.Id);

            if (cnae == null)
            {
                return Result.Failure("Cnae não encontrado");
            }

            var result = cnae.Atualizar(request.Divisao, request.DescricaoDivisao, request.SubClasse, request.DescricaoSubClasse, request.Categoria);

            if (result.IsFailure)
            {
                return result;
            }

            await _cnaeRepository.AtualizarAsync(cnae);

            await CommitAsync(cancellationToken);

            return Result.Success();
        }
    }
}
