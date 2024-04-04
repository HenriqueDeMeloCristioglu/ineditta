using CSharpFunctionalExtensions;

using Ineditta.Application.ClientesMatriz.Repositories;
using Ineditta.BuildingBlocks.Core.Database;
using Ineditta.BuildingBlocks.Core.Domain.Handlers;

using MediatR;

namespace Ineditta.Application.ClientesMatriz.UseCases.InativarAtivarToggle
{
    public class InativarAtivarClienteMatrizRequestHandler : BaseCommandHandler, IRequestHandler<InativarAtivarClienteMatrizRequest, Result>
    {
        private readonly IClienteMatrizRepository _clienteMatrizRepository;
        public InativarAtivarClienteMatrizRequestHandler(IUnitOfWork unitOfWork, IClienteMatrizRepository clienteMatrizRepository) : base(unitOfWork)
        {
            _clienteMatrizRepository = clienteMatrizRepository;
        }
        public async Task<Result> Handle(InativarAtivarClienteMatrizRequest request, CancellationToken cancellationToken)
        {
            var clienteMatriz = await _clienteMatrizRepository.ObterPorId(request.Id, cancellationToken);
            if (clienteMatriz is null) return Result.Failure("Cliente matriz não encontrado");

            var inativarAtivarResult = clienteMatriz.InativarAtivarToggle();

            if (inativarAtivarResult.IsFailure)
            {
                return inativarAtivarResult;
            }

            await _clienteMatrizRepository.AtualizarAsync(clienteMatriz, cancellationToken);

            await CommitAsync(cancellationToken);

            return Result.Success();
        }
    }
}
