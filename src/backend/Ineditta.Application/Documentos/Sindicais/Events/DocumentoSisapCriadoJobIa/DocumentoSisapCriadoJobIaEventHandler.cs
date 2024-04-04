using CSharpFunctionalExtensions;

using Ineditta.Application.AIs.DocumentosSindicais.Entities;
using Ineditta.Application.AIs.DocumentosSindicais.Repositories;
using Ineditta.Application.AIs.DocumentosSindicais.UseCases.Upsert;
using Ineditta.BuildingBlocks.Core.Bus;

using MediatR;

namespace Ineditta.Application.Documentos.Sindicais.Events.DocumentoSisapCriadoJobIa
{
#pragma warning disable CA1711 // Identifiers should not have incorrect suffix
    public class DocumentoSisapCriadoJobIaEventHandler : BuildingBlocks.Core.Bus.IRequestHandler<DocumentoSisapCriadoJobIaEvent>
    {
        private readonly IMediator _mediator;

        public DocumentoSisapCriadoJobIaEventHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async ValueTask<Result> Handle(DocumentoSisapCriadoJobIaEvent message, CancellationToken cancellationToken = default)
        {
            var requestUpsert = new UpsertDocumentoSindicalIARequest
            {
                DocumentoReferenciaId = message.DocumentoId,
                Status = IADocumentoStatus.AguardandoProcessamento
            };

            var result = await _mediator.Send(requestUpsert, cancellationToken);

            if (result.IsFailure) throw new ArgumentException(result.Error);

            return Result.Success();
        }
    }
}
