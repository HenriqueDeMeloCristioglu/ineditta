using CSharpFunctionalExtensions;

using Ineditta.Application.Etiquetas.Entities;
using Ineditta.Application.Etiquetas.Respositories;
using Ineditta.BuildingBlocks.Core.Database;
using Ineditta.BuildingBlocks.Core.Domain.Handlers;

using MediatR;

namespace Ineditta.Application.Etiquetas.UseCases.Upsert
{
    public class UpsertEtiquetaHandler : BaseCommandHandler, IRequestHandler<UpsertEtiquetaRequest, Result>
    {
        private readonly IEtiquetaRepository _etiquetaRepository;
        public UpsertEtiquetaHandler(IUnitOfWork unitOfWork, IEtiquetaRepository etiquetaRepository) : base(unitOfWork)
        {
            _etiquetaRepository = etiquetaRepository;
        }

        public async Task<Result> Handle(UpsertEtiquetaRequest request, CancellationToken cancellationToken)
        {
            return request.Id > 0 ? await Atualizar(request, cancellationToken) : await Incluir(request, cancellationToken);
        }

        public async Task<Result> Incluir(UpsertEtiquetaRequest request, CancellationToken cancellationToken)
        {
            var etiqueta = Etiqueta.Criar(request.Nome, request.TipoEtiquetaId);

            if (etiqueta.IsFailure)
            {
                return etiqueta;
            }

            await _etiquetaRepository.IncluirAsync(etiqueta.Value);

            _ = await CommitAsync(cancellationToken);

            return Result.Success();
        }

        public async Task<Result> Atualizar(UpsertEtiquetaRequest request, CancellationToken cancellationToken)
        {
            var etiqueta = await _etiquetaRepository.ObterPorIdAsync(request.Id);

            if (etiqueta == null)
            {
                return Result.Failure("Etiqueta não encontrada");
            }

            var result = etiqueta.Atualizar(request.Nome, request.TipoEtiquetaId);

            if (result.IsFailure)
            {
                return result;
            }

            await _etiquetaRepository.AtualizarAsync(etiqueta);

            _ = await CommitAsync(cancellationToken);

            return Result.Success();
        }
    }
}
