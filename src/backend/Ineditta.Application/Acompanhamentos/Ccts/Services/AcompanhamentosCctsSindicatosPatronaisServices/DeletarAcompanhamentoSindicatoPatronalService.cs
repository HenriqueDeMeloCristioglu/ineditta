using CSharpFunctionalExtensions;

using Ineditta.Application.Acompanhamentos.CctsSindicatosPatronais.Entities;
using Ineditta.Application.Acompanhamentos.CctsSindicatosPatronais.UseCases.Deletar;

using MediatR;

namespace Ineditta.Application.Acompanhamentos.Ccts.Services.AcompanhamentosCctsSindicatosPatronaisServices
{
    public class DeletarAcompanhamentoSindicatoPatronalService
    {
        private readonly IMediator _mediator;

        public DeletarAcompanhamentoSindicatoPatronalService(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<Result> Deletar(IEnumerable<AcompanhamentoCctSinditoPatronal> acompanhamentoSindicatosPatronaisIds, CancellationToken cancellationToken)
        {
            foreach (var acompanhamentoSindicatoPatronal in acompanhamentoSindicatosPatronaisIds)
            {
                var deletarAcompanhamentoCctSindicatoPatronalRequest = new DeletarAcompanhamentoCctSindicatoPatronalRequest
                {
                    AcompanhamentoCctSinditoPatronal = acompanhamentoSindicatoPatronal
                };

                var reusltDeletarAcompanhamentoSindicatoPatronal = await _mediator.Send(deletarAcompanhamentoCctSindicatoPatronalRequest, cancellationToken);

                if (reusltDeletarAcompanhamentoSindicatoPatronal.IsFailure)
                {
                    return Result.Failure("Não foi possível deletar sindicatos patronais");
                }
            }

            return Result.Success();
        }
    }
}
