using CSharpFunctionalExtensions;

namespace Ineditta.Application.ClientesUnidades.Entities
{
    public class CnaeUnidade : ValueObject<CnaeUnidade>
    {
        protected CnaeUnidade()
        {
        }

        private CnaeUnidade(int id)
        {
            Id = id;
        }

        public int Id { get; private set; }

        public static Result<CnaeUnidade> Criar(int id)
        {
            var cnaeUnidade = new CnaeUnidade(id);

            return Result.Success(cnaeUnidade);
        }

        protected override bool EqualsCore(CnaeUnidade other)
        {
            throw new NotImplementedException();
        }

        protected override int GetHashCodeCore()
        {
            throw new NotImplementedException();
        }
    }
}
