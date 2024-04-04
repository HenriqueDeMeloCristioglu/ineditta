using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CSharpFunctionalExtensions;

using Ineditta.Application.ClientesUnidades.Repositories;
using Ineditta.Application.ClientesUnidadesSindicatosPatronais.Entities;
using Ineditta.Application.ClientesUnidadesSindicatosPatronais.Repositories;
using Ineditta.Application.Sindicatos.Patronais.Repositories;
using Ineditta.BuildingBlocks.Core.Bus;
using Ineditta.BuildingBlocks.Core.Database;
using Ineditta.BuildingBlocks.Core.Domain.Handlers;

using MediatR;

namespace Ineditta.Application.ClientesUnidadesSindicatosPatronais.UseCases.Adicionar
{
    public class AdicionarClienteUnidadeSindicatoPatronalRequestHandler : BaseCommandHandler, IRequestHandler<AdicionarClienteUnidadeSindicatoPatronalRequest, Result>
    {
        private readonly IClienteUnidadeSindicatoPatronalRepository _clienteUnidadePatronalRepository;
        private readonly IClienteUnidadeRepository _clienteUnidadeRepository;
        private readonly ISindicatoPatronalRepository _sindicatoPatronalRepository;
        public AdicionarClienteUnidadeSindicatoPatronalRequestHandler(IUnitOfWork unitOfWork, IClienteUnidadeSindicatoPatronalRepository clienteUnidadePatronalRepository, IClienteUnidadeRepository clienteUnidadeRepository, ISindicatoPatronalRepository sindicatoPatronalRepository) : base(unitOfWork)
        {
            _clienteUnidadePatronalRepository = clienteUnidadePatronalRepository;
            _clienteUnidadeRepository = clienteUnidadeRepository;
            _sindicatoPatronalRepository = sindicatoPatronalRepository;
        }
        public async Task<Result> Handle(AdicionarClienteUnidadeSindicatoPatronalRequest request, CancellationToken cancellationToken)
        {
            var sindicatoPatronalExiste = await _sindicatoPatronalRepository.ExistePorIdAsync(request.SindicatoPatronalId);
            if (!sindicatoPatronalExiste) return Result.Failure("Sindicato patronal fornecido não encontrado no sistema");

            var clienteUnidadeExiste = await _clienteUnidadeRepository.ExistePorIdAsync(request.ClienteUnidadeId);
            if (!clienteUnidadeExiste) return Result.Failure("Cliente unidade fornecido não encontrado no sistema");

            var resultCriar = ClienteUnidadeSindicatoPatronal.Criar(request.ClienteUnidadeId, request.SindicatoPatronalId);
            if (resultCriar.IsFailure) return resultCriar;

            await _clienteUnidadePatronalRepository.IncluirAsync(resultCriar.Value);
            await CommitAsync(cancellationToken);
            
            return Result.Success();
        }
    }
}
