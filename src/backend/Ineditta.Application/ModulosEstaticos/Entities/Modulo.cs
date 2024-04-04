
using CSharpFunctionalExtensions;

using Ineditta.Application.ModulosAcompanhamentoCct.Entities;

namespace Ineditta.Application.ModulosEstaticos.Entities
{
    public class Modulo : Entity<int>
    {
        public static class Sisap
        {
            public readonly static Modulo AcompanhamentoCCT = new(38, "Acompahmento CCT", Tipo.Sisap, true, true);
        }
        public static class Comercial
        {
            public readonly static Modulo AcompanhamentoCCT = new(3, "Acompanhamento CCT Cliente", Tipo.Comercial, true, true);
            public readonly static Modulo Clausulas = new(6, "Cláusulas", Tipo.Comercial, true, true);
            public readonly static Modulo BuscaRapida = new(64, "Mapa sindical – Busca rápida", Tipo.Sisap, true, true);
        }

        protected Modulo()
        {

        }

        private Modulo(int id, string nome, Tipo tipo, bool criar, bool consultar)
        {
            Id = id;
            Nome = nome;
            Tipo = tipo;
            Criar = criar;
            Consultar = consultar;
        }

        public string Nome { get; private set; } = null!;
        public Tipo Tipo { get; private set; }
        public bool Criar { get; private set; }
        public bool Consultar { get; private set; }
    }
}
