using CSharpFunctionalExtensions;

using Ineditta.Application.Sindicatos.Base.ValueObjects;
using Ineditta.Application.Sindicatos.Laborais.Entities;
using Ineditta.BuildingBlocks.Core.Domain.ValueObjects;

using MediatR;

namespace Ineditta.Application.Sindicatos.Patronais.UseCases.Upsert
{
    public class UpsertSindicatoPatronalRequest : IRequest<Result>
    {
        public int? Id { get; set; }
        public string Sigla { get; set; } = null!;
        public string Cnpj { get; set; } = null!;
        public string RazaoSocial { get; set; } = null!;
        public string Denominacao { get; set; } = null!;
        public string CodigoSindical { get; set; } = null!;
        public string? Situacao { get; set; }
        public string Logradouro { get; set; } = null!;
        public string Municipio { get; set; } = null!;
        public string Uf { get; set; } = null!;
        public string Telefone1 { get; set; } = null!;
        public string? Telefone2 { get; set; }
        public string? Telefone3 { get; set; }
        public string? Ramal { get; set; }
        public string? Enquadramento { get; set; }
        public string? Contribuicao { get; set; }
        public string? Negociador { get; set; }
        public string? Email1 { get; set; }
        public string? Email2 { get; set; }
        public string? Email3 { get; set; }
        public string? Twitter { get; set; }
        public string? Facebook { get; set; }
        public string? Instagram { get; set; }
        public string? Site { get; set; }
        public Grau Grau { get; set; }
        public bool Status { get; set; }
        public int FederacaoId { get; set; }
        public int? CentralSindicalId { get; set; }
        public int ConfederacaoId { get; set; }
        public IEnumerable<BaseTerritorialRequest> BasesTerritoriais { get; set; } = null!;
    }

    public class BaseTerritorialRequest
    {
        public int? BaseTerritorialId { get; set; }
        public int LocalizacaoId { get; set; }
        public int CnaeId { get; set; }
    }

}
