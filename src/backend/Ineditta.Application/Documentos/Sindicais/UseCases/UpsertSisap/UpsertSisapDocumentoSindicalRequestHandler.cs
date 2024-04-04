using CSharpFunctionalExtensions;

using Ineditta.Application.Cnaes.Repositories;
using Ineditta.Application.Documentos.Estabelecimentos.Entities;
using Ineditta.Application.Documentos.Estabelecimentos.Repositories;
using Ineditta.Application.Documentos.Localizacoes.Entities;
using Ineditta.Application.Documentos.Localizacoes.Repositories;
using Ineditta.Application.Documentos.Localizados.Repositories;
using Ineditta.Application.Documentos.Sindicais.Dtos;
using Ineditta.Application.Documentos.Sindicais.Entities;
using Ineditta.Application.Documentos.Sindicais.Repositories;
using Ineditta.Application.Documentos.SindicatosLaborais;
using Ineditta.Application.Documentos.SindicatosLaborais.Repositories;
using Ineditta.Application.Documentos.SindicatosPatronais;
using Ineditta.Application.Documentos.SindicatosPatronais.Repositories;
using Ineditta.Application.Localizacoes.Repositories;
using Ineditta.Application.Sindicatos.Laborais.Repositories;
using Ineditta.Application.Sindicatos.Patronais.Repositories;
using Ineditta.BuildingBlocks.Core.Database;
using Ineditta.BuildingBlocks.Core.Domain.Handlers;

using MediatR;

namespace Ineditta.Application.Documentos.Sindicais.UseCases.UpsertSisap
{
    public class UpsertSisapDocumentoSindicalRequestHandler : BaseCommandHandler, IRequestHandler<UpsertSisapDocumentoSindicalRequest, Result<int>>
    {
        private readonly IDocumentoLocalizadoRepository _documentoLocalizadoRepository;
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

        public UpsertSisapDocumentoSindicalRequestHandler(IUnitOfWork unitOfWork, IDocumentoLocalizadoRepository documentoLocalizadoRepository, IDocumentoSindicalRepository documentoSindicalRepository, ISindicatoLaboralRepository sindicatoLaboralRepository, ISindicatoPatronalRepository sindicatoPatronalRepository, ICnaeRepository cnaeRepository, ILocalizacaoRepository localizacaoRepository, IDocumentoEstabelecimentoRepository documentoEstabelecimentoRepository, IDocumentoSindicatoLaboralRepository documentoSindicatoLaboralRepository, IDocumentoSindicatoPatronalRepository documentoSindicatoPatronalRepository, IDocumentoEstabelecimentoCruzamentosRepository documentoEstabelecimentoCruzamentosRepository, IDocumentoLocalizacaoRepository documentoLocalizacaoRepository) : base(unitOfWork)
        {
            _documentoLocalizadoRepository = documentoLocalizadoRepository;
            _documentoSindicalRepository = documentoSindicalRepository;
            _sindicatoLaboralRepository = sindicatoLaboralRepository;
            _sindicatoPatronalRepository = sindicatoPatronalRepository;
            _cnaeRepository = cnaeRepository;
            _localizacaoRepository = localizacaoRepository;
            _documentoEstabelecimentoRepository = documentoEstabelecimentoRepository;
            _documentoSindicatoLaboralRepository = documentoSindicatoLaboralRepository;
            _documentoSindicatoPatronalRepository = documentoSindicatoPatronalRepository;
            _documentoEstabelecimentoCruzamentosRepository = documentoEstabelecimentoCruzamentosRepository;
            _documentoLocalizacaoRepository = documentoLocalizacaoRepository;
        }

        public async Task<Result<int>> Handle(UpsertSisapDocumentoSindicalRequest request, CancellationToken cancellationToken)
        {
            return request.IdDocSind is null ?
                await Insert(request, cancellationToken) :
                await Update(request, cancellationToken);
        }

