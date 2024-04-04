using CSharpFunctionalExtensions;

using Ineditta.Application.TiposEtiquetas.Entities;
using Ineditta.Application.TiposEtiquetas.Respositories;
using Ineditta.BuildingBlocks.Core.Database;
using Ineditta.BuildingBlocks.Core.Domain.Handlers;

using MediatR;

namespace Ineditta.Application.TiposEtiquetas.UseCases.Upsert
{
    public class UpsertTipoEtiquetaHandler : BaseCommandHandler, IRequestHandler<UpsertTipoEtiquetaRequest, Result>
    {
        private readonly ITipoEtiquetaRepository _tipoEtiquetaRepository;

        public UpsertTipoEtiquetaHandler(IUnitOfWork unitOfWork, ITipoEtiquetaRepository tipoEtiquetaRepository) : base(unitOfWork)
        {
            _tipoEtiquetaRepository = tipoEtiquetaRepository;
        }

        public async Task<Result> Handle(UpsertTipoEtiquetaRequest request, CancellationToken cancellationToken)
        {
            return request.Id > 0 ? await Atualizar(request, cancellationToken) : await Incluir(request, cancellationToken);
        }

        public async Task<Result> Atualizar(UpsertTipoEtiquetaRequest request, CancellationToken cancellationToken)
        {
            var tipoEtiqueta = await _tipoEtiquetaRepository.ObterPorIdAsync(request.Id);

            if (tipoEtiqueta is null)
            {
                return Result.Failure("Tipo de etiqueta não encontrada");
            }

            var result = tipoEtiqueta.Atualizar(request.Nome);

            if (result.IsFailure)
            {
                return result;
            }

            await _tipoEtiquetaRepository.AtualizarAsync(tipoEtiqueta);

            _ = await CommitAsync(cancellationToken);

            return Result.Success();
        }

        public async Task<Result> Incluir(UpsertTipoEtiquetaRequest request, CancellationToken cancellationToken)
        {
            var tipoEtiqueta = TipoEtiqueta.Criar(request.Nome);

            if (tipoEtiqueta.IsFailure)
            {
                return tipoEtiqueta;
            }

            await _tipoEtiquetaRepository.InlcuirAsync(tipoEtiqueta.Value);

            _ = await CommitAsync(cancellationToken);

            return Result.Success();
        }
    }
}
