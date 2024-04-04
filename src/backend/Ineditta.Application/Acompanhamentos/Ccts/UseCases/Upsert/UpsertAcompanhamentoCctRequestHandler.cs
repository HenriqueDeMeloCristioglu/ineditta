using CSharpFunctionalExtensions;

using Ineditta.Application.Acompanhamentos.Ccts.Entities;
using Ineditta.Application.Acompanhamentos.Ccts.Factories;
using Ineditta.Application.Acompanhamentos.Ccts.Repositories;
using Ineditta.Application.Acompanhamentos.Ccts.Services.AcompanhamentosCctsLocalizacoesServices;
using Ineditta.Application.Acompanhamentos.Ccts.Services.AcompanhamentosCctsSindicatosLaboraisServices;
using Ineditta.BuildingBlocks.Core.Database;
using Ineditta.BuildingBlocks.Core.Domain.Handlers;

using MediatR;

namespace Ineditta.Application.Acompanhamentos.Ccts.UseCases.Upsert
{
    public class UpsertAcompanhamentoCctRequestHandler : BaseCommandHandler, IRequestHandler<UpsertAcompanhamentoCctRequest, Result>
    {
        private readonly IAcompanhamentoCctRepository _acompanhamentoCctRepository;
        private readonly IncluirAcompanhamentoCctSindicatoLaboralService _incluirAcompanhamentoCctSindicatoLaboralService;
        private readonly IncluirAcompanhamentoCctSindicatoPatronalService _incluirAcompanhamentoCctSindicatoPatronalService;
        private readonly IncluirAcompanhamentoCctLocalizacaoService _incluirAcompanhamentoCctLocalizacaoService;
        private readonly AtualizarAcompanhamentoCctLocalizacaoService _atualizarAcompanhamentoCctLocalizacaoService;
        private readonly AtualizarAcompanhamentoCctSindicatoPatronalService _atualizarAcompanhamentoCctSindicatoPatronalService;
        private readonly AtualizarAcompanhamentoSindicatoLaboralService _atualizarAcompanhamentoSindicatoLaboralService;
        private readonly AcompanhamentoCctAssuntoFactory _acompanhamentoCctAssuntoFactory;
        private readonly AcompanhamentoCctEtiquetaFactory _acompanhamentoCctEtiquetaFactory;

        public UpsertAcompanhamentoCctRequestHandler(IUnitOfWork unitOfWork, IAcompanhamentoCctRepository acompanhamentoCctSindicatoLaboral, IncluirAcompanhamentoCctSindicatoLaboralService incluirAcompanhamentoCctSindicatoLaboralService, IncluirAcompanhamentoCctSindicatoPatronalService incluirAcompanhamentoCctSindicatoPatronalService, IncluirAcompanhamentoCctLocalizacaoService incluirAcompanhamentoCctLocalizacaoService, AtualizarAcompanhamentoCctLocalizacaoService atualizarAcompanhamentoCctLocalizacaoService, AtualizarAcompanhamentoCctSindicatoPatronalService atualizarAcompanhamentoCctSindicatoPatronalService, AtualizarAcompanhamentoSindicatoLaboralService atualizarAcompanhamentoSindicatoLaboralService, AcompanhamentoCctAssuntoFactory acompanhamentoCctAssuntoFactory, AcompanhamentoCctEtiquetaFactory acompanhamentoCctEtiquetaFactory) : base(unitOfWork)
        {
            _acompanhamentoCctRepository = acompanhamentoCctSindicatoLaboral;
            _incluirAcompanhamentoCctSindicatoLaboralService = incluirAcompanhamentoCctSindicatoLaboralService;
            _incluirAcompanhamentoCctSindicatoPatronalService = incluirAcompanhamentoCctSindicatoPatronalService;
            _incluirAcompanhamentoCctLocalizacaoService = incluirAcompanhamentoCctLocalizacaoService;
            _atualizarAcompanhamentoCctLocalizacaoService = atualizarAcompanhamentoCctLocalizacaoService;
            _atualizarAcompanhamentoCctSindicatoPatronalService = atualizarAcompanhamentoCctSindicatoPatronalService;
            _atualizarAcompanhamentoSindicatoLaboralService = atualizarAcompanhamentoSindicatoLaboralService;
            _acompanhamentoCctAssuntoFactory = acompanhamentoCctAssuntoFactory;
            _acompanhamentoCctEtiquetaFactory = acompanhamentoCctEtiquetaFactory;
        }

        public async Task<Result> Handle(UpsertAcompanhamentoCctRequest request, CancellationToken cancellationToken)
        {
            return request.Id > 0 ? await Atualizar(request, cancellationToken) : await Incluir(request, cancellationToken);
        }