        private async Task<Result<int>> Insert(UpsertSisapDocumentoSindicalRequest request, CancellationToken cancellationToken)
        {
            var documentoLocalizado = await _documentoLocalizadoRepository.ObterPorIdAsync(request.IdDocumento);

            if (documentoLocalizado is null)
            {
                return Result.Failure<int>("O id do documento localizado fornecido não foi encontrado");
            }

            IEnumerable<Estabelecimento> estabelecimentos = new List<Estabelecimento>();
            var sindicatosLaborais = new List<SindicatoLaboral>();
            var sindicatosPatronais = new List<SindicatoPatronal>();
            var cnaes = new List<Cnae>();
            var abrangencias = new List<Abrangencia>();

            if (request.ClienteEstabelecimento is null || !request.ClienteEstabelecimento.Any())
            {
                estabelecimentos = await _documentoEstabelecimentoCruzamentosRepository.ObterEstabelecimentosDocumentoPorCruzamento(
                    request.CnaesIds,
                    request.Abrangencia,
                    request.SindPatronal,
                    request.SindLaboral,
                    request.DataBase.Substring(0, 3)
                ) ?? new List<Estabelecimento>();
            }
            else
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
                .Criar(request.Versao, request.ValidadeInicial, request.ValidadeFinal, request.Prorrogacao, request.Tipo,
                       request.Origem, request.NumeroSolicitacaoMR, request.DataRegistro, request.NumeroRegistro, request.DataAssinatura,
                       request.Permissao, request.Observacao, request.IdDocumento, request.Restrito,
                       estabelecimentos, request.Referencia, sindicatosLaborais, sindicatosPatronais, cnaes, abrangencias,
                       TipoModulo.SISAP, documentoLocalizado.Uf, documentoLocalizado.CaminhoArquivo, null, request.TipoUnidadeCliente, request.NomeArquivo, request.DataBase, null, null);

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

                documentoLocalizado.Atualizar(
                    documentoLocalizado.NomeDocumento,
                    documentoLocalizado.Origem,
                    documentoLocalizado.CaminhoArquivo,
                    documentoLocalizado.Situacao,
                    documentoLocalizado.DataAprovacao,
                    true,
                    documentoLocalizado.IdLegado,
                    documentoLocalizado.Uf
                );

                await _documentoLocalizadoRepository.AtualizarAsync(documentoLocalizado);

                await CommitAsync(cancellationToken);

                return Result.Success();
            }, cancellationToken);

            if (transactionResult.IsFailure) return Result.Failure<int>(transactionResult.Error);

