using CSharpFunctionalExtensions;

using Ineditta.Application.Clausulas.Gerais.Dtos.ResumirClausulas;
using Ineditta.Application.Clausulas.Gerais.Factories;
using Ineditta.Application.Clausulas.Gerais.Repositiories;
using Ineditta.Application.Clausulas.Gerais.Services.ResumirClausulas;
using Ineditta.Application.Clausulas.Gerais.UseCases.GeraNovasClausulas;
using Ineditta.BuildingBlocks.Core.Database;

using MediatR;

namespace Ineditta.Application.Clausulas.Gerais.Events.ResumirClausulasEvent
{
#pragma warning disable CA1711 // Identifiers should not have incorrect suffix
    public class ResumirClausulaEventHandler : BuildingBlocks.Core.Bus.IRequestHandler<ResumirClausulasEvent>
#pragma warning restore CA1711 // Identifiers should not have incorrect suffix
    {
        private readonly IClausulaGeralRepository _clausulaGeralRepository;
        private readonly IResumirClausulas _resumirClausulas;
        private readonly IMediator _mediator;
        private readonly IUnitOfWork _unitOfWork;

        public ResumirClausulaEventHandler(IClausulaGeralRepository clausulaGeralRepository, IResumirClausulas resumirClausulas, IMediator mediator, IUnitOfWork unitOfWork)
        {
            _clausulaGeralRepository = clausulaGeralRepository;
            _resumirClausulas = resumirClausulas;
            _mediator = mediator;
            _unitOfWork = unitOfWork;
        }

        public async ValueTask<Result> Handle(ResumirClausulasEvent message, CancellationToken cancellationToken = default)
        {
            var result = await _clausulaGeralRepository.ObterTodasPorEmpresaDocumentoId(message.DocumentoId);

            if (result == null)
            {
#pragma warning disable S112 // General exceptions should never be thrown
                throw new Exception("Nenhuma cláusula encontrada para ser resumida");
#pragma warning restore S112 // General exceptions should never be thrown
            }

            var clausulasClassificadas = ClassificarClausulasPorEstruturaFactory.Criar(result);
            if (clausulasClassificadas.IsFailure)
            {
                return clausulasClassificadas;
            }

            foreach (var classificacaoItem in clausulasClassificadas.Value)
            {
                if (classificacaoItem[0] is null)
                {
                    continue;
                }

                var resumirClausulasRequest = new ResumirClausulasServiceRequest
                {
                    Textos = classificacaoItem.Select(t => t.Texto),
                    InstrucoesIA = classificacaoItem[0].InstrucaoIa,
                    MaximoPalavrasIA = classificacaoItem[0].MaximoPalavrasIa
                };

                var resumo = await _resumirClausulas.Criar(resumirClausulasRequest);

                if (resumo.IsFailure)
                {
                    return resumo;
                }

                foreach (var cf in classificacaoItem)
                {
                    var clausula = await _clausulaGeralRepository.ObterPorId(cf.Id);

                    if (clausula != null)
                    {
                        var resultResumir = clausula.AtualizarResumo(resumo.Value.Texto);
                        if (resultResumir.IsFailure)
                        {
                            return resultResumir;
                        }

                        await _clausulaGeralRepository.AtualizarAsync(clausula);

                        await _unitOfWork.CommitAsync(cancellationToken);
                    }
                }
            }

            var resultGerarNovasClausulas = await _mediator.Send(new GeraNovasClausulasRequest
            {
                DocumentoId = message.DocumentoId,
                UsuarioId = message.UsuarioId
            }, cancellationToken);

            if (resultGerarNovasClausulas.IsFailure)
            {
                return resultGerarNovasClausulas;
            }

            return Result.Success();
        }
    }
}
