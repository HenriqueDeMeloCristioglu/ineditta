using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CSharpFunctionalExtensions;

namespace Ineditta.Application.Modulos.Entities
{
    public class Modulo : Entity<int>
    {
        public Modulo(string nome, TipoModulo tipo, bool permiteCriar, bool permiteConsultar, bool permiteComentar, bool permiteAlterar, bool permiteExcluir, bool permiteAprovar, string? uri)
        {
            Nome = nome;
            Tipo = tipo;
            PermiteCriar = permiteCriar;
            PermiteConsultar = permiteConsultar;
            PermiteComentar = permiteComentar;
            PermiteAlterar = permiteAlterar;
            PermiteExcluir = permiteExcluir;
            PermiteAprovar = permiteAprovar;
            Uri = uri;
        }

        protected Modulo() { }

        public string Nome { get; set; } = null!;
        public TipoModulo Tipo { get; set; }
        public bool PermiteCriar { get; set; }
        public bool PermiteConsultar { get; set; }
        public bool PermiteComentar { get; set; }
        public bool PermiteAlterar { get; set; }
        public bool PermiteExcluir { get; set; }
        public bool PermiteAprovar { get; set; }
        public string? Uri { get; set; }

        public static Result<Modulo> Criar(string nome, TipoModulo tipo, bool permiteCriar, bool permiteConsultar, bool permiteComentar, bool permiteAlterar, bool permiteExcluir, bool permiteAprovar, string? uri)
        {
            if (string.IsNullOrEmpty(nome)) return Result.Failure<Modulo>("Você deve fornecer o nome do módulo.");

            var modulo = new Modulo(
                nome,
                tipo,
                permiteCriar,
                permiteConsultar,
                permiteComentar,
                permiteAlterar,
                permiteExcluir,
                permiteAprovar,
                uri
            );

            return Result.Success(modulo);
        }
    }
}