            return Result.Success(novoDocumentSindical.Value.Id);
        }

        private async Task<Result<int>> Update(UpsertSisapDocumentoSindicalRequest request, CancellationToken cancellationToken)
        {
            var docSind = await _documentoSindicalRepository.ObterPorIdAsync(request.IdDocSind ?? 0);

            if (docSind is null)
            {
                return Result.Failure<int>("O documento sindicical com o id fornecido não foi encontrado");
            }

            var documentoLocalizado = await _documentoLocalizadoRepository.ObterPorIdAsync(request.IdDocumento);

            if (documentoLocalizado is null)
            {
                return Result.Failure<int>("O id do documento localizado fornecido não foi encontrado");
            }

            if (string.IsNullOrEmpty(request.DataBase)) return Result.Failure<int>("Forneça a data-base do documento.");

            IEnumerable<Estabelecimento> estabelecimentos = new List<Estabelecimento>();
            var sindicatosLaborais = new List<SindicatoLaboral>();
            var sindicatosPatronais = new List<SindicatoPatronal>();
            var cnaes = new List<Cnae>();
            var abrangencias = new List<Abrangencia>();

            if (request.ClienteEstabelecimento is null || !request.ClienteEstabelecimento.Any())
            {
                estabelecimentos = await _documentoEstabelecimentoCruzamentosRepository.ObterEstabelecimentosDocumentoPorCruzamento(
                    request.CnaesIds,
                    request.Abrangencia,
                    request.SindPatronal,
                    request.SindLaboral,
                    request.DataBase.Substring(0, 3)
                ) ?? new List<Estabelecimento>();
            }
            else
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

            var atualizarResult = docSind.Atualizar(request.Versao, request.ValidadeInicial, request.ValidadeFinal, request.Prorrogacao, request.Tipo,
                       request.Origem, request.NumeroSolicitacaoMR, request.DataRegistro, request.NumeroRegistro, request.DataAssinatura,
                       request.Permissao, request.Observacao, request.IdDocumento, request.Restrito,
                       estabelecimentos, request.Referencia, sindicatosLaborais, sindicatosPatronais, cnaes, abrangencias,
                       TipoModulo.SISAP, documentoLocalizado.Uf, documentoLocalizado.CaminhoArquivo, request.TipoUnidadeCliente, request.DataBase);

            if (atualizarResult.IsFailure)
            {
                return Result.Failure<int>(atualizarResult.Error);
            }

            var transactionResult = await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                await _documentoSindicalRepository.AtualizarAsync(docSind, cancellationToken);
                await CommitAsync(cancellationToken);

                await _documentoSindicatoPatronalRepository.DeletarPorDocumentoIdAsync(docSind.Id);
                await _documentoSindicatoLaboralRepository.DeletarPorDocumentoIdAsync(docSind.Id);

                if (request.SindPatronal is not null && request.SindPatronal.Any())
                {
                    foreach (var sindPatronalId in request.SindPatronal)
                    {
                        var criarDocumentoSindicatoPatronalResult = DocumentoSindicatoPatronal.Criar(docSind.Id, sindPatronalId);
                        if (criarDocumentoSindicatoPatronalResult.IsFailure) return criarDocumentoSindicatoPatronalResult;
                        await _documentoSindicatoPatronalRepository.InserirAsync(criarDocumentoSindicatoPatronalResult.Value);
                    }
                }

                if (request.SindLaboral is not null && request.SindLaboral.Any())
                {
                    foreach (var sindLaboralId in request.SindLaboral)
                    {
                        var criarDocumentoSindicatoLaboralResult = DocumentoSindicatoLaboral.Criar(docSind.Id, sindLaboralId);
                        if (criarDocumentoSindicatoLaboralResult.IsFailure) return criarDocumentoSindicatoLaboralResult;
                        await _documentoSindicatoLaboralRepository.InserirAsync(criarDocumentoSindicatoLaboralResult.Value);
                    }
                }

                foreach (var estabelecimento in estabelecimentos)
                {
                    await _documentoEstabelecimentoRepository.RemoverTudoPorDocumentoId(docSind.Id);

                    var criarDocumentoEstabelecimentoResult = DocumentoEstabelecimento.Criar(docSind.Id, estabelecimento.Id);
                    if (criarDocumentoEstabelecimentoResult.IsFailure) return criarDocumentoEstabelecimentoResult;
                    await _documentoEstabelecimentoRepository.InserirAsync(criarDocumentoEstabelecimentoResult.Value);
                }

                foreach (int localizacaoId in request.Abrangencia ?? new List<int>())
                {
                    await _documentoLocalizacaoRepository.RemoverTudoPorDocumentoIdAsync(docSind.Id);

                    var criarDocumentoLocalizacaoResult = DocumentoLocalizacao.Criar(docSind.Id, localizacaoId);
                    if (criarDocumentoLocalizacaoResult.IsFailure) return criarDocumentoLocalizacaoResult;
                    await _documentoLocalizacaoRepository.InserirAsync(criarDocumentoLocalizacaoResult.Value);
                }

                await CommitAsync(cancellationToken);

                return Result.Success();
            }, cancellationToken);

            await CommitAsync(cancellationToken);

            if (transactionResult.IsFailure) return Result.Failure<int>(transactionResult.Error);

            return Result.Success(docSind.Id);
        }
    }
}