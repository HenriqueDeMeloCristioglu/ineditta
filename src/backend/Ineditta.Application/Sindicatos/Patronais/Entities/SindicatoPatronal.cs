using CSharpFunctionalExtensions;

using Ineditta.Application.Sindicatos.Base.ValueObjects;
using Ineditta.Application.Sindicatos.Laborais.Entities;
using Ineditta.Application.Sindicatos.Laborais.Erros;
using Ineditta.Application.Sindicatos.Patronais.Erros;
using Ineditta.BuildingBlocks.Core.Domain.Contracts;
using Ineditta.BuildingBlocks.Core.Domain.ValueObjects;

namespace Ineditta.Application.Sindicatos.Patronais.Entities
{
    public class SindicatoPatronal : Entity<int>, IAuditable
    {
#pragma warning disable CS0628 // New protected member declared in sealed type
        protected SindicatoPatronal()
#pragma warning restore CS0628 // New protected member declared in sealed type
        {

        }
        private SindicatoPatronal(
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
            int confederacaoId)
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

        public static Result<SindicatoPatronal> Criar(string sigla, CNPJ cnpj, string razaoSocial, string denominacao, CodigoSindical codigoSindical, string? situacao, string logradouro, string municipio, string uf, Telefone telefone1, Telefone? telefone2, Telefone? telefone3, Ramal? ramal, string? enquadramento, string? contribuicao, string? negociador, Email? email1, Email? email2, Email? email3, string? twitter, string? facebook, string? instagram, string? site, Grau grau, bool status, int federacaoId, int confederacaoId)
        {
            if (sigla is null)
            {
                return Result.Failure<SindicatoPatronal>(SindicatoPatronalError.CampoVazio("Sigla"));
            }

            if (cnpj.Value is null)
            {
                return Result.Failure<SindicatoPatronal>(SindicatoPatronalError.CampoVazio("Cnpj"));
            }

            if (razaoSocial is null)
            {
                return Result.Failure<SindicatoPatronal>(SindicatoPatronalError.CampoVazio("Razao Social"));
            }

            if (denominacao is null)
            {
                return Result.Failure<SindicatoPatronal>(SindicatoPatronalError.CampoVazio("Denominação"));
            }

            if (codigoSindical is null)
            {
                return Result.Failure<SindicatoPatronal>(SindicatoPatronalError.CampoVazio("Código Sindical"));
            }

            if (logradouro is null)
            {
                return Result.Failure<SindicatoPatronal>(SindicatoPatronalError.CampoVazio("Logradouro"));
            }

            if (municipio is null)
            {
                return Result.Failure<SindicatoPatronal>(SindicatoPatronalError.CampoVazio("Municipio"));
            }

            if (uf is null)
            {
                return Result.Failure<SindicatoPatronal>(SindicatoPatronalError.CampoVazio("Uf"));
            }

            if (telefone1 is null)
            {
                return Result.Failure<SindicatoPatronal>(SindicatoPatronalError.CampoVazio("Telefone 1"));
            }

            if (grau < 0)
            {
                return Result.Failure<SindicatoPatronal>(SindicatoPatronalError.CampoVazio("Grau"));
            }

            if (confederacaoId <= 0)
            {
                return Result.Failure<SindicatoPatronal>(SindicatoPatronalError.CampoInvalido("Confederação"));
            }

            if (federacaoId <= 0)
            {
                return Result.Failure<SindicatoPatronal>(SindicatoPatronalError.CampoInvalido("Federaçao"));
            }

            var sindicatoPatronal = new SindicatoPatronal(sigla, cnpj, razaoSocial, denominacao, codigoSindical, situacao, logradouro, municipio, uf, telefone1, telefone2, telefone3, ramal, enquadramento, contribuicao, negociador, email1, email2, email3, twitter, facebook, instagram, site, grau, status, federacaoId, confederacaoId);

            return Result.Success(sindicatoPatronal);
        }

        public Result Atualizar(string sigla, CNPJ cnpj, string razaoSocial, string denominacao, CodigoSindical codigoSindical, string? situacao, string logradouro, string municipio, string uf, Telefone telefone1, Telefone? telefone2, Telefone? telefone3, Ramal? ramal, string? enquadramento, string? contribuicao, string? negociador, Email? email1, Email? email2, Email? email3, string? twitter, string? facebook, string? instagram, string? site, Grau grau, bool status, int federacaoId, int confederacaoId)
        {
            if (sigla is null)
            {
                return Result.Failure(SindicatoPatronalError.CampoVazio("Sigla"));
            }

            if (cnpj.Value is null)
            {
                return Result.Failure(SindicatoPatronalError.CampoVazio("Cnpj"));
            }

            if (razaoSocial is null)
            {
                return Result.Failure(SindicatoPatronalError.CampoVazio("Razao Social"));
            }

            if (denominacao is null)
            {
                return Result.Failure(SindicatoPatronalError.CampoVazio("Denominação"));
            }

            if (codigoSindical is null)
            {
                return Result.Failure(SindicatoPatronalError.CampoVazio("Código Sindical"));
            }

            if (logradouro is null)
            {
                return Result.Failure(SindicatoPatronalError.CampoVazio("Logradouro"));
            }

            if (municipio is null)
            {
                return Result.Failure(SindicatoPatronalError.CampoVazio("Municipio"));
            }

            if (uf is null)
            {
                return Result.Failure(SindicatoPatronalError.CampoVazio("Uf"));
            }

            if (telefone1 is null)
            {
                return Result.Failure(SindicatoPatronalError.CampoVazio("Telefone 1"));
            }

            if (grau < 0)
            {
                return Result.Failure(SindicatoPatronalError.CampoVazio("Grau"));
            }

            if (confederacaoId <= 0)
            {
                return Result.Failure<SindicatoPatronal>(SindicatoPatronalError.CampoInvalido("Confederação"));
            }

            if (federacaoId <= 0)
            {
                return Result.Failure<SindicatoPatronal>(SindicatoPatronalError.CampoInvalido("Federaçao"));
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

            return Result.Success();
        }
    }
}