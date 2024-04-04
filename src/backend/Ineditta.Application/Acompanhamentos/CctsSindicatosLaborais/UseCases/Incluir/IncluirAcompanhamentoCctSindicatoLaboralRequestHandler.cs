
using CSharpFunctionalExtensions;

using Ineditta.Application.Acompanhamentos.CctsSindicatosLaborais.Entities;
using Ineditta.Application.Acompanhamentos.CctsSindicatosLaborais.Repositories;
using Ineditta.BuildingBlocks.Core.Database;
using Ineditta.BuildingBlocks.Core.Domain.Handlers;

using MediatR;

namespace Ineditta.Application.Acompanhamentos.CctsSindicatosLaborais.UseCases.Incluir
{
    public class IncluirAcompanhamentoCctSindicatoLaboralRequestHandler : BaseCommandHandler, IRequestHandler<IncluirAcompanhamentoCctSindicatoLaboralRequest, Result>
    {
        private readonly IAcompanhamentoCctSindicatoLaboralRepository _acompanhamentoCctSindicatoLaboralRepository;

        public IncluirAcompanhamentoCctSindicatoLaboralRequestHandler(IUnitOfWork unitOfWork, IAcompanhamentoCctSindicatoLaboralRepository acompanhamentoCctEstabelecimentoRepository) : base(unitOfWork)
        {
            _acompanhamentoCctSindicatoLaboralRepository = acompanhamentoCctEstabelecimentoRepository;
        }

        public async Task<Result> Handle(IncluirAcompanhamentoCctSindicatoLaboralRequest request, CancellationToken cancellationToken)
        {
            var acompanhamentoCctEstabelecimento = AcompanhamentoCctSinditoLaboral.Criar(request.AcompanhamentoCctId, request.SindicatoId);

            await _acompanhamentoCctSindicatoLaboralRepository.IncluirAsync(acompanhamentoCctEstabelecimento.Value);

            await CommitAsync(cancellationToken);

            return Result.Success();
        }
    }
}
