using CSharpFunctionalExtensions;

using Ineditta.Application.Acompanhamentos.CctsLocalizacoes.Entities;
using Ineditta.Application.Acompanhamentos.CctsLocalizacoes.UseCases.Deletar;

using MediatR;

namespace Ineditta.Application.Acompanhamentos.Ccts.Services.AcompanhamentosCctsLocalizacoesServices
{
    public class DeletarAcompanhamentoCctLocalizacaoService
    {
        private readonly IMediator _mediator;

        public DeletarAcompanhamentoCctLocalizacaoService(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<Result> Deletar(IEnumerable<AcompanhamentoCctLocalizacao> localizacoes, CancellationToken cancellationToken)
        {
            foreach (var localizacao in localizacoes)
            {
                var deletarAcompanhamentoCctLocalizacaoRequest = new DeletarAcompanhamentoCctLocalizacaoRequest
                {
                    AcompanhamentoCctLocalizacao = localizacao
                };

                var reusltDeletarAcompanhamentoLocalizacao = await _mediator.Send(deletarAcompanhamentoCctLocalizacaoRequest, cancellationToken);

                if (reusltDeletarAcompanhamentoLocalizacao.IsFailure)
                {
                    return Result.Failure("Não foi possível deletar localizações");
                }
            }

            return Result.Success();
        }
    }
}
