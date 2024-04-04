using CSharpFunctionalExtensions;

using Ineditta.Application.Clausulas.Gerais.Repositiories;
using Ineditta.Application.Usuarios.Factories;
using Ineditta.BuildingBlocks.Core.Database;
using Ineditta.BuildingBlocks.Core.Domain.Handlers;

using MediatR;

namespace Ineditta.Application.Clausulas.Gerais.UseCases.Aprovar
{
    public class AprovarClausulaGeralRequestHandler : BaseCommandHandler, IRequestHandler<AprovarClausulaGeralRequest, Result>
    {
        private readonly IClausulaGeralRepository _clausulaGeralRepository;
        private readonly ObterUsuarioLogadoFactory _obterUsuarioLogadoFactory;

        public AprovarClausulaGeralRequestHandler(IUnitOfWork unitOfWork, IClausulaGeralRepository clausulaGeralRepository, ObterUsuarioLogadoFactory obterUsuarioLogadoFactory) : base(unitOfWork)
        {
            _clausulaGeralRepository = clausulaGeralRepository;
            _obterUsuarioLogadoFactory = obterUsuarioLogadoFactory;
        }

        public async Task<Result> Handle(AprovarClausulaGeralRequest request, CancellationToken cancellationToken)
        {
            var usuario = await _obterUsuarioLogadoFactory.PorEmail();

            if (usuario.IsFailure)
            {
                return usuario;
            }

            var clausulaGeral = await _clausulaGeralRepository.ObterPorId(request.Id);

            if (clausulaGeral == null)
            {
                return Result.Failure("Clausula não encontrada");
            }

            clausulaGeral.Aprovar(usuario.Value.Id);

            await _clausulaGeralRepository.AtualizarAsync(clausulaGeral);

            await CommitAsync(cancellationToken);

            return Result.Success();
        }
    }
}
