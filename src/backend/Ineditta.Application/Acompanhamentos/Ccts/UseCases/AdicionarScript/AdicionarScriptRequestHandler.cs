using CSharpFunctionalExtensions;

using Ineditta.Application.Acompanhamentos.Ccts.Repositories;
using Ineditta.Application.Acompanhamentos.Ccts.Services.AcompanhamentosCctsEventos;
using Ineditta.Application.Acompanhamentos.Ccts.Services.AcompanhamentosCctsEventosServices;
using Ineditta.Application.Acompanhamentos.CctsFases.Repositories;
using Ineditta.Application.AcompanhamentosCcts.Entities;
using Ineditta.Application.Usuarios.Factories;
using Ineditta.BuildingBlocks.Core.Database;
using Ineditta.BuildingBlocks.Core.Domain.Handlers;

using MediatR;

namespace Ineditta.Application.Acompanhamentos.Ccts.UseCases.AdicionarScript
{
    public class AdicionarScriptRequestHandler : BaseCommandHandler, IRequestHandler<AdicionarScriptRequest, Result>
    {
        private readonly IAcompanhamentoCctRepository _acompanhamentoCctRepository;
        private readonly ObterUsuarioLogadoFactory _obterUsuarioLogadoFactory;
        private readonly IFaseCctRepository _fasesCctsRepository;
        private readonly AcompanhamentoCctEventoAssembleiaPatronalService _acompanhamentoCctEventoAssembleiaPatronalService;
        private readonly AcompanhamentoCctEventoReuniaoEntreAsPartesService _acompanhamentoCctEventoReuniaoEntreAsPartesService;

        public AdicionarScriptRequestHandler(IUnitOfWork unitOfWork, IAcompanhamentoCctRepository acompanhamentoCctRepository, ObterUsuarioLogadoFactory obterUsuarioLogadoFactory, IFaseCctRepository fasesCctsRepository, AcompanhamentoCctEventoAssembleiaPatronalService acompanhamentoCctEventoAssembleiaPatronalService, AcompanhamentoCctEventoReuniaoEntreAsPartesService acompanhamentoCctEventoReuniaoEntreAsPartesService) : base(unitOfWork)
        {
            _acompanhamentoCctRepository = acompanhamentoCctRepository;
            _obterUsuarioLogadoFactory = obterUsuarioLogadoFactory;
            _fasesCctsRepository = fasesCctsRepository;
            _acompanhamentoCctEventoAssembleiaPatronalService = acompanhamentoCctEventoAssembleiaPatronalService;
            _acompanhamentoCctEventoReuniaoEntreAsPartesService = acompanhamentoCctEventoReuniaoEntreAsPartesService;
        }

        public async Task<Result> Handle(AdicionarScriptRequest request, CancellationToken cancellationToken)
        {
            var usuario = await _obterUsuarioLogadoFactory.PorEmail();

            if (usuario.IsFailure)
            {
                return usuario;
            }

            var acompanhamentos = await _acompanhamentoCctRepository.ObterPorIdsAsync(request.AcompanhamentosCctsIds);

            if (acompanhamentos is null || !acompanhamentos.Any())
            {
                return Result.Failure("Acompanhamento Cct não encontrado");
            }

            var faseCct = await _fasesCctsRepository.ObterPorIdAsync(request.FaseId);

            if (faseCct == null)
            {
                return Result.Failure("Fase não encontrada");
            }

            var novoScript = Script.Gerar(
                faseCct.Fase,
                request.Respostas,
                DateTime.Now,
                usuario.Value.Nome
            );

            if (novoScript.IsFailure)
            {
                return Result.Failure("Erro ao criar script");
            }

            foreach (var acompanhamento in acompanhamentos)
            {
                var result = acompanhamento.IncluirScript(novoScript.Value, request.Respostas, request.StatusId);

                if (result.IsFailure)
                {
                    return result;
                }

                await _acompanhamentoCctRepository.AtualizarAsync(acompanhamento);

                await CommitAsync(cancellationToken);

                var resultCriarEventoAssembleiaPatronal = await _acompanhamentoCctEventoAssembleiaPatronalService.Criar(acompanhamento.Id, request.Respostas, request.FaseId, cancellationToken);

                if (resultCriarEventoAssembleiaPatronal.IsFailure)
                {
                    return resultCriarEventoAssembleiaPatronal;
                }

                var resultCriarEventoReuniaoEntreAsPartes = await _acompanhamentoCctEventoReuniaoEntreAsPartesService.Criar(acompanhamento.Id, request.Respostas, request.FaseId, cancellationToken);

                if (resultCriarEventoReuniaoEntreAsPartes.IsFailure)
                {
                    return resultCriarEventoReuniaoEntreAsPartes;
                }
            }

            return Result.Success();
        }
    }
}
