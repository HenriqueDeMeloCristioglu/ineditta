using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CSharpFunctionalExtensions;

using Ineditta.Application.Localizacoes.Enumerations;

namespace Ineditta.Application.Localizacoes.Entities
{
    public class Localizacao : Entity<int>
    {
        private Localizacao(PaisEnumeration pais, RegiaoEnumeration regiao, UfEnumeration uf, EstadoEnumeration estado, string municipio)
        {
            Pais = pais;
            Regiao = regiao;
            Uf = uf;
            Estado = estado;
            Municipio = municipio;
        }

        protected Localizacao() { }

        public PaisEnumeration Pais { get; private set; } = null!;
        public RegiaoEnumeration Regiao { get; private set; } = null!;
        public UfEnumeration Uf { get; private set; } = null!;
        public EstadoEnumeration Estado { get; private set; } = null!;
        public string Municipio { get; private set; } = null!;

        public static Result<Localizacao> Criar(PaisEnumeration pais, RegiaoEnumeration regiao, UfEnumeration uf, EstadoEnumeration estado, string municipio)
        {
            if (pais is null) return Result.Failure<Localizacao>("Você deve fornencer o país da localização.");
            if (regiao is null) return Result.Failure<Localizacao>("Você deve fornencer a região da localização.");
            if (uf is null) return Result.Failure<Localizacao>("Você deve fornencer a sigla uf da localização.");
            if (estado is null) return Result.Failure<Localizacao>("Você deve fornencer o estado da localização.");
            if (string.IsNullOrEmpty(municipio)) return Result.Failure<Localizacao>("Você deve fornencer o município da localização.");

            var localizacao = new Localizacao(
                pais,
                regiao,
                uf,
                estado,
                municipio
            );

            return Result.Success(localizacao);
        }
    }
}
