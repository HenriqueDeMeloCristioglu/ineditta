using CSharpFunctionalExtensions;

using Ineditta.Application.Acompanhamentos.CctsSindicatosPatronais.UseCases.Incluir;

using MediatR;

namespace Ineditta.Application.Acompanhamentos.Ccts.Services.AcompanhamentosCctsSindicatosLaboraisServices
{
    public class IncluirAcompanhamentoCctSindicatoPatronalService
    {
        private readonly IMediator _mediator;

        public IncluirAcompanhamentoCctSindicatoPatronalService(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<Result> Incluir(IEnumerable<int> sindicatosPatronaisIds, long acompanhamentoCctId, CancellationToken cancellationToken)
        {
            foreach (var sindicatoPatronalId in sindicatosPatronaisIds)
            {
                var upsertAcompanhamentoCctSindicatoPatronalRequest = new IncluirAcompanhamentoCctSindicatoPatronalRequest
                {
                    SindicatoId = sindicatoPatronalId,
                    AcompanhamentoCctId = acompanhamentoCctId
                };

                var reusltUpsertAcompanhamentoSindicatoPatronal = await _mediator.Send(upsertAcompanhamentoCctSindicatoPatronalRequest, cancellationToken);

                if (reusltUpsertAcompanhamentoSindicatoPatronal.IsFailure)
                {
                    return Result.Failure("Não foi possível cadastrar sindicatos patronais");
                }
            }

            return Result.Success();
        }
    }
}
