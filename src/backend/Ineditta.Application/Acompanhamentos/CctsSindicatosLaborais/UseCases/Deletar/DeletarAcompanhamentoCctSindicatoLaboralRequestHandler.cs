using CSharpFunctionalExtensions;

using Ineditta.Application.Acompanhamentos.CctsSindicatosLaborais.Repositories;
using Ineditta.BuildingBlocks.Core.Database;
using Ineditta.BuildingBlocks.Core.Domain.Handlers;

using MediatR;

namespace Ineditta.Application.Acompanhamentos.CctsSindicatosLaborais.UseCases.Deletar
{
    public class DeletarAcompanhamentoCctSindicatoLaboralRequestHandler : BaseCommandHandler, IRequestHandler<DeletarAcompanhamentoCctSindicatoLaboralRequest, Result>
    {
        private readonly IAcompanhamentoCctSindicatoLaboralRepository _acompanhamentoCctSindicatoLaboralRepository;
        public DeletarAcompanhamentoCctSindicatoLaboralRequestHandler(IUnitOfWork unitOfWork, IAcompanhamentoCctSindicatoLaboralRepository acompanhamentoCctSindicatoLaboralRepository) : base(unitOfWork)
        {
            _acompanhamentoCctSindicatoLaboralRepository = acompanhamentoCctSindicatoLaboralRepository;
        }

        public async Task<Result> Handle(DeletarAcompanhamentoCctSindicatoLaboralRequest request, CancellationToken cancellationToken)
        {
            await _acompanhamentoCctSindicatoLaboralRepository.DeletarAsync(request.AcompanhamentoCctSinditoLaboral);

            await CommitAsync(cancellationToken);

            return Result.Success();
        }
    }
}
