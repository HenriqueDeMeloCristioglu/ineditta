using CSharpFunctionalExtensions;

using Ineditta.Application.Acompanhamentos.CctsSindicatosLaborais.Entities;
using Ineditta.Application.Acompanhamentos.CctsSindicatosLaborais.UseCases.Deletar;

using MediatR;

namespace Ineditta.Application.Acompanhamentos.Ccts.Services.AcompanhamentosCctsSindicatosLaboraisServices
{
    public class DeletarAcompanhamentoSindicatoLaboralService
    {
        private readonly IMediator _mediator;

        public DeletarAcompanhamentoSindicatoLaboralService(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<Result> Deletar(IEnumerable<AcompanhamentoCctSinditoLaboral> acompanhamentoSindicatosLaboraisIds, CancellationToken cancellationToken)
        {
            foreach (var acompanhamentosSindicatoLaboralParaDeletarItem in acompanhamentoSindicatosLaboraisIds)
            {
                var deletarAcompanhamentoCctSindicatoLaboralRequest = new DeletarAcompanhamentoCctSindicatoLaboralRequest
                {
                    AcompanhamentoCctSinditoLaboral = acompanhamentosSindicatoLaboralParaDeletarItem
                };

                var reusltDeletarAcompanhamentoSindicatoLaboral = await _mediator.Send(deletarAcompanhamentoCctSindicatoLaboralRequest, cancellationToken);

                if (reusltDeletarAcompanhamentoSindicatoLaboral.IsFailure)
                {
                    return Result.Failure("Não foi possível deletar sindicatos laborais");
                }
            }

            return Result.Success();
        }
    }
}
