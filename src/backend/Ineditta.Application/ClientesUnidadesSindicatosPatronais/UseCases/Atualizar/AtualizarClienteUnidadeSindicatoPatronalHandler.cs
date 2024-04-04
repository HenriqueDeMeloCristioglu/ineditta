using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CSharpFunctionalExtensions;

using Ineditta.Application.ClientesUnidadesSindicatosPatronais.Entities;
using Ineditta.Application.ClientesUnidadesSindicatosPatronais.Repositories;
using Ineditta.Application.ClientesUnidadesSindicatosPatronais.UseCases.Adicionar;
using Ineditta.Application.ClientesUnidadesSindicatosPatronais.UseCases.Deletar;
using Ineditta.BuildingBlocks.Core.Bus;
using Ineditta.BuildingBlocks.Core.Database;
using Ineditta.BuildingBlocks.Core.Domain.Handlers;

using MediatR;

namespace Ineditta.Application.ClientesUnidadesSindicatosPatronais.UseCases.Atualizar
{
    public class AtualizarClienteUnidadeSindicatoPatronalHandler : BaseCommandHandler, IRequestHandler<AtualizarClienteUnidadeSindicatoPatronalRequest, Result>
    {
        private readonly IMediator _mediator;
        private readonly IClienteUnidadeSindicatoPatronalRepository _clienteUnidadeSindicatoPatronalRepository;
        public AtualizarClienteUnidadeSindicatoPatronalHandler(IUnitOfWork unitOfWork, IMediator mediator, IClienteUnidadeSindicatoPatronalRepository clienteUnidadeSindicatoPatronalRepository) : base(unitOfWork)
        {
            _mediator = mediator;
            _clienteUnidadeSindicatoPatronalRepository = clienteUnidadeSindicatoPatronalRepository;
        }

        public async Task<Result> Handle(AtualizarClienteUnidadeSindicatoPatronalRequest request, CancellationToken cancellationToken)
        {
            var unidadesAssociadasSindicato = 
                await _clienteUnidadeSindicatoPatronalRepository.ObterTodosPorSindicatoId(request.SindicatoPatronalId) ?? 
                new List<ClienteUnidadeSindicatoPatronal>();

            var unidadesIdsRequest = request.ClienteUnidadeId ?? new List<int>();

            var unidadesAssociadasParaAdicionar = unidadesIdsRequest
                                                    .Where(uId => !unidadesAssociadasSindicato.Any(x => x.ClienteUnidadeId == uId));

            var unidadesAssociadasParaRemover = unidadesAssociadasSindicato
                                                    .Where(ua => !unidadesIdsRequest.Any(uId => uId == ua.ClienteUnidadeId));

            var resultTransaction = await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                foreach (var unidadeAdicionar in unidadesAssociadasParaAdicionar)
                {
                    var requestAdicionar = new AdicionarClienteUnidadeSindicatoPatronalRequest
                    {
                        ClienteUnidadeId = unidadeAdicionar,
                        SindicatoPatronalId = request.SindicatoPatronalId,
                    };

                    var resultAdicionar = await _mediator.Send(requestAdicionar);
                    if (resultAdicionar.IsFailure) return resultAdicionar;
                }

                foreach (var unidadeRemover in unidadesAssociadasParaRemover)
                {
                    var requestRemover = new DeletarClienteUnidadeSindicatoPatronalRequest
                    {
                        Id = unidadeRemover.Id,
                    };

                    var resultRemover = await _mediator.Send(requestRemover);
                    if (resultRemover.IsFailure) return resultRemover;
                }

                return Result.Success();
            }, cancellationToken);

            if (resultTransaction.IsFailure) return resultTransaction;

            return Result.Success();
        }
    }
}
