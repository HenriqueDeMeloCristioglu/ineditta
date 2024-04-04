using CSharpFunctionalExtensions;

using Ineditta.Application.Comentarios.Entities;
using Ineditta.Application.Comentarios.Repositories;
using Ineditta.Application.Usuarios.Factories;
using Ineditta.BuildingBlocks.Core.Database;
using Ineditta.BuildingBlocks.Core.Domain.Handlers;

using MediatR;

namespace Ineditta.Application.Comentarios.UseCases.Upsert
{
    public class UpsertComentarioRequestHandler : BaseCommandHandler, IRequestHandler<UpsertComentarioRequest, Result>
    {
        private readonly ObterUsuarioLogadoFactory _obterUsuarioLogadoFactory;
        private readonly IComentarioRepository _comentarioRepository;

        public UpsertComentarioRequestHandler(IUnitOfWork unitOfWork, ObterUsuarioLogadoFactory obterUsuarioLogadoFactory, IComentarioRepository comentarioRepository) : base(unitOfWork)
        {
            _obterUsuarioLogadoFactory = obterUsuarioLogadoFactory;
            _comentarioRepository = comentarioRepository;
        }
        public async Task<Result> Handle(UpsertComentarioRequest request, CancellationToken cancellationToken)
        {
            return request.Id > 0 ? await Atualizar(request, cancellationToken) : await Incluir(request, cancellationToken);
        }

        public async Task<Result> Incluir(UpsertComentarioRequest request, CancellationToken cancellationToken)
        {
            var usuario = await _obterUsuarioLogadoFactory.PorEmail();

            if (usuario.IsFailure)
            {
                return usuario;
            }

            bool visivel = false;
            if (request.Visivel is not null)
            {
                visivel = true;
            }

            var comentario = Comentario.Criar(request.Tipo, request.Valor, request.TipoNotificacao, request.ReferenciaId, request.DataValidade, request.TipoUsuarioDestino, request.UsuarioDestionoId, request.EtiquetaId, visivel);

            if (comentario.IsFailure)
            {
                return comentario;
            }

            await _comentarioRepository.IncluirAsync(comentario.Value);

            _ = await CommitAsync(cancellationToken);

            return Result.Success();
        }

        public async Task<Result> Atualizar(UpsertComentarioRequest request, CancellationToken cancellationToken)
        {
            var comentario = await _comentarioRepository.ObterPorIdAsync(request.Id);

            if (comentario == null)
            {
                return Result.Failure("Comentário não encontrado");
            }

            bool visivel = false;
            if (request.Visivel is not null)
            {
                visivel = true;
            }

            var result = comentario.Atualizar(request.Tipo, request.Valor, request.TipoNotificacao, request.ReferenciaId, request.DataValidade, request.TipoUsuarioDestino, request.UsuarioDestionoId, request.EtiquetaId, visivel);

            if (result.IsFailure)
            {
                return result;
            }

            await _comentarioRepository.AtualizarAsync(comentario);

            _ = await CommitAsync(cancellationToken);

            return Result.Success();
        }
    }
}
