using System.Linq;

using CSharpFunctionalExtensions;

using FluentValidation.Results;

using Ineditta.Application.BasesTerritoriaisPatronais.Entities;
using Ineditta.Application.BasesTerritoriaisPatronais.Repositories;
using Ineditta.Application.Sindicatos.Base.ValueObjects;
using Ineditta.Application.Sindicatos.Patronais.Entities;
using Ineditta.Application.Sindicatos.Patronais.Repositories;
using Ineditta.BuildingBlocks.Core.Database;
using Ineditta.BuildingBlocks.Core.Domain.Handlers;
using Ineditta.BuildingBlocks.Core.Domain.ValueObjects;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Ineditta.Application.Sindicatos.Patronais.UseCases.Upsert
{
    public class UpsertSindicatoPatronalRequestHandler : BaseCommandHandler, IRequestHandler<UpsertSindicatoPatronalRequest, Result>
    {
        private readonly ISindicatoPatronalRepository _sindicatoPatronalRepository;
        private readonly IBaseTerritorialRepository _baseTerritorialRepository;
        public UpsertSindicatoPatronalRequestHandler(IUnitOfWork unitOfWork, ISindicatoPatronalRepository sindicatoPatronalRepository, IBaseTerritorialRepository baseTerritorialRepository) : base(unitOfWork)
        {
            _sindicatoPatronalRepository = sindicatoPatronalRepository;
            _baseTerritorialRepository = baseTerritorialRepository;
        }
        public async Task<Result> Handle(UpsertSindicatoPatronalRequest request, CancellationToken cancellationToken)
        {
            return request.Id is not null ? 
                await AtualizarAsync(request, cancellationToken) : 
                await IncluirAsync(request, cancellationToken);
        }

        private async Task<Result> IncluirAsync(UpsertSindicatoPatronalRequest request, CancellationToken cancellationToken)
        {
            var cnpjRequest = CNPJ.Criar(request.Cnpj);
            if (!cnpjRequest.IsSuccess) return Result.Failure("CNPJ inválido.");

            if (await _sindicatoPatronalRepository.ExisteAsync(cnpjRequest.Value))
            {
                return Result.Failure("Existe outro sindicato cadastrado com esse CNPJ");
            }

            var codigoSindicalRequest = CodigoSindical.Criar(request.CodigoSindical);
            if (codigoSindicalRequest.IsFailure) return codigoSindicalRequest;

            var telefone1Request = Telefone.Criar(request.Telefone1);
            if (telefone1Request.IsFailure) return telefone1Request;

            Telefone? telefone2Request = null;
            Telefone? telefone3Request = null;
            Ramal? ramalRequest = null;
            Email? email1Request = null;
            Email? email2Request = null;
            Email? email3Request = null;

            if (request.Telefone2 is not null)
            {
                var telefone2 = Telefone.Criar(request.Telefone2);
                if (telefone2.IsFailure) return telefone2;

                telefone2Request = telefone2.Value;
            }

            if (request.Telefone3 is not null)
            {
                var telefone3 = Telefone.Criar(request.Telefone3);
                if (telefone3.IsFailure) return telefone3;

                telefone3Request = telefone3.Value;
            }

            if (!string.IsNullOrEmpty(request.Ramal))
            {
                var ramal = Ramal.Criar(request.Ramal);
                if (ramal.IsFailure) return ramal;

                ramalRequest = ramal.Value;
            }

            if (!string.IsNullOrEmpty(request.Email1))
            {
                var email = Email.Criar(request.Email1);
                if (email.IsFailure) return email;

                email1Request = email.Value;
            }

            if (!string.IsNullOrEmpty(request.Email2))
            {
                var email = Email.Criar(request.Email2);
                if (email.IsFailure) return email;

                email2Request = email.Value;
            }

            if (!string.IsNullOrEmpty(request.Email3))
            {
                var email = Email.Criar(request.Email3);
                if (email.IsFailure) return email;

                email3Request = email.Value;
            }

            var sindicatoPatronal = SindicatoPatronal.Criar(
                request.Sigla,
                cnpjRequest.Value,
                request.RazaoSocial,
                request.Denominacao,
                codigoSindicalRequest.Value,
                request.Situacao,
                request.Logradouro,
                request.Municipio,
                request.Uf,
                telefone1Request.Value,
                telefone2Request,
                telefone3Request,
                ramalRequest,
                request.Enquadramento,
                request.Contribuicao,
                request.Negociador,
                email1Request,
                email2Request,
                email3Request,
                request.Twitter,
                request.Facebook,
                request.Instagram,
                request.Site,
                request.Grau,
                request.Status,
                request.FederacaoId,
                request.ConfederacaoId
            );

            if (sindicatoPatronal.IsFailure)
            {
                return sindicatoPatronal;
            }

            var resultTransaction = await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                await _sindicatoPatronalRepository.IncluirAsync(sindicatoPatronal.Value);
                _ = await CommitAsync(cancellationToken);

                foreach (var baseTerritorialRequest in request.BasesTerritoriais)
                {
                    var baseTerritorial = BaseTerritorialPatronal.Criar(
                        sindicatoPatronal.Value,
                        baseTerritorialRequest.LocalizacaoId,
                        baseTerritorialRequest.CnaeId
                    );

                    if (baseTerritorial.IsFailure)
                    {
                        return baseTerritorial;
                    }

                    await _baseTerritorialRepository.IncluirAsync(baseTerritorial.Value);
                    _ = await CommitAsync(cancellationToken);
                }

                return Result.Success();
            }, cancellationToken);

            return resultTransaction;
        }

        private async Task<Result> AtualizarAsync(UpsertSindicatoPatronalRequest request, CancellationToken cancellationToken)
        {
            if (request.BasesTerritoriais is null) return Result.Failure("Você deve informar a base territorial");

            var cnpjRequest = CNPJ.Criar(request.Cnpj);
            if (!cnpjRequest.IsSuccess) return Result.Failure("CNPJ inválido.");

            if (await _sindicatoPatronalRepository.ExisteAsync(cnpjRequest.Value, request.Id ?? 0))
            {
                return Result.Failure("Existe outro sindicato cadastrado com esse CNPJ");
            }

            var sindicatoPatronal = await _sindicatoPatronalRepository.ObterPorIdAsync(request.Id ?? 0);

            if (sindicatoPatronal is null)
            {
                return Result.Failure("Sindicato patronal não foi encontrado");
            }

            var codigoSindicalRequest = CodigoSindical.Criar(request.CodigoSindical);
            if (codigoSindicalRequest.IsFailure) return codigoSindicalRequest;

            var telefone1Request = Telefone.Criar(request.Telefone1);
            if (telefone1Request.IsFailure) return telefone1Request;

            Telefone? telefone2Request = null;
            Telefone? telefone3Request = null;
            Ramal? ramalRequest = null;
            Email? email1Request = null;
            Email? email2Request = null;
            Email? email3Request = null;

            if (request.Telefone2 is not null)
            {
                var telefone2 = Telefone.Criar(request.Telefone2);
                if (telefone2.IsFailure) return telefone2;

                telefone2Request = telefone2.Value;
            }

            if (request.Telefone3 is not null)
            {
                var telefone3 = Telefone.Criar(request.Telefone3);
                if (telefone3.IsFailure) return telefone3;

                telefone3Request = telefone3.Value;
            }

            if (!string.IsNullOrEmpty(request.Ramal))
            {
                var ramal = Ramal.Criar(request.Ramal);
                if (ramal.IsFailure) return ramal;

                ramalRequest = ramal.Value;
            }

            if (!string.IsNullOrEmpty(request.Email1))
            {
                var email = Email.Criar(request.Email1);
                if (email.IsFailure) return email;

                email1Request = email.Value;
            }

            if (!string.IsNullOrEmpty(request.Email2))
            {
                var email = Email.Criar(request.Email2);
                if (email.IsFailure) return email;

                email2Request = email.Value;
            }

            if (!string.IsNullOrEmpty(request.Email3))
            {
                var email = Email.Criar(request.Email3);
                if (email.IsFailure) return email;

                email3Request = email.Value;
            }

            var resultAtualizacao = sindicatoPatronal.Atualizar(
                request.Sigla,
                cnpjRequest.Value,
                request.RazaoSocial,
                request.Denominacao,
                codigoSindicalRequest.Value,
                request.Situacao,
                request.Logradouro,
                request.Municipio,
                request.Uf,
                telefone1Request.Value,
                telefone2Request,
                telefone3Request,
                ramalRequest,
                request.Enquadramento,
                request.Contribuicao,
                request.Negociador,
                email1Request,
                email2Request,
                email3Request,
                request.Twitter,
                request.Facebook,
                request.Instagram,
                request.Site,
                request.Grau,
                request.Status,
                request.FederacaoId,
                request.ConfederacaoId
            );

            if (resultAtualizacao.IsFailure)
            {
                return resultAtualizacao;
            }

            var resultTransaction = await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                await _sindicatoPatronalRepository.AtualizarAsync(sindicatoPatronal);
                _ = await CommitAsync(cancellationToken);

                var basesTerritoriaisExistentes = await _baseTerritorialRepository.ObterVigentesPorSindicatoPatronalIdAsync(sindicatoPatronal.Id);
                var basesTerritoriaisIdsRequest = request.BasesTerritoriais.Where(bt => bt.BaseTerritorialId is not null).Select(bt => (int)bt.BaseTerritorialId!).ToList();

                if (basesTerritoriaisExistentes is not null && basesTerritoriaisExistentes.Any())
                {
                    var basesTerritoriaisFinalizacao = basesTerritoriaisExistentes.Where(bte => !basesTerritoriaisIdsRequest.Contains(bte.Id));

                    if (basesTerritoriaisFinalizacao is not null && basesTerritoriaisFinalizacao.Any())
                    {
                        foreach (var baseTerritorialFinalizacao in basesTerritoriaisFinalizacao)
                        {
                            var finalizacaoResult = baseTerritorialFinalizacao.Finalizar();

                            if (finalizacaoResult.IsFailure)
                            {
                                return finalizacaoResult;
                            }

                            await _baseTerritorialRepository.AtualizarAsync(baseTerritorialFinalizacao);
                            _ = await CommitAsync(cancellationToken);
                        }
                    }
                }

#pragma warning disable S3267
                foreach (var baseTerritorialRequest in request.BasesTerritoriais)
                {
                    if (baseTerritorialRequest.BaseTerritorialId is null)
                    {
                        var baseTerritorial = BaseTerritorialPatronal.Criar(
                            sindicatoPatronal,
                            baseTerritorialRequest.LocalizacaoId,
                            baseTerritorialRequest.CnaeId
                        );

                        if (baseTerritorial.IsFailure)
                        {
                            return baseTerritorial;
                        }

                        await _baseTerritorialRepository.IncluirAsync(baseTerritorial.Value);
                        _ = await CommitAsync(cancellationToken);
                    }
                }

                return Result.Success();
            }, cancellationToken );

            return resultTransaction;
        }
    }
}
