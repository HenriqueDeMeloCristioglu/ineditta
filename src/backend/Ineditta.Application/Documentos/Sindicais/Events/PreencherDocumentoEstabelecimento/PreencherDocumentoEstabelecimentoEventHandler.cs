using CSharpFunctionalExtensions;

using Ineditta.Application.CalendarioSindicais.Eventos.UseCases.Notificar;
using Ineditta.Application.Documentos.Estabelecimentos.Entities;
using Ineditta.Application.Documentos.Estabelecimentos.Repositories;
using Ineditta.Application.Documentos.Sindicais.Dtos;
using Ineditta.Application.Documentos.Sindicais.Events.DocumentoSisapCriadoJobIa;
using Ineditta.Application.Documentos.Sindicais.Repositories;
using Ineditta.Application.Documentos.Sindicais.UseCases.NotificarCriacao;
using Ineditta.BuildingBlocks.Core.Database;

using MediatR;

using Org.BouncyCastle.Asn1.Ocsp;

namespace Ineditta.Application.Documentos.Sindicais.Events.PreencherDocumentoEstabelecimento
{
#pragma warning disable CA1711 // Identifiers should not have incorrect suffix
    public class PreencherDocumentoEstabelecimentoEventHandler : BuildingBlocks.Core.Bus.IRequestHandler<PreencherDocumentoEstabelecimentoEvent>
    {
        private readonly IDocumentoSindicalRepository _documentoSindicalRepository;
        private readonly IDocumentoEstabelecimentoCruzamentosRepository _documentoEstabelecimentoCruzamentosRepository;
        private readonly IDocumentoEstabelecimentoRepository _documentoEstabelecimentoRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediator _mediator;

        public PreencherDocumentoEstabelecimentoEventHandler(IDocumentoSindicalRepository documentoSindicalRepository, IDocumentoEstabelecimentoCruzamentosRepository documentoEstabelecimentoCruzamentosRepository, IUnitOfWork unitOfWork, IMediator mediator, IDocumentoEstabelecimentoRepository documentoEstabelecimentoRepository)
        {
            _documentoSindicalRepository = documentoSindicalRepository;
            _documentoEstabelecimentoCruzamentosRepository = documentoEstabelecimentoCruzamentosRepository;
            _unitOfWork = unitOfWork;
            _mediator = mediator;
            _documentoEstabelecimentoRepository = documentoEstabelecimentoRepository;
        }

        public async ValueTask<Result> Handle(PreencherDocumentoEstabelecimentoEvent message, CancellationToken cancellationToken = default)
        {
            var documento = await _documentoSindicalRepository.ObterPorIdAsync(message.DocumentoId);
            if (documento == null)
            {
                throw new ArgumentException("Documento não encontrado: id - " + message.DocumentoId);
            }

            var estabelecimentos = await _documentoEstabelecimentoCruzamentosRepository.ObterEstabelecimentosDocumentoComercialPorCruzamento(
                message.CnaesIds,
                message.LocalizacoesIds,
                message.SindicatosPatronaisIds,
                message.SindicatosLaboraisIds,
                message.EmailUsuario
            ) ?? new List<Estabelecimento>();

            documento.AtualizarEstabelecimentos(estabelecimentos.ToList());
            
            await _documentoSindicalRepository.AtualizarAsync(documento, cancellationToken);

            foreach (var estabelecimento in estabelecimentos)
            {
                var criarDocumentoEstabelecimentoResult = DocumentoEstabelecimento.Criar(message.DocumentoId, estabelecimento.Id);
                if (criarDocumentoEstabelecimentoResult.IsFailure) return criarDocumentoEstabelecimentoResult;
                await _documentoEstabelecimentoRepository.InserirAsync(criarDocumentoEstabelecimentoResult.Value);
            }

            await _unitOfWork.CommitAsync(cancellationToken);

            var notificarEmailRequest = new NotificarCriacaoRequest
            {
                DocumentoId = message.DocumentoId,
                UsuariosParaNotificarIds = message.UsuariosParaNotificarIds ?? new List<long>()
            };

            await _mediator.Send(notificarEmailRequest, cancellationToken);

            return Result.Success();
        }
    }
}
