using CSharpFunctionalExtensions;

using Ineditta.Application.Sindicatos.Base.ValueObjects;
using Ineditta.Application.Sindicatos.Laborais.Erros;
using Ineditta.Application.Sindicatos.Patronais.Erros;
using Ineditta.BuildingBlocks.Core.Domain.Contracts;
using Ineditta.BuildingBlocks.Core.Domain.ValueObjects;

namespace Ineditta.Application.Sindicatos.Laborais.Entities
{
    public class SindicatoLaboral : Entity<int>, IAuditable
    {
        protected SindicatoLaboral()
#pragma warning restore CS0628 // New protected member declared in sealed type
        {

        }
        private SindicatoLaboral(
            string sigla,
            CNPJ cnpj,
            string razaoSocial,
            string denominacao,
            CodigoSindical codigoSindical,
            string? situacao,
            string logradouro,
            string municipio,
            string uf,
            Telefone telefone1,
            Telefone? telefone2,
            Telefone? telefone3,
            Ramal? ramal,
            string? enquadramento,
            string? contribuicao,
            string? negociador,
            Email? email1,
            Email? email2,
            Email? email3,
            string? twitter,
            string? facebook,
            string? instagram,
            string? site,
            Grau grau,
            bool status,
            int federacaoId,
            int confederacaoId,
            int? centralSindicalId)
        {
            Sigla = sigla;
            Cnpj = cnpj;
            RazaoSocial = razaoSocial;
            Denominacao = denominacao;
            CodigoSindical = codigoSindical;
            Situacao = situacao;
            Logradouro = logradouro;
            Municipio = municipio;
            Uf = uf;
            Telefone1 = telefone1;
            Telefone2 = telefone2;
            Telefone3 = telefone3;
            Ramal = ramal;
            Enquadramento = enquadramento;
            Contribuicao = contribuicao;
            Negociador = negociador;
            Email1 = email1;
            Email2 = email2;
            Email3 = email3;
            Twitter = twitter;
            Facebook = facebook;
            Instagram = instagram;
            Site = site;
            Grau = grau;
            Status = status;
            FederacaoId = federacaoId;
            ConfederacaoId = confederacaoId;
            CentralSindicalId = centralSindicalId;
        }

        public string Sigla { get; private set; } = null!;
        public CNPJ Cnpj { get; private set; } = null!;
        public string RazaoSocial { get; private set; } = null!;
        public string Denominacao { get; private set; } = null!;
        public CodigoSindical CodigoSindical { get; private set; } = null!;
        public string? Situacao { get; private set; } = null!;
        public string Logradouro { get; private set; } = null!;
        public string Municipio { get; private set; } = null!;
        public string Uf { get; private set; } = null!;
        public Telefone Telefone1 { get; private set; } = null!;
        public Telefone? Telefone2 { get; private set; }
        public Telefone? Telefone3 { get; private set; }
        public Ramal? Ramal { get; private set; }
        public string? Enquadramento { get; set; }
        public string? Contribuicao { get; set; }
        public string? Negociador { get; set; }
        public Email? Email1 { get; private set; } = null!;
        public Email? Email2 { get; private set; }
        public Email? Email3 { get; private set; }
        public string? Twitter { get; private set; }
        public string? Facebook { get; private set; }
        public string? Instagram { get; private set; }
        public string? Site { get; private set; }
        public Grau Grau { get; private set; }
        public bool Status { get; private set; }
        public int? FederacaoId { get; private set; }
        public int? ConfederacaoId { get; private set; }
        public int? CentralSindicalId { get; set; }

