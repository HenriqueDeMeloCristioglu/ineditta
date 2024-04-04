using CSharpFunctionalExtensions;

using Ineditta.Application.BasesTerritoriaisLaborais.Entities;
using Ineditta.Application.BasesTerritoriaisLaborais.Repositories;
using Ineditta.Application.Sindicatos.Base.ValueObjects;
using Ineditta.Application.Sindicatos.Laborais.Repositories;
using Ineditta.BuildingBlocks.Core.Database;
using Ineditta.BuildingBlocks.Core.Domain.Handlers;
using Ineditta.BuildingBlocks.Core.Domain.ValueObjects;

using MediatR;

using SindicatoLaboral = Ineditta.Application.Sindicatos.Laborais.Entities.SindicatoLaboral;

namespace Ineditta.Application.Sindicatos.Laborais.UseCases.Upsert
{
    public class UpsertSindicatoLaboralHandler : BaseCommandHandler, IRequestHandler<UpsertSindicatoLaboralRequest, Result>
    {
        private readonly ISindicatoLaboralRepository _sindicatoLaboralRepository;
        private readonly IBaseTerritorialLaboralRepository _baseTerritorialRepository;
        public UpsertSindicatoLaboralHandler(IUnitOfWork unitOfWork, ISindicatoLaboralRepository sindicatoLaboralRepository, IBaseTerritorialLaboralRepository baseTerritorialRepository) : base(unitOfWork)
        {
            _sindicatoLaboralRepository = sindicatoLaboralRepository;
            _baseTerritorialRepository = baseTerritorialRepository;
        }

        public async Task<Result> Handle(UpsertSindicatoLaboralRequest request, CancellationToken cancellationToken)
        {
            return request.Id is not null ?
                await AtualizarAsync(request, cancellationToken) :
                await IncluirAsync(request, cancellationToken);
        }

        private async Task<Result> IncluirAsync(UpsertSindicatoLaboralRequest request, CancellationToken cancellationToken)
        {
            var cnpjRequest = CNPJ.Criar(request.Cnpj);
            if (!cnpjRequest.IsSuccess) return Result.Failure("CNPJ inválido.");

            if (await _sindicatoLaboralRepository.ExisteAsync(cnpjRequest.Value))
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

            var sindicatoLaboral = SindicatoLaboral.Criar(
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
                request.ConfederacaoId,
                request.CentralSindicalId
            );

            if (sindicatoLaboral.IsFailure)
            {
                return sindicatoLaboral;
            }

            var resultTransaction = await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                await _sindicatoLaboralRepository.IncluirAsync(sindicatoLaboral.Value);
                _ = await CommitAsync(cancellationToken);

                foreach (var baseTerritorialRequest in request.BasesTerritoriais)
                {
                    var baseTerritorial = BaseTerritorialLaboral.Criar(
                        sindicatoLaboral.Value,
                        baseTerritorialRequest.LocalizacaoId,
                        baseTerritorialRequest.CnaeId,
                        baseTerritorialRequest.DataNegociacao
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

        private async Task<Result> AtualizarAsync(UpsertSindicatoLaboralRequest request, CancellationToken cancellationToken)
        {
            if (request.BasesTerritoriais is null) return Result.Failure("Você deve informar a base territorial");

            var cnpjRequest = CNPJ.Criar(request.Cnpj);
            if (!cnpjRequest.IsSuccess) return Result.Failure("CNPJ inválido.");

            if (await _sindicatoLaboralRepository.ExisteAsync(cnpjRequest.Value, request.Id ?? 0))
            {
                return Result.Failure("Existe outro sindicato cadastrado com esse CNPJ");
            }

            var sindicatoLaboral = await _sindicatoLaboralRepository.ObterPorIdAsync(request.Id ?? 0);

            if (sindicatoLaboral is null)
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

            var resultAtualizacao = sindicatoLaboral.Atualizar(
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
                request.ConfederacaoId,
                request.CentralSindicalId
            );

            if (resultAtualizacao.IsFailure)
            {
                return resultAtualizacao;
            }

            var resultTransaction = await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                await _sindicatoLaboralRepository.AtualizarAsync(sindicatoLaboral);
                _ = await CommitAsync(cancellationToken);

                var basesTerritoriaisExistentes = await _baseTerritorialRepository.ObterVigentesPorSindicatoLaboralIdAsync(sindicatoLaboral.Id);
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
                        var baseTerritorial = BaseTerritorialLaboral.Criar(
                            sindicatoLaboral,
                            baseTerritorialRequest.LocalizacaoId,
                            baseTerritorialRequest.CnaeId,
                            baseTerritorialRequest.DataNegociacao
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
            }, cancellationToken);

            return resultTransaction;
        }
    }
}
