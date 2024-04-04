using CSharpFunctionalExtensions;

using Ineditta.Application.ClientesMatriz.Entities;
using Ineditta.Application.ClientesMatriz.Repositories;
using Ineditta.Application.GruposEconomicos.Repositories;
using Ineditta.Application.Modulos.Repositories;
using Ineditta.Application.ModulosClientes.Entities;
using Ineditta.Application.ModulosClientes.Repositories;
using Ineditta.Application.TiposDocumentos.Repositories;
using Ineditta.Application.TiposDocumentosClientesMatriz.Entities;
using Ineditta.Application.TiposDocumentosClientesMatriz.repositories;
using Ineditta.BuildingBlocks.Core.Database;
using Ineditta.BuildingBlocks.Core.Domain.Handlers;
using Ineditta.BuildingBlocks.Core.FileStorage;

using MediatR;

using Microsoft.AspNetCore.Http;

namespace Ineditta.Application.ClientesMatriz.UseCases.Upsert
{
    public class UpsertClienteMatrizRequestHandler : BaseCommandHandler, IRequestHandler<UpsertClienteMatrizRequest, Result>
    {
        private readonly IClienteMatrizRepository _clienteMatrizRepository;
        private readonly IGrupoEconomicoRepository _grupoEconomicoRepository;
        private readonly ITipoDocumentoRepository _tipoDocumentoRepository;
        private readonly IModuloClienteRepository _moduloClienteRepository;
        private readonly IModuloRepository _moduloRepository;
        private readonly ITipoDocumentoClienteMatrizRepository _tipoDocumentoClienteMatrizRepository;
        private readonly IFileStorage _fileStorage;
        public UpsertClienteMatrizRequestHandler(IUnitOfWork unitOfWork, IClienteMatrizRepository clienteMatrizRepository, IGrupoEconomicoRepository grupoEconomicoRepository, ITipoDocumentoRepository tipoDocumentoRepository, IModuloClienteRepository moduloClienteRepository, ITipoDocumentoClienteMatrizRepository tipoDocumentoClienteMatrizRepository, IModuloRepository moduloRepository, IFileStorage fileStorage) : base(unitOfWork)
        {
            _tipoDocumentoRepository = tipoDocumentoRepository;
            _clienteMatrizRepository = clienteMatrizRepository;
            _grupoEconomicoRepository = grupoEconomicoRepository;
            _moduloClienteRepository = moduloClienteRepository;
            _tipoDocumentoClienteMatrizRepository = tipoDocumentoClienteMatrizRepository;
            _moduloRepository = moduloRepository;
            _fileStorage = fileStorage;
        }
        public async Task<Result> Handle(UpsertClienteMatrizRequest request, CancellationToken cancellationToken)
        {
            if (request.Id is not null)
            {
                return await AtualizarAsync(request, cancellationToken);
            }
            return await IncluirAsync(request, cancellationToken);
        }

        private async Task<Result> IncluirAsync(UpsertClienteMatrizRequest request, CancellationToken cancellationToken)
        {
            var uploadResult = await UploadLogo(request.Logo, cancellationToken);
            if (uploadResult.IsFailure)
            {
                return uploadResult;
            }

            string? logo = uploadResult.Value;

            var grupoEconomico = await _grupoEconomicoRepository.ObterPorIdAsync(request.GrupoEconomicoId);
            if (grupoEconomico is null) return Result.Failure("O grupo econômico de id '" + request.GrupoEconomicoId + "' não foi encontrado");

            var tiposDocumentos = await _tipoDocumentoRepository.ObterPorListaIds(request.TiposDocumentos);
            if (tiposDocumentos is null) return Result.Failure("Lista de tipos de documentos não recebida.");

            if (tiposDocumentos.Count() < request.TiposDocumentos.Count())
            {
                return Result.Failure("Algum(ns) dos tipos fornecidos não foram encontrados na base -- operação cancelada.");
            }

            var modulos = await _moduloRepository.ObterPorListaIds(request.ModulosIds ?? new List<int>());
            if (modulos is null) return Result.Failure("Lista de modulos não recebida ou não encontrada na base.");

            if (modulos.Count() < (request.ModulosIds is null ? 0 : request.ModulosIds.Count()))
            {
                return Result.Failure("Algum(ns) dos módulos fornecidos não foram encontrados na base -- operação cancelada.");
            }

            var result = await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                var clienteMatriz = ClienteMatriz.Criar(
                    request.Codigo,
                    request.Nome,
                    request.AberturaNegociacao,
                    request.SlaPrioridade,
                    request.DataCorteForpag,
                    request.TipoProcessamento,
                    request.GrupoEconomicoId,
                    logo
                );

                if (clienteMatriz.IsFailure)
                {
                    return clienteMatriz;
                }

                await _clienteMatrizRepository.IncluirAsync(clienteMatriz.Value);

                await CommitAsync();

                if (modulos is not null && modulos.Any())
                {
                    foreach (var modulo in modulos)
                    {
                        var moduloCliente = ModuloCliente.Criar(
                            DateOnly.FromDateTime(DateTime.Today),
                            modulo.Id,
                            clienteMatriz.Value.Id
                        );

                        if (moduloCliente.IsFailure)
                        {
                            return moduloCliente;
                        }

                        await _moduloClienteRepository.IncluirAsync(moduloCliente.Value);

                        await CommitAsync();
                    }
                }

                if (tiposDocumentos is not null && tiposDocumentos.Any())
                {
                    foreach (var tipoDocumento in tiposDocumentos)
                    {
                        var tipoDocumentoClienteMatriz = TipoDocumentoClienteMatriz.Criar(
                            tipoDocumento.Id,
                            clienteMatriz.Value.Id
                        );

                        if (tipoDocumentoClienteMatriz.IsFailure)
                        {
                            return tipoDocumentoClienteMatriz;
                        }

                        await _tipoDocumentoClienteMatrizRepository.IncluirAsync(tipoDocumentoClienteMatriz.Value);

                        await CommitAsync();
                    }
                }

                return Result.Success();
            }, cancellationToken);

