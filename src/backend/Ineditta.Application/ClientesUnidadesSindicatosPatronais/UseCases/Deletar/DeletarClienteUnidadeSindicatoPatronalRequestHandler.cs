using CSharpFunctionalExtensions;

using Ineditta.Application.ClientesUnidadesSindicatosPatronais.Repositories;
using Ineditta.BuildingBlocks.Core.Database;
using Ineditta.BuildingBlocks.Core.Domain.Handlers;

using MediatR;

namespace Ineditta.Application.ClientesUnidadesSindicatosPatronais.UseCases.Deletar
{
    public class DeletarClienteUnidadeSindicatoPatronalRequestHandler : BaseCommandHandler, IRequestHandler<DeletarClienteUnidadeSindicatoPatronalRequest, Result>
    {
        private readonly IClienteUnidadeSindicatoPatronalRepository _clienteUnidadeSindicatoPatronalRepository;
        public DeletarClienteUnidadeSindicatoPatronalRequestHandler(IUnitOfWork unitOfWork, IClienteUnidadeSindicatoPatronalRepository clienteUnidadeSindicatoPatronalRepository) : base(unitOfWork)
        {
            _clienteUnidadeSindicatoPatronalRepository = clienteUnidadeSindicatoPatronalRepository;
        }
        public async Task<Result> Handle(DeletarClienteUnidadeSindicatoPatronalRequest request, CancellationToken cancellationToken)
        {
            var existe = await _clienteUnidadeSindicatoPatronalRepository.ExistePorIdAsync(request.Id);
            if (!existe) return Result.Failure("Não foi encontrado nenhum 'cliente unidade sindicato patronal' com o id fornecido.");
            
            await _clienteUnidadeSindicatoPatronalRepository.RemoverPorIdAsync(request.Id);
            await CommitAsync(cancellationToken);
            
            return Result.Success();
        }
    }
}
