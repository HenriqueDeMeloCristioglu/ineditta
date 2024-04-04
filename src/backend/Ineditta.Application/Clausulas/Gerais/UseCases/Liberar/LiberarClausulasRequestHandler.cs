using CSharpFunctionalExtensions;

using Ineditta.Application.Clausulas.Gerais.Repositiories;
using Ineditta.BuildingBlocks.Core.Database;
using Ineditta.BuildingBlocks.Core.Domain.Handlers;

using MediatR;

namespace Ineditta.Application.Clausulas.Gerais.UseCases.Liberar
{
    public class LiberarClausulasRequestHandler : BaseCommandHandler, IRequestHandler<LiberarClausulasRequest, Result>
    {
        private readonly IClausulaGeralRepository _clausulaGeralRepository;
        public LiberarClausulasRequestHandler(IUnitOfWork unitOfWork, IClausulaGeralRepository clausulaGeralRepository) : base(unitOfWork)
        {
            _clausulaGeralRepository = clausulaGeralRepository;
        }

        public async Task<Result> Handle(LiberarClausulasRequest request, CancellationToken cancellationToken)
        {
            var clausulas = await _clausulaGeralRepository.ObterTodasPorDocumentoId(request.DocumentoId);

            if (clausulas == null)
            {
                return Result.Failure("Clausulas não encontradas");
            }

            foreach (var clausula in clausulas)
            {
                var result = clausula.Liberar();

                if (result.IsFailure)
                {
                    return result;
                }

                await _clausulaGeralRepository.AtualizarAsync(clausula);
            }

            await CommitAsync(cancellationToken);

            return Result.Success();
        }
    }
}
