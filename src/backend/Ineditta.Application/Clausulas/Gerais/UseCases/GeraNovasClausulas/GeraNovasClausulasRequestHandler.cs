using CSharpFunctionalExtensions;

using Ineditta.Application.Clausulas.Gerais.Entities;
using Ineditta.Application.Clausulas.Gerais.Repositiories;
using Ineditta.Application.Clausulas.Gerais.UseCases.Upsert;
using Ineditta.Application.EstruturasClausulas.Gerais.Repositories;
using Ineditta.Application.Usuarios.Repositories;
using Ineditta.BuildingBlocks.Core.Database;
using Ineditta.BuildingBlocks.Core.Domain.Handlers;

using MediatR;

namespace Ineditta.Application.Clausulas.Gerais.UseCases.GeraNovasClausulas
{
    public class GeraNovasClausulasRequestHandler : BaseCommandHandler, IRequestHandler<GeraNovasClausulasRequest, Result>
    {
        private readonly IEstruturaClausulaRepository _estruturaClausulaRepository;
        private readonly IClausulaGeralRepository _clausulaGeralRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        public GeraNovasClausulasRequestHandler(IUnitOfWork unitOfWork, IEstruturaClausulaRepository estruturaClausulaRepository, IClausulaGeralRepository clausulaGeralRepository, IUsuarioRepository usuarioRepository) : base(unitOfWork)
        {
            _estruturaClausulaRepository = estruturaClausulaRepository;
            _clausulaGeralRepository = clausulaGeralRepository;
            _usuarioRepository = usuarioRepository;
        }

        public async Task<Result> Handle(GeraNovasClausulasRequest request, CancellationToken cancellationToken)
        {
            var usuario = await _usuarioRepository.ObterPorIdAsync(request.UsuarioId);

            if (usuario == null)
            {
                return Result.Failure("Usuário não encontrado");
            }

            var estruturasClausulas = await _estruturaClausulaRepository.ObterTodasNaoContaDocumento(request.DocumentoId);

            if (estruturasClausulas is null)
            {
                return Result.Failure("Erro ao achar estruturas cláusulas que não constam no documento");
            }

            foreach (var estruturaItem in estruturasClausulas)
            {
                var clausula = ClausulaGeral.Criar(null, request.DocumentoId, estruturaItem.Id, null, null, null, usuario.Id, "Não consta", false, false);

                if (clausula.IsFailure)
                {
                    continue;
                }

                await _clausulaGeralRepository.IncluirAsync(clausula.Value);

                await CommitAsync(cancellationToken);
            }

            return Result.Success();
        }
    }
}
