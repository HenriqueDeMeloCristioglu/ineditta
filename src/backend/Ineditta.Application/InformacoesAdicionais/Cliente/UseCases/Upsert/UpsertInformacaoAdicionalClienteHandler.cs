using CSharpFunctionalExtensions;

using Ineditta.Application.InformacoesAdicionais.Cliente.Entities;
using Ineditta.Application.InformacoesAdicionais.Cliente.Factories;
using Ineditta.Application.InformacoesAdicionais.Cliente.Repositories;
using Ineditta.Application.Usuarios.Repositories;
using Ineditta.BuildingBlocks.Core.Auth;
using Ineditta.BuildingBlocks.Core.Database;
using Ineditta.BuildingBlocks.Core.Domain.Handlers;

using MediatR;

namespace Ineditta.Application.InformacoesAdicionais.Cliente.UseCases.Upsert
{
    public class UpsertInformacaoAdicionalClienteHandler : BaseCommandHandler, IRequestHandler<UpsertInformacaoAdicionalClienteRequest, Result>
    {
        private readonly IInformacaoAdicionalClienteRepository _informacaoAdicionalClienteRepository;
        private readonly IUserInfoService _userInfoService;
        private readonly IUsuarioRepository _usuarioRepository;
        public UpsertInformacaoAdicionalClienteHandler(IUnitOfWork unitOfWork, IInformacaoAdicionalClienteRepository informacaoAdicionalClienteRepository, IUserInfoService userInfoService, IUsuarioRepository usuarioRepository) : base(unitOfWork)
        {
            _informacaoAdicionalClienteRepository = informacaoAdicionalClienteRepository;
            _userInfoService = userInfoService;
            _usuarioRepository = usuarioRepository;
        }

        public async Task<Result> Handle(UpsertInformacaoAdicionalClienteRequest request, CancellationToken cancellationToken)
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

            IEnumerable<InformacaoAdicional> informacoesAdicionais = Enumerable.Empty<InformacaoAdicional>();

            if (request.InformacoesAdicionais is not null && request.InformacoesAdicionais.Any())
            {
                var informacoesAdicionaisResult = InformacaoAdicionalFactory.Criar(request.InformacoesAdicionais);

                if (informacoesAdicionaisResult.IsFailure)
                {
                    return Result.Failure(informacoesAdicionaisResult.Error);
                }

                informacoesAdicionais = informacoesAdicionaisResult.Value;
            }


            var obeervacoesAdicionaisResult = ObservacaoAdicionalFactory.Criar(request.ObservacoesAdicionais);
            if (obeervacoesAdicionaisResult.IsFailure)
            {
                return Result.Failure(obeervacoesAdicionaisResult.Error);
            }

            if (informacaoAdicionalCliente is null)
            {
                var novaInformacaoAdicionalCliente = InformacaoAdicionalCliente.Criar(usuario, request.DocumentoSindicalId, informacoesAdicionais, obeervacoesAdicionaisResult.Value, request.Orientacao, request.OutrasInformacoes);

                if (novaInformacaoAdicionalCliente.IsFailure)
                {
                    return novaInformacaoAdicionalCliente;
                }

                await _informacaoAdicionalClienteRepository.IncluirAsync(novaInformacaoAdicionalCliente.Value);

                _ = await CommitAsync(cancellationToken);

                return Result.Success();
            }

            informacaoAdicionalCliente.Atualizar(usuario, informacoesAdicionais, obeervacoesAdicionaisResult.Value, request.Orientacao, request.OutrasInformacoes);

            await _informacaoAdicionalClienteRepository.AtualizarAsync(informacaoAdicionalCliente);

            _ = await CommitAsync(cancellationToken);

            return Result.Success();
        }
    }
}