            return result;
        }

        private async Task<Result> AtualizarAsync(UpsertClienteMatrizRequest request, CancellationToken cancellationToken)
        {
            var uploadResult = await UploadLogo(request.Logo, cancellationToken);
            if (uploadResult.IsFailure)
            {
                return uploadResult;
            }

            string? logo = uploadResult.Value;

            var clienteMatriz = await _clienteMatrizRepository.ObterPorId(request.Id ?? 0, cancellationToken);
            if (clienteMatriz is null) return Result.Failure("Cliente Matriz a ser atualizado não encontrado no sistema");

            var grupoEconomico = await _grupoEconomicoRepository.ObterPorIdAsync(request.GrupoEconomicoId);
            if (grupoEconomico is null) return Result.Failure("O grupo econômico de id '" + request.GrupoEconomicoId + "' não foi encontrado");

            var tiposDocumentos = await _tipoDocumentoRepository.ObterPorListaIds(request.TiposDocumentos);
            if (tiposDocumentos is null) return Result.Failure("Lista de tipos de documentos não recebida.");

            if (tiposDocumentos.Count() < request.TiposDocumentos.Count())
            {
                return Result.Failure("Algum(ns) dos tipos fornecidos não foram encontrados na base -- operação cancelada.");
            }

            var modulos = await _moduloRepository.ObterPorListaIds(request.ModulosIds ?? new List<int>());
            if (modulos is null) return Result.Failure("Lista de modulos não recebida ou não encontrada na base.");

            if (modulos.Count() < (request.ModulosIds is null ? 0 : request.ModulosIds.Count()))
            {
                return Result.Failure("Algum(ns) dos módulos fornecidos não foram encontrados na base -- operação cancelada.");
            }

            var moduloClientes = (await _moduloClienteRepository.ObterVigentesPorIdClienteMatriz(request.Id ?? 0)) ?? new List<ModuloCliente>();

            var modulosAdicionados = modulos.Where(m => !moduloClientes.Any(mc => mc.ModuloId == m.Id));
            var modulosRemovidos = moduloClientes.Where(mc => !modulos.Any(m => m.Id == mc.ModuloId));

            var result = await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                var updateResult = clienteMatriz.Atualizar(
                    request.Codigo,
                    request.Nome,
                    request.AberturaNegociacao,
                    request.SlaPrioridade,
                    request.DataCorteForpag,
                    request.TipoProcessamento,
                    request.GrupoEconomicoId,
                    logo ?? clienteMatriz.Logo
                );

                if (updateResult.IsFailure)
                {
                    return updateResult;
                }

                await _clienteMatrizRepository.AtualizarAsync(clienteMatriz);

                await CommitAsync();

                foreach (var moduloRemovido in modulosRemovidos)
                {
                    var finalizacaoResult = moduloRemovido.Finalizar();
                    if (finalizacaoResult.IsFailure)
                    {
                        return finalizacaoResult;
                    }

                    await _moduloClienteRepository.AtualizarAsync(moduloRemovido);
                    await CommitAsync();
                }

                foreach (var modulo in modulosAdicionados)
                {
                    var moduloCliente = ModuloCliente.Criar(
                        DateOnly.FromDateTime(DateTime.Today),
                        modulo.Id,
                        clienteMatriz.Id
                    );

                    if (moduloCliente.IsFailure)
                    {
                        return moduloCliente;
                    }

                    await _moduloClienteRepository.IncluirAsync(moduloCliente.Value);
                    await CommitAsync();
                }

                await _tipoDocumentoClienteMatrizRepository.LimparTodosPorMatrizIdAsync(clienteMatriz.Id);

                if (tiposDocumentos is not null && tiposDocumentos.Any())
                {
                    foreach (var tipoDocumento in tiposDocumentos)
                    {
                        var tipoDocumentoClienteMatriz = TipoDocumentoClienteMatriz.Criar(
                            tipoDocumento.Id,
                            clienteMatriz.Id
                        );

                        if (tipoDocumentoClienteMatriz.IsFailure)
                        {
                            return tipoDocumentoClienteMatriz;
                        }

                        await _tipoDocumentoClienteMatrizRepository.IncluirAsync(tipoDocumentoClienteMatriz.Value);
                        await CommitAsync();
                    }
                }

                return Result.Success();
            }, cancellationToken);

            return result;
        }

        private async Task<Result<string?>> UploadLogo(IFormFile? file, CancellationToken cancellationToken)
        {
            string? logo = null;

            if (file is not null)
            {
                Guid guid = Guid.NewGuid();

                byte[] bytes;

                using (MemoryStream ms = new MemoryStream())
                {
                    await file.CopyToAsync(ms, cancellationToken);
                    bytes = ms.ToArray();
                }
                var resultUpload = await _fileStorage.AddAsync(new FileDto(guid.ToString() + Path.GetExtension(file.FileName), bytes, ClienteMatriz.PastaDocumento), cancellationToken);

                if (resultUpload.IsFailure)
                {
                    return Result.Failure<string?>("Falha ao tentar realizar o upload do arquivo");
                }

                logo = resultUpload.Value.FileName;
            }

            return Result.Success(logo);
        }
    }
}
