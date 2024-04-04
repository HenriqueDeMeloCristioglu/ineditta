using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CSharpFunctionalExtensions;

using Ineditta.Application.Cnaes.Entities;
using Ineditta.BuildingBlocks.Core.Domain.ValueObjects;

namespace Ineditta.Application.Confederacoes.Entities
{
    public class Confederacao : Entity
    {
        private Confederacao(string sigla, CNPJ cnpj, string areaGeoeconomica, string telefone, string grupo, string grau)
        {
            Sigla = sigla;
            CNPJ = cnpj;
            AreaGeoeconomica = areaGeoeconomica;
            Telefone = telefone;
            Grupo = grupo;
            Grau = grau;
        }
        public string Sigla { get; private set; }
        public CNPJ CNPJ { get; private set; }
        public string AreaGeoeconomica { get; private set; }
        public string Telefone { get; private set; }
        public string Grupo { get; private set; }
        public string Grau { get; private set; }

        public static Result<Confederacao> Criar(string sigla, CNPJ cnpj, string areaGeoeconomica, string telefone, string grupo, string grau)
        {
            if (string.IsNullOrEmpty(sigla))
            {
                return Result.Failure<Confederacao>("Informe a sigla");
            }

            if (sigla.Length > 100)
            {
                return Result.Failure<Confederacao>("Sigla deve ter no máximo 100 caracteres");
            }

            if (cnpj is null)
            {
                return Result.Failure<Confederacao>("Informe o CNPJ");
            }

            if (string.IsNullOrEmpty(areaGeoeconomica))
            {
                return Result.Failure<Confederacao>("Informe a Área GeoEconômica");
            }

            if (areaGeoeconomica.Length > 250)
            {
                return Result.Failure<Confederacao>("Área GeoEconômica deve ter no máximo 250 caracteres");
            }

            if (string.IsNullOrEmpty(telefone))
            {
                return Result.Failure<Confederacao>("Informe um Telefone");
            }

            if (telefone.Length > 15)
            {
                return Result.Failure<Confederacao>("Telefone deve ter no máximo 15 caracteres");
            }

            if (string.IsNullOrEmpty(grupo))
            {
                return Result.Failure<Confederacao>("Informe o Grupo");
            }

            if (grupo.Length > 200)
            {
                return Result.Failure<Confederacao>("Grupo deve ter no máximo 200 caracteres");
            }

            if (string.IsNullOrEmpty(grau))
            {
                return Result.Failure<Confederacao>("Informe o Grau");
            }

            if (grau.Length > 200)
            {
                return Result.Failure<Confederacao>("Grau deve ter no máximo 200 caracteres");
            }


            var confederacao = new Confederacao(sigla, cnpj, areaGeoeconomica, telefone, grupo, grau);

            return Result.Success(confederacao);
        }
    }
}
