using CSharpFunctionalExtensions;

using Ineditta.Application.SharedKernel.Auth;
using Ineditta.Application.Usuarios.Entities;
using Ineditta.Application.Usuarios.Repositories;
using Ineditta.Application.UsuariosTiposEventosCalendarioSindical.Entities;
using Ineditta.Application.UsuariosTiposEventosCalendarioSindical.Repositories;
using Ineditta.BuildingBlocks.Core.Database;
using Ineditta.BuildingBlocks.Core.Domain.Handlers;
using Ineditta.BuildingBlocks.Core.Domain.ValueObjects;

using MediatR;

namespace Ineditta.Application.Usuarios.UseCases.Upsert
{
    public class UpsertUsuarioRequestHandler : BaseCommandHandler, IRequestHandler<UpsertUsuarioRequest, Result>
    {
        private readonly IAuthService _authService;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IUsuarioTipoEventoCalendarioSindicalRepository _usuarioTipoEventoCalendarioSindicalRepository;
        public UpsertUsuarioRequestHandler(IAuthService authService, IUsuarioRepository usuarioRepository, IUnitOfWork unitOfWork, IUsuarioTipoEventoCalendarioSindicalRepository usuarioTipoEventoCalendarioSindicalRepository)
            : base(unitOfWork)
        {
            _authService = authService;
            _usuarioRepository = usuarioRepository;
            _usuarioTipoEventoCalendarioSindicalRepository = usuarioTipoEventoCalendarioSindicalRepository;
        }

        public async Task<Result> Handle(UpsertUsuarioRequest request, CancellationToken cancellationToken)
        {
            return request.Id > 0 ?
                 await AtualizarAsync(request, cancellationToken) :
                 await IncluirAsync(request, cancellationToken);
        }

        public async Task<Result> IncluirAsync(UpsertUsuarioRequest request, CancellationToken cancellationToken)
        {
            var email = Email.Criar(request.Email!);

            if (email.IsFailure)
            {
                return email;
            }

            Ausencia? ausencia = null;
            if (request.AusenciaInicio is not null && request.AusenciaFim is not null)
            {
                var result = Ausencia.Criar(request.AusenciaInicio.Value, request.AusenciaFim.Value);

                if (result.IsFailure)
                {
                    return result;
                }

                ausencia = result.Value;
            }

            var modulosSisap = new List<UsuarioModulo>(request.ModulosSisap?.Count() ?? 1);

            if ((request.ModulosSisap?.Any() ?? false))
            {
                foreach (var moduloRequest in request.ModulosSisap)
                {
                    var moduloSisap = UsuarioModulo.Gerar(moduloRequest.Id, moduloRequest.Criar, moduloRequest.Consultar, moduloRequest.Comentar, moduloRequest.Alterar, moduloRequest.Excluir, moduloRequest.Aprovar);

                    if (moduloSisap.IsFailure)
                    {
                        return moduloSisap;
                    }

                    modulosSisap.Add(moduloSisap.Value);
                }
            }

            var modulosComerciais = new List<UsuarioModulo>(request.ModulosComerciais?.Count() ?? 1);

            if ((request.ModulosComerciais?.Any() ?? false))
            {
                foreach (var moduloRequest in request.ModulosComerciais)
                {
                    var moduloComercial = UsuarioModulo.Gerar(moduloRequest.Id, moduloRequest.Criar, moduloRequest.Consultar, moduloRequest.Comentar, moduloRequest.Alterar, moduloRequest.Excluir, moduloRequest.Aprovar);

                    if (moduloComercial.IsFailure)
                    {
                        return moduloComercial;
                    }

                    modulosComerciais.Add(moduloComercial.Value);
                }
            }

            var usuario = Usuario.Criar(
                request.Nome,
                email.Value,
                request.Cargo,
                request.Celular,
                request.Ramal,
                request.SuperiorId,
                request.Departamento,
                request.Bloqueado,
                request.DocumentoRestrito,
                ausencia,
                request.Tipo,
                request.Nivel,
                request.NotificarWhatsapp,
                request.NotificarEmail,
                request.GrupoEconomicoId,
                request.JornadaId,
                request.LocalidadesIds,
                request.CnaesIds,
                request.GruposClausulasIds,
                request.EstabelecimentosIds,
                modulosSisap,
                modulosComerciais
                );

            if (usuario.IsFailure)
            {
                return usuario;
            }

            await _usuarioRepository.IncluirAsync(usuario.Value);

            var authServiceResult = await _authService.IncluirAsync(usuario.Value, request.Username);

            if (authServiceResult.IsFailure)
            {
                return Result.Failure(authServiceResult.Error.Message);
            }

            _ = await CommitAsync(cancellationToken);

            var requestCalendarioConfig = request.CalendarioConfig ?? new List<UsuarioTipoEventoCalendarioSindicalInputModel>();

            foreach (var config in requestCalendarioConfig)
            {
                var criarResult = UsuarioTipoEventoCalendarioSindical.Criar(
                     usuario.Value.Id,
                     config.TipoId,
                     config.SubtipoId,
                     config.NotificarEmail,
                     config.NotificarWhatsapp,
                     null
                 );

                if (criarResult.IsFailure) return criarResult;

                await _usuarioTipoEventoCalendarioSindicalRepository.IncluirAsync(criarResult.Value);

                await CommitAsync(cancellationToken);
            }

            return Result.Success();
        }

