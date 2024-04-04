using CSharpFunctionalExtensions;

using Ineditta.Application.Acompanhamentos.CctsSindicatosPatronais.Repositories;
using Ineditta.BuildingBlocks.Core.Database;
using Ineditta.BuildingBlocks.Core.Domain.Handlers;

using MediatR;

namespace Ineditta.Application.Acompanhamentos.CctsSindicatosPatronais.UseCases.Deletar
{
    public class DeletarAcompanhamentoCctSindicatoPatronalRequestHandler : BaseCommandHandler, IRequestHandler<DeletarAcompanhamentoCctSindicatoPatronalRequest, Result>
    {
        private readonly IAcompanhamentoCctSindicatoPatronalRepository _acompanhamentoCctSindicatoPatronalRepository;
        public DeletarAcompanhamentoCctSindicatoPatronalRequestHandler(IUnitOfWork unitOfWork, IAcompanhamentoCctSindicatoPatronalRepository acompanhamentoCctSindicatoPatronalRepository) : base(unitOfWork)
        {
            _acompanhamentoCctSindicatoPatronalRepository = acompanhamentoCctSindicatoPatronalRepository;
        }

        public async Task<Result> Handle(DeletarAcompanhamentoCctSindicatoPatronalRequest request, CancellationToken cancellationToken)
        {
            await _acompanhamentoCctSindicatoPatronalRepository.DeletarAsync(request.AcompanhamentoCctSinditoPatronal);

            await CommitAsync(cancellationToken);

            return Result.Success();
        }
    }
}
