using CSharpFunctionalExtensions;

using Ineditta.Application.Clausulas.Gerais.Events.ResumirClausulasEvent;
using Ineditta.Application.Clausulas.Gerais.Repositiories;
using Ineditta.Application.Usuarios.Factories;
using Ineditta.BuildingBlocks.Core.Bus;
using Ineditta.BuildingBlocks.Core.Database;
using Ineditta.BuildingBlocks.Core.Domain.Handlers;

using MediatR;

namespace Ineditta.Application.Clausulas.Gerais.UseCases.Resumir
{
    public class ResumirClausulaRequestHandler : BaseCommandHandler, IRequestHandler<ResumirClausulaRequest, Result>
    {
        private readonly IClausulaGeralRepository _clausulaGeralRepository;
        private readonly IMessagePublisher _messagePublisher;
        private readonly ObterUsuarioLogadoFactory _observationUsuarioLogadoFactory;

        public ResumirClausulaRequestHandler(IUnitOfWork unitOfWork, IMessagePublisher messagePublisher, IClausulaGeralRepository clausulaGeralRepository, ObterUsuarioLogadoFactory observationUsuarioLogadoFactory) : base(unitOfWork)
        {
            _messagePublisher = messagePublisher;
            _clausulaGeralRepository = clausulaGeralRepository;
            _observationUsuarioLogadoFactory = observationUsuarioLogadoFactory;
        }

        public async Task<Result> Handle(ResumirClausulaRequest request, CancellationToken cancellationToken)
        {

            var clausulas = await _clausulaGeralRepository.ObterTodasPorDocumentoResumivel(request.DocumentoId);

            if (clausulas == null)
            {
                return Result.Failure("Nenhuma cláusula encontrada para ser resumida");
            }

            foreach (var clausula in clausulas)
            {
                var resultResumir = clausula.Resumir();
                if (resultResumir.IsFailure)
                {
                    return resultResumir;
                }

                await _clausulaGeralRepository.AtualizarAsync(clausula);

                await CommitAsync(cancellationToken);
            }

            var usuario = await _observationUsuarioLogadoFactory.PorEmail();

            if (usuario.IsFailure)
            {
                return usuario;
            }

            ResumirClausulasEvent evento = new()
            {
                DocumentoId = request.DocumentoId,
                UsuarioId = usuario.Value.Id
            };

            var result = await _messagePublisher.SendAsync(evento, cancellationToken);
            if (result.IsFailure)
            {
                return result;
            }

            return Result.Success();
        }
    }
}
