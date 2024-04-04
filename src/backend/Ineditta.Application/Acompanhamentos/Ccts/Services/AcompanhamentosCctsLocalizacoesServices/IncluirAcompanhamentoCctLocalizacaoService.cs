using CSharpFunctionalExtensions;

using Ineditta.Application.Acompanhamentos.CctsLocalizacoes.UseCases.Upsert;

using MediatR;

namespace Ineditta.Application.Acompanhamentos.Ccts.Services.AcompanhamentosCctsLocalizacoesServices
{
    public class IncluirAcompanhamentoCctLocalizacaoService
    {
        private readonly IMediator _mediator;

        public IncluirAcompanhamentoCctLocalizacaoService(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<Result> Incluir(IEnumerable<int> localizaceosId, long acompanhamentoCctId, CancellationToken cancellationToken)
        {
            foreach (var localizacaoId in localizaceosId)
            {
                var upsertAcompanhamentoCctLocalizacaoRequest = new IncluirAcompanhamentoCctLocalizacaoRequest
                {
                    LocalizacaoId = localizacaoId,
                    AcompanhamentoCctId = acompanhamentoCctId
                };

                var reusltUpsertAcompanhamentoLocalizacao = await _mediator.Send(upsertAcompanhamentoCctLocalizacaoRequest, cancellationToken);

                if (reusltUpsertAcompanhamentoLocalizacao.IsFailure)
                {
                    return Result.Failure("Não foi possível cadastrar localizações");
                }
            }

            return Result.Success();
        }
    }
}
