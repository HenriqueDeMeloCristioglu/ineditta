using CSharpFunctionalExtensions;

using Ineditta.Application.Jornada.Entities;
using Ineditta.Application.Jornada.Repositories;

using MediatR;

namespace Ineditta.Application.Jornada.UseCases.Upsert
{
    public class UpsertJornadaHandler : IRequestHandler<UpsertJornadaRequest, Result>
    {
        private readonly IJornadaRepository _jornadaRepository;
        public UpsertJornadaHandler(IJornadaRepository jornadaRepository)
        {
            _jornadaRepository = jornadaRepository;
        }

        public async Task<Result> Handle(UpsertJornadaRequest request, CancellationToken cancellationToken)
        {
            var jornada = Entities.Jornada.Criar(
                    request.JoranadaSemanal, request.Descricao, request.IsDeault
                );

            if (jornada.IsFailure)
            {
                return jornada;
            }

            await _jornadaRepository.IncluirAsync(jornada.Value);

            return Result.Success();
        }
    }
}
