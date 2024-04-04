using CSharpFunctionalExtensions;
using Ineditta.Application.AIs.Clausulas.Entities;
using Ineditta.Application.AIs.Clausulas.Repositories;
using Ineditta.BuildingBlocks.Core.Database;
using Ineditta.BuildingBlocks.Core.Domain.Handlers;
using MediatR;
using Ineditta.Application.AIs.DocumentosSindicais.Repositories;
using Ineditta.Application.Sinonimos.Repositories;
using Ineditta.Application.EstruturasClausulas.Gerais.Repositories;

namespace Ineditta.Application.AIs.Clausulas.UseCases.Upsert
{
    public class UpsertIAClausulaRequestHandler : BaseCommandHandler, IRequestHandler<UpsertIAClausulaRequest, Result>
    {
        private readonly IIADocumentoSindicalRepository _iADocumentoSindicalRepository;
        private readonly IIAClausulaRepository _iaclausulaRepository;
        private readonly ISinonimoRepository _sinonimoRepository;
        private readonly IEstruturaClausulaRepository _estruturaClausulaRepository;

        public UpsertIAClausulaRequestHandler(IUnitOfWork unitOfWork, IIADocumentoSindicalRepository iADocumentoSindicalRepository, IIAClausulaRepository iaclausulaRepository, ISinonimoRepository sinonimoRepository, IEstruturaClausulaRepository estruturaClausulaRepository) : base(unitOfWork)
        {
            _iADocumentoSindicalRepository = iADocumentoSindicalRepository;
            _iaclausulaRepository = iaclausulaRepository;
            _sinonimoRepository = sinonimoRepository;
            _estruturaClausulaRepository = estruturaClausulaRepository;
        }

        public async Task<Result> Handle(UpsertIAClausulaRequest request, CancellationToken cancellationToken)
        {
            return request.Id is null ?
                 await Inserir(request, cancellationToken) :
                 await Atualizar(request, cancellationToken);
        }

        private async Task<Result> Inserir(UpsertIAClausulaRequest request, CancellationToken cancellationToken)
        {
            var iADocumentoSindical = await _iADocumentoSindicalRepository.ObterPorIdAsync(request.DocumentoSindicalId);
            if (iADocumentoSindical is null)
            {
                return Result.Failure("O documento sindical IA de id " + request.DocumentoSindicalId + " não existe");
            }

            var estruturaClausula = await _estruturaClausulaRepository.ObterPorIdAsync(request.EstruturaClausulaId);
            if (estruturaClausula is null)
            {
                return Result.Failure("A estrutura de clausula de id " + request.EstruturaClausulaId + " não existe");
            }

            var sinonimo = await _sinonimoRepository.ObterPorIdAsync(request.SinonimoId);
            if (sinonimo is null)
            {
                return Result.Failure("O sinônimo de id " + request.SinonimoId + " não existe");
            }

            if (sinonimo.EstruturaClausulaId != estruturaClausula.Id)
            {
                return Result.Failure("Sinônimo com classificação inconsistente");
            }

            var iaClausulaCriarResult = IAClausula.Criar(
                string.Empty,
                request.Texto,
                string.Empty,
                string.Empty,
                request.DocumentoSindicalId,
                request.EstruturaClausulaId,
                request.Numero,
                request.SinonimoId,
                request.Status
            );

            if (iaClausulaCriarResult.IsFailure) return iaClausulaCriarResult;

            await _iaclausulaRepository.InserirAsync(iaClausulaCriarResult.Value);

            await CommitAsync(cancellationToken);

            return Result.Success();
        }

        private async Task<Result> Atualizar(UpsertIAClausulaRequest request, CancellationToken cancellationToken)
        {
            var iaClausula = await _iaclausulaRepository.ObterPorIdAsync(request.Id ?? 0);
            if (iaClausula == null)
            {
                return Result.Failure("A ia_clausula de id " + request.Id + " não foi encontrado");
            }

            var estruturaClausula = await _estruturaClausulaRepository.ObterPorIdAsync(request.EstruturaClausulaId);
            if (estruturaClausula is null)
            {
                return Result.Failure("A estrutura de clausula de id " + request.EstruturaClausulaId + " não existe");
            }

            var sinonimo = await _sinonimoRepository.ObterPorIdAsync(request.SinonimoId);
            if (sinonimo is null)
            {
                return Result.Failure("O sinônimo de id " + request.SinonimoId + " não existe");
            }

            if (sinonimo.EstruturaClausulaId != estruturaClausula.Id)
            {
                return Result.Failure("Sinônimo com classificação inconsistente");
            }

            var iaClausulaAtualizarResult = iaClausula.Atualizar(
                request.Texto,
                iaClausula.IADocumentoSindicalId,
                request.EstruturaClausulaId,
                request.Numero,
                request.SinonimoId,
                IAClausulaStatus.Consistente
            );

            if (iaClausulaAtualizarResult.IsFailure) return iaClausulaAtualizarResult;

            await _iaclausulaRepository.AtualizarAsync(iaClausula);

            await CommitAsync(cancellationToken);

            return Result.Success();
        }
    }
}
