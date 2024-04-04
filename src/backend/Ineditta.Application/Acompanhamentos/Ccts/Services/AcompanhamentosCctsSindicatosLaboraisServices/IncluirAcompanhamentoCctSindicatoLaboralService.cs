using CSharpFunctionalExtensions;

using Ineditta.Application.Acompanhamentos.CctsSindicatosLaborais.UseCases.Incluir;

using MediatR;

namespace Ineditta.Application.Acompanhamentos.Ccts.Services.AcompanhamentosCctsSindicatosLaboraisServices
{
    public class IncluirAcompanhamentoCctSindicatoLaboralService
    {
        private readonly IMediator _mediator;

        public IncluirAcompanhamentoCctSindicatoLaboralService(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<Result> Incluir(IEnumerable<int> sindicatosLaboraisIds, long acompanhamentoCctId, CancellationToken cancellationToken)
        {
            foreach (var sindicatoLaboralId in sindicatosLaboraisIds)
            {
                var upsertAcompanhamentoCctSindicatoLaboralRequest = new IncluirAcompanhamentoCctSindicatoLaboralRequest
                {
                    SindicatoId = sindicatoLaboralId,
                    AcompanhamentoCctId = acompanhamentoCctId
                };

                var reusltUpsertAcompanhamentoSindicatoLaboral = await _mediator.Send(upsertAcompanhamentoCctSindicatoLaboralRequest, cancellationToken);

                if (reusltUpsertAcompanhamentoSindicatoLaboral.IsFailure)
                {
                    return Result.Failure("Não foi possível cadastrar sindicatos laborais");
                }
            }

            return Result.Success();
        }
    }
}
