using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CSharpFunctionalExtensions;

using Ineditta.Application.TiposDocumentos.Entities;
using Ineditta.Application.TiposDocumentos.Repositories;
using Ineditta.BuildingBlocks.Core.Bus;
using Ineditta.BuildingBlocks.Core.Database;
using Ineditta.BuildingBlocks.Core.Domain.Handlers;

using MediatR;

using Microsoft.EntityFrameworkCore.Update.Internal;

namespace Ineditta.Application.TiposDocumentos.UseCases.Upsert
{
    public class UpsertTipoDocumentoRequestHandler : BaseCommandHandler, IRequestHandler<UpsertTipoDocumentoRequest, Result<int>>
    {
        private readonly ITipoDocumentoRepository _tipoDocumentoRepository;

        public UpsertTipoDocumentoRequestHandler(IUnitOfWork unitOfWork, ITipoDocumentoRepository tipoDocumentoRepository) : base(unitOfWork)
        {
            _tipoDocumentoRepository = tipoDocumentoRepository;
        }

        public async Task<Result<int>> Handle(UpsertTipoDocumentoRequest request, CancellationToken cancellationToken)
        {
            return request.Id is null ?
                 await Insert(request, cancellationToken) :
                 await Update(request, cancellationToken);
        }

        private async Task<Result<int>> Insert(UpsertTipoDocumentoRequest request, CancellationToken cancellationToken)
        {
            var td = TipoDocumento.Criar(
                request.Tipo,
                request.Nome,
                request.Sigla,
                request.Processado?.ToLowerInvariant() == "s",
                request.Modulo
            );

            if (td.IsFailure)
            {
                return Result.Failure<int>("Não foi possível criar o tipo de documento: " + td.Error);
            }

            var tipoDocumento = td.Value;

            await _tipoDocumentoRepository.IncluirAsync(tipoDocumento);

            await CommitAsync(cancellationToken);

            return Result.Success(tipoDocumento.Id);
        }

        private async Task<Result<int>> Update(UpsertTipoDocumentoRequest request, CancellationToken cancellationToken)
        {
            var tipoDocumento = await _tipoDocumentoRepository.ObterPorIdAsync(request.Id ?? 0);

            if (tipoDocumento == null)
            {
                return Result.Failure<int>("Tipo de documento informado não encontrado. Id: " + request.Id);
            }

            var tdUpdateResult = tipoDocumento.Atualizar(
                request.Tipo,
                request.Nome,
                request.Sigla,
                request.Processado?.ToLowerInvariant() == "s",
                request.Modulo
            );

            if (tdUpdateResult.IsFailure)
            {
                return Result.Failure<int>("Não foi possível criar o tipo de documento: " + tdUpdateResult.Error);
            }

            await _tipoDocumentoRepository.AtualizarAsync(tipoDocumento);

            await CommitAsync(cancellationToken);

            return Result.Success(tipoDocumento.Id);
        }
    }
}