        public static Result<SindicatoLaboral> Criar(string sigla, CNPJ cnpj, string razaoSocial, string denominacao, CodigoSindical codigoSindical, string? situacao, string logradouro, string municipio, string uf, Telefone telefone1, Telefone? telefone2, Telefone? telefone3, Ramal? ramal, string? enquadramento, string? contribuicao, string? negociador, Email? email1, Email? email2, Email? email3, string? twitter, string? facebook, string? instagram, string? site, Grau grau, bool status, int federacaoId, int confederacaoId, int centralSindicalId)
        {
            if (sigla is null)
            {
                return Result.Failure<SindicatoLaboral>(SindicatoLaboralError.CampoVazio("Sigla"));
            }

            if (cnpj.Value is null)
            {
                return Result.Failure<SindicatoLaboral>(SindicatoLaboralError.CampoVazio("Cnpj"));
            }

            if (razaoSocial is null)
            {
                return Result.Failure<SindicatoLaboral>(SindicatoLaboralError.CampoVazio("Razao Social"));
            }

            if (denominacao is null)
            {
                return Result.Failure<SindicatoLaboral>(SindicatoLaboralError.CampoVazio("Denominação"));
            }

            if (codigoSindical is null)
            {
                return Result.Failure<SindicatoLaboral>(SindicatoLaboralError.CampoVazio("Código Sindical"));
            }

            if (logradouro is null)
            {
                return Result.Failure<SindicatoLaboral>(SindicatoLaboralError.CampoVazio("Logradouro"));
            }

            if (municipio is null)
            {
                return Result.Failure<SindicatoLaboral>(SindicatoLaboralError.CampoVazio("Municipio"));
            }

            if (uf is null)
            {
                return Result.Failure<SindicatoLaboral>(SindicatoLaboralError.CampoVazio("Uf"));
            }

            if (telefone1 is null)
            {
                return Result.Failure<SindicatoLaboral>(SindicatoLaboralError.CampoVazio("Telefone 1"));
            }

            if (grau < 0)
            {
                return Result.Failure<SindicatoLaboral>(SindicatoLaboralError.CampoVazio("Grau"));
            }

            if (confederacaoId <= 0)
            {
                return Result.Failure<SindicatoLaboral>(SindicatoLaboralError.CampoInvalido("Confederação"));
            }

            if (federacaoId <= 0)
            {
                return Result.Failure<SindicatoLaboral>(SindicatoLaboralError.CampoInvalido("Federaçao"));
            }

            if (centralSindicalId <= 0)
            {
                return Result.Failure<SindicatoLaboral>(SindicatoLaboralError.CampoInvalido("Central Sindical"));
            }

            var sindicatoLaboral = new SindicatoLaboral(sigla, cnpj, razaoSocial, denominacao, codigoSindical, situacao, logradouro, municipio, uf, telefone1, telefone2, telefone3, ramal, enquadramento, contribuicao, negociador, email1, email2, email3, twitter, facebook, instagram, site, grau, status, federacaoId, confederacaoId, centralSindicalId);

            return Result.Success(sindicatoLaboral);
        }

        public Result Atualizar(string sigla, CNPJ cnpj, string razaoSocial, string denominacao, CodigoSindical codigoSindical, string? situacao, string logradouro, string municipio, string uf, Telefone telefone1, Telefone? telefone2, Telefone? telefone3, Ramal? ramal, string? enquadramento, string? contribuicao, string? negociador, Email? email1, Email? email2, Email? email3, string? twitter, string? facebook, string? instagram, string? site, Grau grau, bool status, int federacaoId, int confederacaoId, int centralSindicalId)
        {
            if (sigla is null)
            {
                return Result.Failure(SindicatoLaboralError.CampoVazio("Sigla"));
            }

            if (cnpj.Value is null)
            {
                return Result.Failure(SindicatoLaboralError.CampoVazio("Cnpj"));
            }

            if (razaoSocial is null)
            {
                return Result.Failure(SindicatoLaboralError.CampoVazio("Razao Social"));
            }

            if (denominacao is null)
            {
                return Result.Failure(SindicatoLaboralError.CampoVazio("Denominação"));
            }

            if (codigoSindical is null)
            {
                return Result.Failure(SindicatoLaboralError.CampoVazio("Código Sindical"));
            }

            if (logradouro is null)
            {
                return Result.Failure(SindicatoLaboralError.CampoVazio("Logradouro"));
            }

            if (municipio is null)
            {
                return Result.Failure(SindicatoLaboralError.CampoVazio("Municipio"));
            }

            if (uf is null)
            {
                return Result.Failure(SindicatoLaboralError.CampoVazio("Uf"));
            }

            if (telefone1 is null)
            {
                return Result.Failure(SindicatoLaboralError.CampoVazio("Telefone 1"));
            }

            if (grau < 0)
            {
                return Result.Failure(SindicatoLaboralError.CampoVazio("Grau"));
            }

            if (confederacaoId <= 0)
            {
                return Result.Failure<SindicatoLaboral>(SindicatoLaboralError.CampoInvalido("Confederação"));
            }

            if (federacaoId <= 0)
            {
                return Result.Failure<SindicatoLaboral>(SindicatoLaboralError.CampoInvalido("Federaçao"));
            }

            if (centralSindicalId <= 0)
            {
                return Result.Failure<SindicatoLaboral>(SindicatoLaboralError.CampoInvalido("Central Sindical"));
            }

            Sigla = sigla;
            Cnpj = cnpj;
            RazaoSocial = razaoSocial;
            Denominacao = denominacao;
            CodigoSindical = codigoSindical;
            Situacao = situacao;
            Logradouro = logradouro;
            Municipio = municipio;
            Uf = uf;
            Telefone1 = telefone1;
            Telefone2 = telefone2;
            Telefone3 = telefone3;
            Ramal = ramal;
            Enquadramento = enquadramento;
            Contribuicao = contribuicao;
            Negociador = negociador;
            Email1 = email1;
            Email2 = email2;
            Email3 = email3;
            Twitter = twitter;
            Facebook = facebook;
            Instagram = instagram;
            Site = site;
            Grau = grau;
            Status = status;
            FederacaoId = federacaoId;
            ConfederacaoId = confederacaoId;
            CentralSindicalId = centralSindicalId;

            return Result.Success();
        }
    }
}
