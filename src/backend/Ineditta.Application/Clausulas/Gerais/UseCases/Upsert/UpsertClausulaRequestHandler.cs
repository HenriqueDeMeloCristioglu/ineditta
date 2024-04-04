using CSharpFunctionalExtensions;

using Ineditta.Application.Clausulas.Gerais.Entities;
using Ineditta.Application.Clausulas.Gerais.Repositiories;
using Ineditta.Application.InformacoesAdicionais.Sisap.UseCases.UpsertMany;
using Ineditta.Application.Usuarios.Factories;
using Ineditta.BuildingBlocks.Core.Database;
using Ineditta.BuildingBlocks.Core.Domain.Handlers;

using MediatR;

namespace Ineditta.Application.Clausulas.Gerais.UseCases.Upsert
{
    public class UpsertClausulaRequestHandler : BaseCommandHandler, IRequestHandler<UpsertClausulaRequest, Result>
    {
        private readonly IClausulaGeralRepository _clausulaGeralRepository;
        private readonly IMediator _mediator;
        private readonly ObterUsuarioLogadoFactory _obterUsuarioLogadoFactory;

        public UpsertClausulaRequestHandler(IUnitOfWork unitOfWork, IClausulaGeralRepository clausulaGeralRepository, IMediator mediator, ObterUsuarioLogadoFactory obterUsuarioLogadoFactory) : base(unitOfWork)
        {
            _clausulaGeralRepository = clausulaGeralRepository;
            _mediator = mediator;
            _obterUsuarioLogadoFactory = obterUsuarioLogadoFactory;
        }

        public async Task<Result> Handle(UpsertClausulaRequest request, CancellationToken cancellationToken)
        {
            return request.Id > 0 ? await Atualizar(request, cancellationToken) : await Incluir(request, cancellationToken);
        }

        public async Task<Result> Incluir(UpsertClausulaRequest request, CancellationToken cancellationToken)
        {
            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                var usuario = await _obterUsuarioLogadoFactory.PorEmail();

                if (usuario.IsFailure)
                {
                    return usuario;
                }

                var clausula = ClausulaGeral.Criar(request.Texto, request.DocumentoSindicalId, request.EstruturaClausulaId, request.Numero, request.AssuntoId, request.SinonimoId, usuario.Value.Id, request.TextoResumido, request.ConstaNoDocumento ?? true, false);

                if (clausula.IsFailure)
                {
                    return clausula;
                }

                await _clausulaGeralRepository.IncluirAsync(clausula.Value);

                await CommitAsync(cancellationToken);

                if (request.InformacoesAdicionais is not null && request.InformacoesAdicionais.Any())
                {
                    List<InformacaoAdicionalSisapRequestItem> informacoesAdicionais = new();

                    foreach (var informacaoAdicional in request.InformacoesAdicionais)
                    {
                        var sequenciaLinha = 0;

                        if (int.TryParse(informacaoAdicional.SequenciaLinha, out int linha))
                        {
                            sequenciaLinha = linha;
                        }

                        informacoesAdicionais.Add(new InformacaoAdicionalSisapRequestItem
                        {
                            Id = informacaoAdicional.Id,
                            DocumentoSindicalId = informacaoAdicional.DocumentoId,
                            EstruturaClausulaId = informacaoAdicional.EstruturaId,
                            ClausulaGeralId = clausula.Value.Id,
                            InforamcacaoAdicionalGrupoId = informacaoAdicional.GrupoId,
                            NomeInformacaoEstruturaClausulaId = informacaoAdicional.Nome,
                            Combo = informacaoAdicional.Combo,
                            Data = informacaoAdicional.Data,
                            Descricao = informacaoAdicional.Descricao,
                            Hora = informacaoAdicional.Hora,
                            Numerico = informacaoAdicional.Numerico,
                            Percentual = informacaoAdicional.Percentual,
                            SequenciaItem = informacaoAdicional.SequenciaItem,
                            SequenciaLinha = sequenciaLinha,
                            Texto = informacaoAdicional.Texto,
                            TipoinformacaoadicionalId = informacaoAdicional.Codigo
                        });
                    }

                    await _mediator.Send(new UpsertManyInformacaoAdicionalSisapRequest
                    {
                        InformacoesAdicionais = informacoesAdicionais
                    });

                    await CommitAsync(cancellationToken);
                }

                return Result.Success();
            }, cancellationToken);

            return Result.Success();
        }

        public async Task<Result> Atualizar(UpsertClausulaRequest request, CancellationToken cancellationToken)
        {
            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                var clausulaGeral = await _clausulaGeralRepository.ObterPorId(request.Id);

                if (clausulaGeral == null)
                {
                    return Result.Failure("Cláusula não encontrada");
                }

                var result = clausulaGeral.Atualizar(request.Texto, request.DocumentoSindicalId, request.EstruturaClausulaId, request.Numero, request.AssuntoId, request.SinonimoId, request.ConstaNoDocumento ?? true);

                if (result.IsFailure)
                {
                    return result;
                }

                await _clausulaGeralRepository.AtualizarAsync(clausulaGeral);

                await CommitAsync(cancellationToken);

                if (request.InformacoesAdicionais is not null && request.InformacoesAdicionais.Any())
                {
                    List<InformacaoAdicionalSisapRequestItem> informacoesAdicionais = new();

                    foreach (var informacaoAdicional in request.InformacoesAdicionais)
                    {
                        var sequenciaLinha = 0;

                        if (int.TryParse(informacaoAdicional.SequenciaLinha, out int linha))
                        {
                            sequenciaLinha = linha;
                        }

                        informacoesAdicionais.Add(new InformacaoAdicionalSisapRequestItem
                        {
                            Id = informacaoAdicional.Id,
                            DocumentoSindicalId = informacaoAdicional.DocumentoId,
                            EstruturaClausulaId = informacaoAdicional.EstruturaId,
                            ClausulaGeralId = clausulaGeral.Id,
                            InforamcacaoAdicionalGrupoId = informacaoAdicional.GrupoId,
                            NomeInformacaoEstruturaClausulaId = informacaoAdicional.Nome,
                            Combo = informacaoAdicional.Combo,
                            Data = informacaoAdicional.Data,
                            Descricao = informacaoAdicional.Descricao,
                            Hora = informacaoAdicional.Hora,
                            Numerico = informacaoAdicional.Numerico,
                            Percentual = informacaoAdicional.Percentual,
                            SequenciaItem = informacaoAdicional.SequenciaItem,
                            SequenciaLinha = sequenciaLinha,
                            Texto = informacaoAdicional.Texto,
                            TipoinformacaoadicionalId = informacaoAdicional.Codigo
                        });
                    }

                    await _mediator.Send(new UpsertManyInformacaoAdicionalSisapRequest
                    {
                        InformacoesAdicionais = informacoesAdicionais,
                        ClausulaId = clausulaGeral.Id
                    });

                    await CommitAsync(cancellationToken);
                }

                return Result.Success();
            }, cancellationToken);

            return Result.Success();
        }
    }
}
