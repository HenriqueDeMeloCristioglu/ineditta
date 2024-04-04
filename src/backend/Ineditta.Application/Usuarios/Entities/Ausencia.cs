using CSharpFunctionalExtensions;

namespace Ineditta.Application.Usuarios.Entities
{
    public class Ausencia: ValueObject<Ausencia>
    {
        private Ausencia(DateOnly dataInicial, DateOnly dataFinal)
        {
            DataInicial = dataInicial;
            DataFinal = dataFinal;
        }

        public DateOnly DataInicial { get; private set; }
        public DateOnly DataFinal { get; private set; }

        public static Result<Ausencia> Criar(DateOnly dataInicial, DateOnly dataFinal)
        {
            if (dataFinal < dataInicial)
            {
                return Result.Failure<Ausencia>("Data de ausência inicial deve ser menor que a data final");
            }

            var ausencia = new Ausencia(dataInicial, dataFinal);

            return Result.Success(ausencia);
        }

        protected override bool EqualsCore(Ausencia other)
        {
            return other.DataInicial == DataInicial && other.DataFinal == DataFinal;
        }

        protected override int GetHashCodeCore()
        {
            return HashCode.Combine(DataInicial, DataFinal);
        }

        public static Ausencia SemAusencia()
        {
            return new Ausencia(DateOnly.MinValue, DateOnly.MinValue);
        }
    }
}
