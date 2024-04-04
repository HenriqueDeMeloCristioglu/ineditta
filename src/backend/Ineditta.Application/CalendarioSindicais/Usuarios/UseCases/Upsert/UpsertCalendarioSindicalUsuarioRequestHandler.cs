using CSharpFunctionalExtensions;

using Ineditta.Application.CalendarioSindicais.Eventos.Entities;
using Ineditta.Application.CalendarioSindicais.Eventos.Repositories;
using Ineditta.Application.CalendarioSindicais.Eventos.Services;
using Ineditta.Application.CalendarioSindicais.Usuarios.Entities;
using Ineditta.Application.CalendarioSindicais.Usuarios.Repositories;
using Ineditta.Application.Usuarios.Repositories;
using Ineditta.BuildingBlocks.Core.Auth;
using Ineditta.BuildingBlocks.Core.Database;
using Ineditta.BuildingBlocks.Core.Domain.Handlers;

using MediatR;

namespace Ineditta.Application.CalendarioSindicais.Usuarios.UseCases.Upsert
{
    public class UpsertCalendarioSindicalUsuarioRequestHandler : BaseCommandHandler, IRequestHandler<UpsertCalendarioSindicalUsuarioRequest, Result>
    {
        private readonly ICalendarioSindicalUsuarioRepository _calendarioSindicatoUsuarioRepository;
        private readonly ICalendarioSindicalGeradorService _calendarioSindicalGeradorService;
        private readonly IUserInfoService _userInfoService;
        private readonly IUsuarioRepository _usuarioRepository;
        public UpsertCalendarioSindicalUsuarioRequestHandler(IUnitOfWork unitOfWork, ICalendarioSindicalUsuarioRepository calendarioSindicatoUsuarioRepository, IUserInfoService userInfoService, IUsuarioRepository usuarioRepository, ICalendarioSindicalGeradorService calendarioSindicalGeradorService) : base(unitOfWork)
        {
            _calendarioSindicatoUsuarioRepository = calendarioSindicatoUsuarioRepository;
            _userInfoService = userInfoService;
            _usuarioRepository = usuarioRepository;
            _calendarioSindicalGeradorService = calendarioSindicalGeradorService;
        }
        public async Task<Result> Handle(UpsertCalendarioSindicalUsuarioRequest request, CancellationToken cancellationToken)
        {
            return request.Id > 0 ?
                await AtualizarAsync(request, cancellationToken) :
                await IncluirAsync(request, cancellationToken);
        }

        private async Task<Result> AtualizarAsync(UpsertCalendarioSindicalUsuarioRequest request, CancellationToken cancellationToken)
        {
            var evento = await _calendarioSindicatoUsuarioRepository.ObterPorIdAsync(request.Id ?? 0);

            if (evento is null)
            {
                return Result.Failure("Evento não encontrado");
            }

            var usuario = await _usuarioRepository.ObterPorEmailAsync(_userInfoService.GetEmail()!);

            if (usuario is null)
            {
                return Result.Failure("Usuário não encontrado");
            }

            var result = evento.Atualizar(
                request.Titulo,
                request.DataHora,
                request.Recorrencia,
                request.ValidadeRecorrencia,
                TimeSpan.FromSeconds(request.NotificarAntes ?? 0),
                request.Local,
                request.Comentarios,
                request.Visivel,
                usuario
            );

            if (result.IsFailure)
            {
                return result;
            }

            await _calendarioSindicatoUsuarioRepository.AtualizarAsync(evento);

            _ = await CommitAsync(cancellationToken);

            return Result.Success();
        }

        private async Task<Result> IncluirAsync(UpsertCalendarioSindicalUsuarioRequest request, CancellationToken cancellationToken)
        {
            var usuario = await _usuarioRepository.ObterPorEmailAsync(_userInfoService.GetEmail()!);

            if (usuario is null)
            {
                return Result.Failure("Usuário não encontrado");
            }

            var evento = CalendarioSindicalUsuario.Criar(
                request.Titulo,
                request.DataHora,
                request.Recorrencia,
                request.ValidadeRecorrencia,
                TimeSpan.FromSeconds(request.NotificarAntes ?? 0),
                request.Local,
                request.Comentarios,
                request.Visivel,
                usuario
            );

            if (evento.IsFailure)
            {
                return evento;
            }

            var result = await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                await _calendarioSindicatoUsuarioRepository.IncluirAsync(evento.Value);

                _ = await CommitAsync(cancellationToken);

                await _calendarioSindicalGeradorService.GerarAgendaEventosAsync(cancellationToken);

                await CommitAsync(cancellationToken);

                return Result.Success();

            }, cancellationToken);

            return result;
        }
    }
}
