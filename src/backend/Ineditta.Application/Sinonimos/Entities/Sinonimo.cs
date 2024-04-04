using CSharpFunctionalExtensions;

namespace Ineditta.Application.Sinonimos.Entities
{
    public class Sinonimo : Entity<int>
    {
        protected Sinonimo() { }

        private Sinonimo(string nome, int estruturaClausulaId, int? assuntoId)
        {
            Nome = nome;
            EstruturaClausulaId = estruturaClausulaId;
            AssuntoId = assuntoId;
        }

        public string Nome { get; private set; } = null!;
        public int EstruturaClausulaId { get; private set; }
        public int? AssuntoId { get; private set; }

        public static Result<Sinonimo> Criar(string nome, int estruturaClausulaId, int? assuntoId)
        {
            if (string.IsNullOrEmpty(nome))
            {
                return Result.Failure<Sinonimo>("Nome deve ser informado");
            }

            if (estruturaClausulaId <= 0)
            {
                return Result.Failure<Sinonimo>("Estrutura cláusula ID deve ser informado");
            }

            var sinonimo = new Sinonimo(nome, estruturaClausulaId, assuntoId);

            return Result.Success(sinonimo);
        }

        public Result Atualizar(string nome, int estruturaClausulaId, int? assuntoId)
        {
            if (string.IsNullOrEmpty(nome))
            {
                return Result.Failure<Sinonimo>("Nome deve ser informado");
            }

            if (estruturaClausulaId <= 0)
            {
                return Result.Failure<Sinonimo>("Estrutura cláusula ID deve ser informado");
            }

            Nome = nome;
            EstruturaClausulaId = estruturaClausulaId;
            AssuntoId = assuntoId;

            return Result.Success();
        }
    }
}