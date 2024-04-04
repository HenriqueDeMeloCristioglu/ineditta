using CSharpFunctionalExtensions;

using Ineditta.Application.AIs.DocumentosSindicais.Repositories;
using Ineditta.Application.ClientesMatriz.Repositories;
using Ineditta.Application.Documentos.Sindicais.Events.DocumentoSisapCriadoJobIa;
using Ineditta.Application.Documentos.Sindicais.Factories;
using Ineditta.Application.Documentos.Sindicais.Repositories;
using Ineditta.Application.Usuarios.Factories;
using Ineditta.BuildingBlocks.Core.Bus;
using Ineditta.BuildingBlocks.Core.Database;
using Ineditta.BuildingBlocks.Core.Domain.Handlers;

using MediatR;

namespace Ineditta.Application.Documentos.Sindicais.UseCases.Aprovar
{
    public class AprovarDocumentoSindicalRequestHandler : BaseCommandHandler, IRequestHandler<AprovarDocumentoSindicalRequest, Result>
    {
        private readonly IDocumentoSindicalRepository _documentoSindicalRepository;
        private readonly IClienteMatrizRepository _clienteMatrizRepository;
        private readonly ObterUsuarioLogadoFactory _obterUsuarioLogadoFactory;
        private readonly IMessagePublisher _messagePublisher;
        private readonly IIADocumentoSindicalRepository _iADocumentoSindicalRepository;

        public AprovarDocumentoSindicalRequestHandler(IUnitOfWork unitOfWork, IDocumentoSindicalRepository documentoSindicalRepository, IClienteMatrizRepository clienteMatrizRepository, ObterUsuarioLogadoFactory obterUsuarioLogadoFactory, IMessagePublisher messagePublisher, IIADocumentoSindicalRepository iADocumentoSindicalRepository) : base(unitOfWork)
        {
            _documentoSindicalRepository = documentoSindicalRepository;
            _clienteMatrizRepository = clienteMatrizRepository;
            _obterUsuarioLogadoFactory = obterUsuarioLogadoFactory;
            _messagePublisher = messagePublisher;
            _iADocumentoSindicalRepository = iADocumentoSindicalRepository;
        }

        public async Task<Result> Handle(AprovarDocumentoSindicalRequest request, CancellationToken cancellationToken)
        {
            if (request.Id <= 0) return Result.Failure("O id deve ser maior ou igual a 0");

            var documentoSindical = await _documentoSindicalRepository.ObterPorIdAsync(request.Id);

            if (documentoSindical is null) return Result.Failure("Documento a ser aprovado não encontrado.");

            IEnumerable<int>? listaMatrizIds = null;

            if (documentoSindical.Estabelecimentos is not null && documentoSindical.Estabelecimentos.Any())
            {
                listaMatrizIds = documentoSindical.Estabelecimentos.Select(e => e.EmpresaId).Distinct();
            }
            else
            {
                return Result.Failure("Não foram encontrados estabelecimentos vinculados a este documento.");
            }

            if (listaMatrizIds is null) return Result.Failure("Nenhuma matriz identificada nos estabelecimentos, favor verificar os dados");

            var menorDataCorte = await _clienteMatrizRepository.ObterMenorDataCortePagamento(listaMatrizIds, cancellationToken);

            var dataSla = DataSlaFactory.Criar(menorDataCorte);

            var usuarioAprovador = await _obterUsuarioLogadoFactory.PorEmail();
            if (usuarioAprovador.IsFailure)
            {
                return usuarioAprovador;
            }

            var resultAprovar = documentoSindical.Aprovar(dataSla, usuarioAprovador.Value.Id);

            if (resultAprovar.IsFailure)
            {
                return resultAprovar;
            }

            await _documentoSindicalRepository.AtualizarAsync(documentoSindical, cancellationToken);
            _ = await CommitAsync(cancellationToken);

            bool scrapJaIniciadoNesseDocumento = await _iADocumentoSindicalRepository.ExistePorDocumentoReferenciaIdAsync(documentoSindical.Id);
            if (scrapJaIniciadoNesseDocumento) return Result.Success();

            DocumentoSisapCriadoJobIaEvent message = new(documentoSindical.Id);
            await _messagePublisher.SendAsync(message, cancellationToken);

            return Result.Success();
        }
    }
}
