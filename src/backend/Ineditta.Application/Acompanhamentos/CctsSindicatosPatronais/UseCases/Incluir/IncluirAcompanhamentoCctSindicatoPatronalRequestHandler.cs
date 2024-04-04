using CSharpFunctionalExtensions;

using Ineditta.Application.Acompanhamentos.CctsSindicatosPatronais.Entities;
using Ineditta.Application.Acompanhamentos.CctsSindicatosPatronais.Repositories;
using Ineditta.BuildingBlocks.Core.Database;
using Ineditta.BuildingBlocks.Core.Domain.Handlers;

using MediatR;

namespace Ineditta.Application.Acompanhamentos.CctsSindicatosPatronais.UseCases.Incluir
{
    public class IncluirAcompanhamentoCctSindicatoPatronalRequestHandler : BaseCommandHandler, IRequestHandler<IncluirAcompanhamentoCctSindicatoPatronalRequest, Result>
    {
        private readonly IAcompanhamentoCctSindicatoPatronalRepository _acompanhamentoCctSindicatoPatronalRepository;
        public IncluirAcompanhamentoCctSindicatoPatronalRequestHandler(IUnitOfWork unitOfWork, IAcompanhamentoCctSindicatoPatronalRepository acompanhamentoCctSindicatoPatronalRepository) : base(unitOfWork)
        {
            _acompanhamentoCctSindicatoPatronalRepository = acompanhamentoCctSindicatoPatronalRepository;
        }

        public async Task<Result> Handle(IncluirAcompanhamentoCctSindicatoPatronalRequest request, CancellationToken cancellationToken)
        {
            var acompanhamentoCctEstabelecimento = AcompanhamentoCctSinditoPatronal.Criar(request.AcompanhamentoCctId, request.SindicatoId);

            await _acompanhamentoCctSindicatoPatronalRepository.IncluirAsync(acompanhamentoCctEstabelecimento.Value);

            await CommitAsync(cancellationToken);

            return Result.Success();
        }
    }
}
