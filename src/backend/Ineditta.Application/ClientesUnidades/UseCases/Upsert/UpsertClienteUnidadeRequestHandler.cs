using CSharpFunctionalExtensions;

using Ineditta.Application.SharedKernel.Logradouros.ValueObjects;
using Ineditta.Application.ClientesUnidades.Entities;
using Ineditta.Application.ClientesUnidades.Repositories;
using Ineditta.BuildingBlocks.Core.Database;
using Ineditta.BuildingBlocks.Core.Domain.Handlers;
using Ineditta.BuildingBlocks.Core.Domain.ValueObjects;

using MediatR;

namespace Ineditta.Application.ClientesUnidades.UseCases
{
    public class UpsertClienteUnidadeRequestHandler : BaseCommandHandler, IRequestHandler<UpsertClienteUnidadeRequest, Result>
    {
        private readonly IClienteUnidadeRepository _clienteUnidadeRepository;

        public UpsertClienteUnidadeRequestHandler(IUnitOfWork unitOfWork, IClienteUnidadeRepository clienteUnidadeRepository) : base(unitOfWork)
        {
            _clienteUnidadeRepository = clienteUnidadeRepository;
        }

        public async Task<Result> Handle(UpsertClienteUnidadeRequest request, CancellationToken cancellationToken)
        {
            return request.Id > 0 ?
                await AtualizarAsync(request, cancellationToken) :
                await IncluirAsync(request, cancellationToken);
        }

        private async ValueTask<Result> AtualizarAsync(UpsertClienteUnidadeRequest request, CancellationToken cancellationToken)
        {
            var clienteUnidade = await _clienteUnidadeRepository.ObterPorIdAsync(request.Id);

            if (clienteUnidade is null)
            {
                return Result.Failure("Unidade não encontrada");
            }

            var cnpjResult = CNPJ.Criar(request.Cnpj);

            if (cnpjResult.IsFailure)
            {
                return cnpjResult;
            }

            CNPJ cnpj = cnpjResult.Value;


            var cepResult = CEP.Criar(request.Cep);

            if (cepResult.IsFailure)
            {
                return cepResult;
            }

            CEP cep = cepResult.Value;

            var logradouroResult = Logradouro.Criar(request.Endereco ?? string.Empty, request.Regiao ?? string.Empty, request.Bairro ?? string.Empty, cep);

            if (logradouroResult.IsFailure)
            {
                return logradouroResult;
            }

            Logradouro logradouro = logradouroResult.Value;

            var cnaesUnidades = new List<CnaeUnidade>(request.CnaesUnidade?.Count() ?? 1);

            if ((request.CnaesUnidade?.Any() ?? false))
            {
                foreach (var cnaeUnidadeRequest in request.CnaesUnidade)
                {
                    var cnaeUnidade = CnaeUnidade.Criar(cnaeUnidadeRequest.Id);

                    if (cnaeUnidade.IsFailure)
                    {
                        return cnaeUnidade;
                    }

                    cnaesUnidades.Add(cnaeUnidade.Value);
                }
            }

            var result = clienteUnidade.Atualizar(
                request.Codigo,
                request.Nome,
                cnpj,
                logradouro,
                request.DataAusencia,
                request.CodigoSindicatoCliente,
                request.CodigoSindicatoPatronal,
                request.EmpresaId,
                request.TipoNegocioId,
                request.LocalizacaoId,
                request.CnaeFilial,
                cnaesUnidades
            );

            if (result.IsFailure)
            {
                return result;
            }

            await _clienteUnidadeRepository.AtualizarAsync(clienteUnidade);

            _ = await CommitAsync(cancellationToken);

            return Result.Success();
        }

        private async Task<Result> IncluirAsync(UpsertClienteUnidadeRequest request, CancellationToken cancellationToken)
        {
            var cnpjResult = CNPJ.Criar(request.Cnpj);

            if (cnpjResult.IsFailure)
            {
                return cnpjResult;
            }

            CNPJ cnpj = cnpjResult.Value;


            var cepResult = CEP.Criar(request.Cep);

            if (cepResult.IsFailure)
            {
                return cepResult;
            }

            CEP cep = cepResult.Value;

            var logradouroResult = Logradouro.Criar(request.Endereco ?? string.Empty, request.Regiao ?? string.Empty, request.Bairro ?? string.Empty, cep);

            if (logradouroResult.IsFailure)
            {
                return logradouroResult;
            }

            Logradouro logradouro = logradouroResult.Value;

            var cnaesUnidades = new List<CnaeUnidade>(request.CnaesUnidade?.Count() ?? 1);

            if ((request.CnaesUnidade?.Any() ?? false))
            {
                foreach (var cnaeUnidadeRequest in request.CnaesUnidade)
                {
                    var cnaeUnidade = CnaeUnidade.Criar(cnaeUnidadeRequest.Id);

                    if (cnaeUnidade.IsFailure)
                    {
                        return cnaeUnidade;
                    }

                    cnaesUnidades.Add(cnaeUnidade.Value);
                }
            }

            var clienteUnidade = ClienteUnidade.Criar(
                request.Codigo,
                request.Nome,
                cnpj,
                logradouro,
                request.DataAusencia,
                request.CodigoSindicatoCliente,
                request.CodigoSindicatoPatronal,
                request.EmpresaId,
                request.TipoNegocioId,
                request.LocalizacaoId,
                request.CnaeFilial,
                cnaesUnidades
            );

            if (clienteUnidade.IsFailure)
            {
                return clienteUnidade;
            }

            await _clienteUnidadeRepository.IncluirAsync(clienteUnidade.Value);

            _ = await CommitAsync(cancellationToken);

            return Result.Success();
        }
    }
}
