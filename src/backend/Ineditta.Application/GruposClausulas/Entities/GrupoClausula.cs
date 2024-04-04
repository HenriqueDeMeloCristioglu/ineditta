using CSharpFunctionalExtensions;

namespace Ineditta.Application.GruposClausulas.Entities
{
    public class GrupoClausula : Entity<int>
    {
        public string Nome { get; private set; } = null!;

        public string Cor { get; private set; } = null!;
    }
}
