namespace Ineditta.BuildingBlocks.Core.Idempotency.Database
{
    public class IdempotentModel
    {
        public IdempotentModel(Guid id, string nome)
        {
            Id = id;
            Nome = nome;
            DataProcessamento = DateTime.Now;
        }

        public Guid Id { get; private set; }
        public string Nome { get; private set; }
        public DateTime DataProcessamento { get; private set; }
    }
}