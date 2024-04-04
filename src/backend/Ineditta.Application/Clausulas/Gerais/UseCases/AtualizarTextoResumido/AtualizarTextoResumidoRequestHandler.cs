using CSharpFunctionalExtensions;

using Ineditta.Application.Clausulas.Gerais.Repositiories;
using Ineditta.BuildingBlocks.Core.Database;
using Ineditta.BuildingBlocks.Core.Domain.Handlers;

using MediatR;

namespace Ineditta.Application.Clausulas.Gerais.UseCases.AtualizarTextoResumido
{
    public class AtualizarTextoResumidoRequestHandler : BaseCommandHandler, IRequestHandler<AtualizarTextoResumidoRequest, Result>
    {
        private readonly IClausulaGeralRepository _clausulaGeralRepository;
        public AtualizarTextoResumidoRequestHandler(IUnitOfWork unitOfWork, IClausulaGeralRepository clausulaGeralRepository) : base(unitOfWork)
        {
            _clausulaGeralRepository = clausulaGeralRepository;
        }

        public async Task<Result> Handle(AtualizarTextoResumidoRequest request, CancellationToken cancellationToken)
        {
            var clausulas = await _clausulaGeralRepository.ObterPorDocumentoEstruturaId(request.EstruturaId, request.DocumentoId);
            if (clausulas == null)
            {
                return Result.Failure("Cláusula não encontrada");
            }

            foreach (var clausula in clausulas)
            {
                var resultReumir = clausula.AtualizarResumo(request.Texto);
                if (resultReumir.IsFailure)
                {
                    return resultReumir;
                }

                await _clausulaGeralRepository.AtualizarAsync(clausula);

                await CommitAsync(cancellationToken);
            }

            return Result.Success();
        }
    }
}
