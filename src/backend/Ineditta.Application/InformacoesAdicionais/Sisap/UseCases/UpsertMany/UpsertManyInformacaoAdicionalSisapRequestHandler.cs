using CSharpFunctionalExtensions;

using Ineditta.Application.InformacoesAdicionais.Cliente.Entities;
using Ineditta.Application.InformacoesAdicionais.Sisap.Entities;
using Ineditta.Application.InformacoesAdicionais.Sisap.Repositiories;
using Ineditta.BuildingBlocks.Core.Database;
using Ineditta.BuildingBlocks.Core.Domain.Handlers;

using MediatR;

namespace Ineditta.Application.InformacoesAdicionais.Sisap.UseCases.UpsertMany
{
    public class UpsertManyInformacaoAdicionalSisapRequestHandler : BaseCommandHandler, IRequestHandler<UpsertManyInformacaoAdicionalSisapRequest, Result>
    {
        private readonly IInformacaoAdicionalSisapRepository _informacaoAdidionalSisapRepository;

        public UpsertManyInformacaoAdicionalSisapRequestHandler(IUnitOfWork unitOfWork, IInformacaoAdicionalSisapRepository informacaoAdidionalSisapRepository) : base(unitOfWork)
        {
            _informacaoAdidionalSisapRepository = informacaoAdidionalSisapRepository;
        }

        public async Task<Result> Handle(UpsertManyInformacaoAdicionalSisapRequest request, CancellationToken cancellationToken)
        {
            return request.ClausulaId > 0 ? await Atualizar(request, cancellationToken) : await Incluir(request, cancellationToken);
        }

        public async Task<Result> Incluir(UpsertManyInformacaoAdicionalSisapRequest request, CancellationToken cancellationToken)
        {
            List<InformacaoAdicionalSisap> informacoesAdicionaisSisap = new();

            foreach (var informacaoAdicional in request.InformacoesAdicionais)
            {
                var informacaoAdicionalSisap = InformacaoAdicionalSisap.Criar(informacaoAdicional.DocumentoSindicalId, informacaoAdicional.ClausulaGeralId, informacaoAdicional.EstruturaClausulaId, informacaoAdicional.NomeInformacaoEstruturaClausulaId, informacaoAdicional.TipoinformacaoadicionalId, informacaoAdicional.InforamcacaoAdicionalGrupoId, informacaoAdicional.SequenciaItem, informacaoAdicional.SequenciaLinha, informacaoAdicional.Texto, informacaoAdicional.Numerico, informacaoAdicional.Descricao, informacaoAdicional.Data, informacaoAdicional.Percentual, informacaoAdicional.Hora, informacaoAdicional.Combo);

                if (informacaoAdicionalSisap.IsFailure)
                {
                    return informacaoAdicionalSisap;
                }

                informacoesAdicionaisSisap.Add(informacaoAdicionalSisap.Value);
            }

            await _informacaoAdidionalSisapRepository.IncluirMuitosAsync(informacoesAdicionaisSisap);

            await CommitAsync(cancellationToken);

            return Result.Success();
        }

        public async Task<Result> Atualizar(UpsertManyInformacaoAdicionalSisapRequest request, CancellationToken cancellationToken)
        {
            var existingInfos = await _informacaoAdidionalSisapRepository.ObterPorClausulaId(request.ClausulaId);

            if (existingInfos == null)
            {
                return Result.Failure("Informacoes adicionais desta cl�usula n�o encontrada");
            }

            foreach (var informacaoAdicional in existingInfos)
            {
                if (!request.InformacoesAdicionais.Any(informacaoAdicionalRequest => informacaoAdicional.Id == informacaoAdicionalRequest.Id))
                {
                    await _informacaoAdidionalSisapRepository.ExcluirAsync(informacaoAdicional);
                }
            }

            foreach (var info in request.InformacoesAdicionais)
            {
                var existingInfo = existingInfos.Find(e => e.Id == info.Id);

                if (existingInfo != null)
                {
                    existingInfo.Atualizar(info.DocumentoSindicalId, request.ClausulaId, info.EstruturaClausulaId, info.NomeInformacaoEstruturaClausulaId, info.TipoinformacaoadicionalId, info.InforamcacaoAdicionalGrupoId, info.SequenciaItem, info.SequenciaLinha, info.Texto, info.Numerico, info.Descricao, info.Data, info.Percentual, info.Hora, info.Combo);

                    await _informacaoAdidionalSisapRepository.AtualizarAsync(existingInfo);
                }
                else
                {
                    var informacaoAdicionalSisap = InformacaoAdicionalSisap.Criar(info.DocumentoSindicalId, info.ClausulaGeralId, info.EstruturaClausulaId, info.NomeInformacaoEstruturaClausulaId, info.TipoinformacaoadicionalId, info.InforamcacaoAdicionalGrupoId, info.SequenciaItem, info.SequenciaLinha, info.Texto, info.Numerico, info.Descricao, info.Data, info.Percentual, info.Hora, info.Combo);

                    if (informacaoAdicionalSisap.IsFailure)
                    {
                        return informacaoAdicionalSisap;
                    }

                    await _informacaoAdidionalSisapRepository.IncluirAsync(informacaoAdicionalSisap.Value);
                }
            }
            await CommitAsync(cancellationToken);

            return Result.Success();
        }
    }
}
