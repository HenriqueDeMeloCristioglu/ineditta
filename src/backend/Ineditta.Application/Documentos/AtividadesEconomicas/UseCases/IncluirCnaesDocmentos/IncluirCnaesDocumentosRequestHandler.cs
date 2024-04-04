using CSharpFunctionalExtensions;

using Ineditta.Application.Documentos.AtividadesEconomicas.Repositories;
using Ineditta.BuildingBlocks.Core.Database;
using Ineditta.BuildingBlocks.Core.Domain.Handlers;
using Ineditta.BuildingBlocks.Core.Domain.Models;

using MediatR;

namespace Ineditta.Application.Documentos.AtividadesEconomicas.UseCases.IncluirCnaesDocmentos
{
    public class IncluirCnaesDocumentosRequestHandler : BaseCommandHandler, IRequestHandler<IncluirCnaesDocumentosRequest, Result<Unit, Error>>
    {
        private readonly IDocumentoAtividadeEconomicaRepository _documentoAtividadeEconomicaRepository;
        public IncluirCnaesDocumentosRequestHandler(IUnitOfWork unitOfWork, IDocumentoAtividadeEconomicaRepository documentoAtividadeEconomicaRepository) : base(unitOfWork)
        {
            _documentoAtividadeEconomicaRepository = documentoAtividadeEconomicaRepository;
        }

        public async Task<Result<Unit, Error>> Handle(IncluirCnaesDocumentosRequest request, CancellationToken cancellationToken)
        {
            var result = await _documentoAtividadeEconomicaRepository.IncluirAtividadeEconomicaDocumento();

            if (result.IsFailure)
            {
                return Result.Failure<Unit, Error>(Error.Create("500", "Erro ao incluir cnaes documentos"));
            }

            return Result.Success<Unit, Error>(Unit.Value);
        }
    }
}
