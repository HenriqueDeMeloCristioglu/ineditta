using CSharpFunctionalExtensions;

using Ineditta.Application.InformacoesAdicionais.Cliente.Aprovar;
using Ineditta.Application.InformacoesAdicionais.Cliente.Repositories;
using Ineditta.Application.Usuarios.Repositories;
using Ineditta.BuildingBlocks.Core.Auth;
using Ineditta.BuildingBlocks.Core.Database;
using Ineditta.BuildingBlocks.Core.Domain.Handlers;

using MediatR;

namespace Ineditta.Application.InformacoesAdicionais.Cliente.UseCases
{
    public class AprovarInformacaoAdicionalClienteHandler : BaseCommandHandler, IRequestHandler<AprovarInformacaoAdicionalClienteRequest, Result>
    {
        private readonly IInformacaoAdicionalClienteRepository _informacaoAdicionalClienteRepository;
        private readonly IUserInfoService _userInfoService;
        private readonly IUsuarioRepository _usuarioRepository;
        public AprovarInformacaoAdicionalClienteHandler(IUnitOfWork unitOfWork, IInformacaoAdicionalClienteRepository informacaoAdicionalClienteRepository, IUserInfoService userInfoService, IUsuarioRepository usuarioRepository) : base(unitOfWork)
        {
            _informacaoAdicionalClienteRepository = informacaoAdicionalClienteRepository;
            _userInfoService = userInfoService;
            _usuarioRepository = usuarioRepository;
        }

        public async Task<Result> Handle(AprovarInformacaoAdicionalClienteRequest request, CancellationToken cancellationToken)
        {
            var usuario = await _usuarioRepository.ObterPorEmailAsync(_userInfoService.GetEmail()!);

            if (usuario is null)
            {
                return Result.Failure("Usuário não foi encontrado");
            }

            if (usuario.GrupoEconomicoId == null)
            {
                return Result.Failure("Usuário não tem grupo econômico vinculado");
            }

            var informacaoAdicionalCliente = await _informacaoAdicionalClienteRepository.ObterPorGrupoDocumentoAsync(usuario.GrupoEconomicoId.Value, request.DocumentoSindicalId);

            if (informacaoAdicionalCliente is null)
            {
                return Result.Failure("Informação Adicional não encontrada");
            }

            var result = informacaoAdicionalCliente.Aprovar(usuario);

            if (result.IsFailure)
            {
                return result;
            }

            await _informacaoAdicionalClienteRepository.AtualizarAsync(informacaoAdicionalCliente);

            _ = await CommitAsync(cancellationToken);

            return Result.Success();
        }
    }
}
