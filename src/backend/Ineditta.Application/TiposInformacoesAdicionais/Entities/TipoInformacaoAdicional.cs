using CSharpFunctionalExtensions;

namespace Ineditta.Application.TiposInformacoesAdicionais.Entities
{
    public class TipoInformacaoAdicional : Entity<int>
    {
        public static readonly TipoInformacaoAdicional NomeClausula = new(170, "Nome da cláusula");
        public static readonly TipoInformacaoAdicional Comparativo = new(310, "Comparativo");

        private TipoInformacaoAdicional(int id, string nome)
        {
            Id = id;
            Nome = nome;
        }

        public string Nome { get; private set; }
    }
}