        public async Task<Result> AtualizarAsync(UpsertUsuarioRequest request, CancellationToken cancellationToken)
        {
            var usuario = await _usuarioRepository.ObterPorIdAsync(request.Id);

            if (usuario is null)
            {
                return Result.Failure("Usuário não encontrado");
            }

            var email = Email.Criar(request.Email!);

            if (email.IsFailure)
            {
                return email;
            }

            Ausencia? ausencia = null;
            if (request.AusenciaInicio is not null && request.AusenciaFim is not null)
            {
                var ausenciaResult = Ausencia.Criar(request.AusenciaInicio.Value, request.AusenciaFim.Value);

                if (ausenciaResult.IsFailure)
                {
                    return ausenciaResult;
                }

                ausencia = ausenciaResult.Value;
            }

            var modulosSisap = new List<UsuarioModulo>(request.ModulosSisap?.Count() ?? 1);

            if ((request.ModulosSisap?.Any() ?? false))
            {
                foreach (var moduloRequest in request.ModulosSisap)
                {
                    var moduloSisap = UsuarioModulo.Gerar(moduloRequest.Id, moduloRequest.Criar, moduloRequest.Consultar, moduloRequest.Comentar, moduloRequest.Alterar, moduloRequest.Excluir, moduloRequest.Aprovar);

                    if (moduloSisap.IsFailure)
                    {
                        return moduloSisap;
                    }

                    modulosSisap.Add(moduloSisap.Value);
                }
            }

            var modulosComerciais = new List<UsuarioModulo>(request.ModulosComerciais?.Count() ?? 1);

            if ((request.ModulosComerciais?.Any() ?? false))
            {
                foreach (var moduloRequest in request.ModulosComerciais)
                {
                    var moduloComercial = UsuarioModulo.Gerar(moduloRequest.Id, moduloRequest.Criar, moduloRequest.Consultar, moduloRequest.Comentar, moduloRequest.Alterar, moduloRequest.Excluir, moduloRequest.Aprovar);

                    if (moduloComercial.IsFailure)
                    {
                        return moduloComercial;
                    }

                    modulosComerciais.Add(moduloComercial.Value);
                }
            }

            var resultTransaction = await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                var requestCalendarioConfig = request.CalendarioConfig ?? new List<UsuarioTipoEventoCalendarioSindicalInputModel>();
                var usuariosTiposEventos = await _usuarioTipoEventoCalendarioSindicalRepository.ObterTodosPorUsuarioIdAsync(usuario.Id);

                foreach (var config in requestCalendarioConfig)
                {
                    if (config.Id is not null)
                    {
                        var configAtualizar = usuariosTiposEventos.SingleOrDefault(ute => ute.Id == config.Id);
                        if (configAtualizar is null) return Result.Failure("Não foram encontradas as informação para atualização das notificações do calendário sindical");

                        var atualizarResult = configAtualizar.Atualizar(
                            config.NotificarEmail,
                            config.NotificarWhatsapp
                        );

                        if (atualizarResult.IsFailure)
                        {
                            return atualizarResult;
                        }

                        await _usuarioTipoEventoCalendarioSindicalRepository.AtualizarAsync(configAtualizar);

                        await CommitAsync(cancellationToken);
                    }
                    else
                    {
                        var criarResult = UsuarioTipoEventoCalendarioSindical.Criar(
                        config.UsuarioId,
                        config.TipoId,
                        config.SubtipoId,
                        config.NotificarEmail,
                        config.NotificarWhatsapp,
                        null
                    );

                        if (criarResult.IsFailure) return criarResult;

                        await _usuarioTipoEventoCalendarioSindicalRepository.IncluirAsync(criarResult.Value);

                        await CommitAsync(cancellationToken);
                    }
                }

                var result = usuario.Atualizar(request.Nome,
                    email.Value,
                    request.Cargo,
                    request.Celular,
                    request.Ramal,
                    request.SuperiorId,
                    request.Departamento,
                    request.Bloqueado,
                    request.DocumentoRestrito,
                    ausencia,
                    request.Tipo,
                    request.Nivel,
                    request.NotificarWhatsapp,
                    request.NotificarEmail,
                    request.GrupoEconomicoId,
                    request.JornadaId,
                    request.LocalidadesIds,
                    request.CnaesIds,
                    request.GruposClausulasIds,
                    request.EstabelecimentosIds,
                    modulosSisap,
                    modulosComerciais);

                if (result.IsFailure)
                {
                    return result;
                }

                await _usuarioRepository.AtualizarAsync(usuario);

                await CommitAsync(cancellationToken);
                return Result.Success();
            }, cancellationToken);

            return resultTransaction;
        }
    }
}