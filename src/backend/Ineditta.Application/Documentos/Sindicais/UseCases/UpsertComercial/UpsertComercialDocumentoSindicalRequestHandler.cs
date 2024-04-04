using CSharpFunctionalExtensions;

using Ineditta.Application.Cnaes.Repositories;
using Ineditta.Application.Localizacoes.Repositories;
using Ineditta.Application.Sindicatos.Laborais.Repositories;
using Ineditta.Application.Sindicatos.Patronais.Repositories;
using Ineditta.BuildingBlocks.Core.Database;
using Ineditta.BuildingBlocks.Core.Domain.Handlers;

using MediatR;
using Ineditta.BuildingBlocks.Core.Auth;
using Ineditta.Application.Documentos.Estabelecimentos.Entities;
using Ineditta.Application.Documentos.Estabelecimentos.Repositories;
using Ineditta.Application.Documentos.Localizacoes.Repositories;
using Ineditta.Application.Documentos.Localizacoes.Entities;
using Ineditta.Application.Documentos.Sindicais.Dtos;
using Ineditta.Application.Documentos.Sindicais.Entities;
using Ineditta.Application.Documentos.Sindicais.Repositories;
using Ineditta.Application.Documentos.SindicatosLaborais;
using Ineditta.Application.Documentos.SindicatosLaborais.Repositories;
using Ineditta.Application.Documentos.SindicatosPatronais;
using Ineditta.Application.Documentos.SindicatosPatronais.Repositories;
using Ineditta.Application.Documentos.Sindicais.Events.PreencherDocumentoEstabelecimento;
using Ineditta.BuildingBlocks.Core.Bus;

namespace Ineditta.Application.Documentos.Sindicais.UseCases.UpsertComercial
{
    public class UpsertComercialDocumentoSindicalRequestHandler : BaseCommandHandler, IRequestHandler<UpsertComercialDocumentoSindicalRequest, Result<int>>
    {
        private readonly IDocumentoSindicalRepository _documentoSindicalRepository;
        private readonly IDocumentoSindicatoLaboralRepository _documentoSindicatoLaboralRepository;
        private readonly IDocumentoSindicatoPatronalRepository _documentoSindicatoPatronalRepository;
        private readonly IDocumentoEstabelecimentoRepository _documentoEstabelecimentoRepository;
        private readonly IDocumentoEstabelecimentoCruzamentosRepository _documentoEstabelecimentoCruzamentosRepository;
        private readonly IDocumentoLocalizacaoRepository _documentoLocalizacaoRepository;
        private readonly ISindicatoLaboralRepository _sindicatoLaboralRepository;
        private readonly ISindicatoPatronalRepository _sindicatoPatronalRepository;
        private readonly ICnaeRepository _cnaeRepository;
        private readonly ILocalizacaoRepository _localizacaoRepository;
        private readonly IUserInfoService _userInfoService;
        private readonly IMessagePublisher _messagePublisher;

        public UpsertComercialDocumentoSindicalRequestHandler(IUnitOfWork unitOfWork, IDocumentoEstabelecimentoCruzamentosRepository documentoEstabelecimentoCruzamentosRepository, ISindicatoLaboralRepository sindicatoLaboralRepository, ISindicatoPatronalRepository sindicatoPatronalRepository, ICnaeRepository cnaeRepository, ILocalizacaoRepository localizacaoRepository, IDocumentoSindicalRepository documentoSindicalRepository, IDocumentoSindicatoLaboralRepository documentoSindicatoLaboralRepository, IDocumentoSindicatoPatronalRepository documentoSindicatoPatronalRepository, IDocumentoEstabelecimentoRepository documentoEstabelecimentoRepository, IDocumentoLocalizacaoRepository documentoLocalizacaoRepository, IUserInfoService userInfoService, IMessagePublisher messagePublisher) : base(unitOfWork)
        {
            _documentoEstabelecimentoCruzamentosRepository = documentoEstabelecimentoCruzamentosRepository;
            _sindicatoLaboralRepository = sindicatoLaboralRepository;
            _sindicatoPatronalRepository = sindicatoPatronalRepository;
            _cnaeRepository = cnaeRepository;
            _localizacaoRepository = localizacaoRepository;
            _documentoSindicalRepository = documentoSindicalRepository;
            _documentoSindicatoLaboralRepository = documentoSindicatoLaboralRepository;
            _documentoSindicatoPatronalRepository = documentoSindicatoPatronalRepository;
            _documentoEstabelecimentoRepository = documentoEstabelecimentoRepository;
            _documentoLocalizacaoRepository = documentoLocalizacaoRepository;
            _userInfoService = userInfoService;
            _messagePublisher = messagePublisher;
        }

