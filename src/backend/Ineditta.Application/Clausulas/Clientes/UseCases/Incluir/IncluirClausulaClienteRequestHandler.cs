using CSharpFunctionalExtensions;

using Ineditta.Application.Clausulas.Clientes.Entities;
using Ineditta.Application.Clausulas.Clientes.Repositories;
using Ineditta.Application.Usuarios.Factories;
using Ineditta.BuildingBlocks.Core.Database;
using Ineditta.BuildingBlocks.Core.Domain.Handlers;

using MediatR;

namespace Ineditta.Application.Clausulas.Clientes.UseCases.Upsert
{
    public class IncluirClausulaClienteRequestHandler : BaseCommandHandler, IRequestHandler<IncluirClausulaClienteRequest, Result>
    {
        private readonly IClausulaClienteRepository _clausulaClienteRepository;
        private readonly ObterUsuarioLogadoFactory _obterUsuarioLogadoFactory;
        public IncluirClausulaClienteRequestHandler(IUnitOfWork unitOfWork, IClausulaClienteRepository clausulaClienteRepository, ObterUsuarioLogadoFactory obterUsuarioLogadoFactory) : base(unitOfWork)
        {
            _clausulaClienteRepository = clausulaClienteRepository;
            _obterUsuarioLogadoFactory = obterUsuarioLogadoFactory;
        }

        public async Task<Result> Handle(IncluirClausulaClienteRequest request, CancellationToken cancellationToken)
        {
            var usuario = await _obterUsuarioLogadoFactory.PorEmail();

            if (usuario.IsFailure)
            {
                return usuario;
            }

            var clausulaCliente = ClausulaCliente.Criar(request.ClausulaId, request.Texto, usuario.Value.GrupoEconomicoId ?? 0, usuario.Value.Nivel);

            if (clausulaCliente.IsFailure)
            {
                return clausulaCliente;
            }

            await _clausulaClienteRepository.IncluirAsync(clausulaCliente.Value);

            await CommitAsync(cancellationToken);

            return Result.Success();
        }
    }
}
