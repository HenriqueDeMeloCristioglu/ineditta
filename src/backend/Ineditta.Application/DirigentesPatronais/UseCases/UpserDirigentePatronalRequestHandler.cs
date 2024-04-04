using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CSharpFunctionalExtensions;

using Ineditta.Application.ClientesUnidades.Repositories;
using Ineditta.Application.DirigentesPatronais.Entities;
using Ineditta.Application.DirigentesPatronais.Repositories;
using Ineditta.Application.Sindicatos.Patronais.Repositories;
using Ineditta.BuildingBlocks.Core.Bus;
using Ineditta.BuildingBlocks.Core.Database;
using Ineditta.BuildingBlocks.Core.Domain.Handlers;

using MediatR;

namespace Ineditta.Application.DirigentesPatronais.UseCases
{
    public class UpserDirigentePatronalRequestHandler : BaseCommandHandler, IRequestHandler<UpsertDirigentePatronalRequest, Result>
    {
        private readonly IDirigentePatronalRepository _dirigentePatronalRepository;
        private readonly ISindicatoPatronalRepository _sindicatoPatronalRepository;
        private readonly IClienteUnidadeRepository _clienteUnidadeRepository;

        public UpserDirigentePatronalRequestHandler(IUnitOfWork unitOfWork, IDirigentePatronalRepository dirigentePatronalRepository, ISindicatoPatronalRepository sindicatoPatronalRepository, IClienteUnidadeRepository clienteUnidadeRepository) : base(unitOfWork)
        {
            _dirigentePatronalRepository = dirigentePatronalRepository;
            _sindicatoPatronalRepository = sindicatoPatronalRepository;
            _clienteUnidadeRepository = clienteUnidadeRepository;
        }

        public async Task<Result> Handle(UpsertDirigentePatronalRequest request, CancellationToken cancellationToken)
        {
            return request.Id is null ?
                await Inserir(request, cancellationToken) :
                await Atualizar(request, cancellationToken);
        }

        private async Task<Result> Inserir(UpsertDirigentePatronalRequest request, CancellationToken cancellationToken)
        {
            var sindicatoPatronal = await _sindicatoPatronalRepository.ObterPorIdAsync(request.SindicatoPatronalId);
            if (sindicatoPatronal is null) return Result.Failure("Sindicato Patronal de id " + request.SindicatoPatronalId + " não encontrado");

            if (request.EstabelecimentoId is not null)
            {
                var estabelecimento = await _clienteUnidadeRepository.ObterPorIdAsync(request.EstabelecimentoId ?? 0);
                if (estabelecimento is null) return Result.Failure("Estabelecimento de id " + request.EstabelecimentoId + " não encontrado");
            } 

            var criarDirigenteResult = DirigentePatronal.Criar(
                request.Nome,
                request.Funcao,
                request.Situacao,
                request.DataInicioMandato,
                request.DataFimMandato,
                request.SindicatoPatronalId,
                request.EstabelecimentoId
            );

            if (criarDirigenteResult.IsFailure) return criarDirigenteResult;

            await _dirigentePatronalRepository.IncluirAsync(criarDirigenteResult.Value);

            await CommitAsync(cancellationToken);

            return Result.Success();
        }

        private async Task<Result> Atualizar(UpsertDirigentePatronalRequest request, CancellationToken cancellationToken)
        {
            var dirigente = await _dirigentePatronalRepository.ObterPorIdAsync(request.Id ?? 0);
            if (dirigente is null) return Result.Failure("Dirigente patronal de id " + request.Id + " não foi encontrado");

            var sindicatoPatronal = await _sindicatoPatronalRepository.ObterPorIdAsync(request.SindicatoPatronalId);
            if (sindicatoPatronal is null) return Result.Failure("Sindicato Patronal de id " + request.SindicatoPatronalId + " não encontrado");

            if (request.EstabelecimentoId is not null)
            {
                var estabelecimento = await _clienteUnidadeRepository.ObterPorIdAsync(request.EstabelecimentoId ?? 0);
                if (estabelecimento is null) return Result.Failure("Estabelecimento de id " + request.EstabelecimentoId + " não encontrado");
            }

            var atualizarDirigenteResult = dirigente.Atualizar(
                request.Nome,
                request.Funcao,
                request.Situacao,
                request.DataInicioMandato,
                request.DataFimMandato,
                request.SindicatoPatronalId,
                request.EstabelecimentoId
            );

            if (atualizarDirigenteResult.IsFailure) return atualizarDirigenteResult;

            await _dirigentePatronalRepository.AtualizarAsync(dirigente);

            await CommitAsync(cancellationToken);

            return Result.Success();
        }
    }
}