        public async Task<Result> Incluir(UpsertAcompanhamentoCctRequest request, CancellationToken cancellationToken)
        {
            var assuntos = await _acompanhamentoCctAssuntoFactory.Criar(request.AssuntosIds);
            var etiquetas = await _acompanhamentoCctEtiquetaFactory.Criar(request.EtiquetasIds);

            var acompanhamento = AcompanhamentoCct.Criar(request.DataInicial, request.DataFinal, request.ProximaLigacao, request.StatusId, request.UsuarioResponsavelId, request.FaseId, request.DataBase, request.CnaesIds, request.EmpresasIds, request.GruposEconomicosIds, request.TipoDocumentoId, request.ObservacoesGerais, request.Anotacoes, request.DataProcessamento, assuntos.ToList(), etiquetas.ToList(), request.ValidadeFinal);

            if (acompanhamento.IsFailure)
            {
                return acompanhamento;
            }

            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                await _acompanhamentoCctRepository.IncluirAsync(acompanhamento.Value);

                await CommitAsync(cancellationToken);

                if (request.SindicatosLaboraisIds is not null && request.SindicatosLaboraisIds.Any())
                {
                    var resultIncluirSindicatosLaborais = await _incluirAcompanhamentoCctSindicatoLaboralService.Incluir(request.SindicatosLaboraisIds, acompanhamento.Value.Id, cancellationToken);

                    if (resultIncluirSindicatosLaborais.IsFailure)
                    {
                        return resultIncluirSindicatosLaborais;
                    }
                }

                if (request.SindicatosPatronaisIds is not null && request.SindicatosPatronaisIds.Any())
                {
                    var resultIncluirSindicatosPatronais = await _incluirAcompanhamentoCctSindicatoPatronalService.Incluir(request.SindicatosPatronaisIds, acompanhamento.Value.Id, cancellationToken);

                    if (resultIncluirSindicatosPatronais.IsFailure)
                    {
                        return resultIncluirSindicatosPatronais;
                    }
                }

                if (request.LocalizacoesIds is not null && request.LocalizacoesIds.Any())
                {
                    var resultIncluirLocalizacoes = await _incluirAcompanhamentoCctLocalizacaoService.Incluir(request.LocalizacoesIds, acompanhamento.Value.Id, cancellationToken);

                    if (resultIncluirLocalizacoes.IsFailure)
                    {
                        return resultIncluirLocalizacoes;
                    }
                }

                return Result.Success();
            }, cancellationToken);

            return Result.Success();
        }

        public async Task<Result> Atualizar(UpsertAcompanhamentoCctRequest request, CancellationToken cancellationToken)
        {
            var acompanhamentoCct = await _acompanhamentoCctRepository.ObterPorIdAsync(request.Id);

            if (acompanhamentoCct == null)
            {
                return Result.Failure("Acompanhamento não encontrado");
            }

            var assuntos = await _acompanhamentoCctAssuntoFactory.Criar(request.AssuntosIds);
            var etiquetas = await _acompanhamentoCctEtiquetaFactory.Criar(request.EtiquetasIds);

            var result = acompanhamentoCct.Atualizar(request.DataInicial, request.DataFinal, request.StatusId, request.UsuarioResponsavelId, request.FaseId, request.DataBase, request.CnaesIds, request.EmpresasIds, request.GruposEconomicosIds, request.TipoDocumentoId, request.ObservacoesGerais, request.Anotacoes, request.DataProcessamento, assuntos.ToList(), etiquetas.ToList(), request.ValidadeFinal);

            if (result.IsFailure)
            {
                return result;
            }

            await _acompanhamentoCctRepository.AtualizarAsync(acompanhamentoCct);

            await CommitAsync(cancellationToken);

            var resultAtualizarAcompanhamentoSindicatoLaboral = await _atualizarAcompanhamentoSindicatoLaboralService.Atualizar(request.Id, request.SindicatosLaboraisIds, cancellationToken);

            if (resultAtualizarAcompanhamentoSindicatoLaboral.IsFailure)
            {
                return resultAtualizarAcompanhamentoSindicatoLaboral;
            }

            var resultAtualizarAcompanhamentoCctSindicatoPatronal = await _atualizarAcompanhamentoCctSindicatoPatronalService.Atualizar(request.Id, request.SindicatosPatronaisIds, cancellationToken);

            if (resultAtualizarAcompanhamentoCctSindicatoPatronal.IsFailure)
            {
                return resultAtualizarAcompanhamentoCctSindicatoPatronal;
            }

            var resultAtualizarAcompanhamentoCctLocalizacao = await _atualizarAcompanhamentoCctLocalizacaoService.Atualizar(request.Id, request.LocalizacoesIds, cancellationToken);

            if (resultAtualizarAcompanhamentoCctLocalizacao.IsFailure)
            {
                return resultAtualizarAcompanhamentoCctLocalizacao;
            }

            return Result.Success();
        }
    }
}