        public async Task<Result<int>> Handle(UpsertComercialDocumentoSindicalRequest request, CancellationToken cancellationToken)
        {
            return await Inserir(request, cancellationToken);
        }

        private async Task<Result<int>> Inserir(UpsertComercialDocumentoSindicalRequest request, CancellationToken cancellationToken)
        {
            var emailUsuario = _userInfoService.GetEmail() ?? "";

            IEnumerable<Estabelecimento> estabelecimentos = new List<Estabelecimento>();
            var sindicatosLaborais = new List<SindicatoLaboral>();
            var sindicatosPatronais = new List<SindicatoPatronal>();
            var cnaes = new List<Cnae>();
            var abrangencias = new List<Abrangencia>();

            if (!(request.ClienteEstabelecimento is null || !request.ClienteEstabelecimento.Any()))
            {
                estabelecimentos = await _documentoEstabelecimentoCruzamentosRepository.ObterEstabelecimentosDocumentoPorIds(request.ClienteEstabelecimento!) ?? new List<Estabelecimento>();
            }

            if (request.SindLaboral is not null && request.SindLaboral.Any())
            {
                var sindicatosLaboraisResult = await _sindicatoLaboralRepository.ObterPorListaIdsAsync(request.SindLaboral);

                if (sindicatosLaboraisResult is not null && sindicatosLaboraisResult.Any())
                {
                    sindicatosLaborais = sindicatosLaboraisResult?
                        .Select(sindicato => new SindicatoLaboral(sindicato.Id, sindicato.Uf, sindicato.Cnpj.Value, sindicato.Sigla, sindicato.Municipio, sindicato.CodigoSindical.Valor))?
                        .ToList();
                }
            }

            if (request.SindPatronal is not null && request.SindPatronal.Any())
            {
                var sindicatosPatronaisResult = await _sindicatoPatronalRepository.ObterPorListaIdAsync(request.SindPatronal);

                if (sindicatosPatronaisResult is not null && sindicatosPatronaisResult.Any())
                {
                    sindicatosPatronais = sindicatosPatronaisResult?
                        .Select(sindicato => new SindicatoPatronal(sindicato.Id, sindicato.Uf, sindicato.Cnpj.Value, sindicato.Sigla, sindicato.Municipio, sindicato.CodigoSindical.Valor))?
                        .ToList();
                }
            }

            if (request.CnaesIds is not null && request.CnaesIds.Any())
            {
                var cnaesResult = await _cnaeRepository.ObterPorListaIdAsync(request.CnaesIds);

                if (cnaesResult is not null && cnaesResult.Any())
                {
                    cnaes = cnaesResult?
                    .Select(c => new Cnae(c.Id, c.DescricaoSubClasse))?
                        .ToList();
                }
            }

            if (request.Abrangencia is not null && request.Abrangencia.Any())
            {
                var abrangenciaResult = await _localizacaoRepository.ObterPorListaIdAsync(request.Abrangencia);

                if (abrangenciaResult is not null && abrangenciaResult.Any())
                {
                    abrangencias = abrangenciaResult?
                        .Select(abrangencia => new Abrangencia(abrangencia.Id, abrangencia.Uf.Id, abrangencia.Municipio))?
                        .ToList();
                }
            }

            var novoDocumentSindical = DocumentoSindical
                .Criar("", request.ValidadeInicial, request.ValidadeFinal, null, request.Tipo,
                       request.Origem, null, null, null, null,
                       request.Permissao, request.Observacao, null, request.Restrito,
                       estabelecimentos, request.Referencia, sindicatosLaborais, sindicatosPatronais, cnaes, abrangencias,
                       TipoModulo.Comercial, null, request.CaminhoArquivo, null, null, request.NomeArquivo, "", request.NumeroLei, request.FonteLei);

            if (novoDocumentSindical.IsFailure)
            {
                return Result.Failure<int>(novoDocumentSindical.Error);
            }

            var transactionResult = await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                await _documentoSindicalRepository.IncluirAsync(novoDocumentSindical.Value, cancellationToken);
                await CommitAsync(cancellationToken);

                await _documentoSindicatoPatronalRepository.DeletarPorDocumentoIdAsync(novoDocumentSindical.Value.Id);
                await _documentoSindicatoLaboralRepository.DeletarPorDocumentoIdAsync(novoDocumentSindical.Value.Id);

                if (request.SindPatronal is not null && request.SindPatronal.Any())
                {
                    foreach (var sindPatronalId in request.SindPatronal)
                    {
                        var criarDocumentoSindicatoPatronalResult = DocumentoSindicatoPatronal.Criar(novoDocumentSindical.Value.Id, sindPatronalId);
                        if (criarDocumentoSindicatoPatronalResult.IsFailure) return criarDocumentoSindicatoPatronalResult;
                        await _documentoSindicatoPatronalRepository.InserirAsync(criarDocumentoSindicatoPatronalResult.Value);
                    }
                }

                if (request.SindLaboral is not null && request.SindLaboral.Any())
                {
                    foreach (var sindLaboralId in request.SindLaboral)
                    {
                        var criarDocumentoSindicatoLaboralResult = DocumentoSindicatoLaboral.Criar(novoDocumentSindical.Value.Id, sindLaboralId);
                        if (criarDocumentoSindicatoLaboralResult.IsFailure) return criarDocumentoSindicatoLaboralResult;
                        await _documentoSindicatoLaboralRepository.InserirAsync(criarDocumentoSindicatoLaboralResult.Value);
                    }
                }

                if (!estabelecimentos.Any())
                {
                    var preencherDocumentoEstabelecimentoRequest = new PreencherDocumentoEstabelecimentoEvent(
                        novoDocumentSindical.Value.Id,
                        request.CnaesIds,
                        request.Abrangencia,
                        request.SindLaboral,
                        request.SindPatronal,
                        emailUsuario,
                        request.UsuariosParaNotificarIds
                    );

                    await _messagePublisher.SendAsync(preencherDocumentoEstabelecimentoRequest, cancellationToken);
                }

                foreach (var estabelecimento in estabelecimentos)
                {
                    var criarDocumentoEstabelecimentoResult = DocumentoEstabelecimento.Criar(novoDocumentSindical.Value.Id, estabelecimento.Id);
                    if (criarDocumentoEstabelecimentoResult.IsFailure) return criarDocumentoEstabelecimentoResult;
                    await _documentoEstabelecimentoRepository.InserirAsync(criarDocumentoEstabelecimentoResult.Value);
                }

                foreach (int localizacaoId in request.Abrangencia ?? new List<int>())
                {
                    var criarDocumentoLocalizacaoResult = DocumentoLocalizacao.Criar(novoDocumentSindical.Value.Id, localizacaoId);
                    if (criarDocumentoLocalizacaoResult.IsFailure) return criarDocumentoLocalizacaoResult;
                    await _documentoLocalizacaoRepository.InserirAsync(criarDocumentoLocalizacaoResult.Value);
                }

                await CommitAsync(cancellationToken);

                return Result.Success();
            }, cancellationToken);

            if (transactionResult.IsFailure) return Result.Failure<int>(transactionResult.Error);

            return Result.Success(novoDocumentSindical.Value.Id);
        }
    }
}
