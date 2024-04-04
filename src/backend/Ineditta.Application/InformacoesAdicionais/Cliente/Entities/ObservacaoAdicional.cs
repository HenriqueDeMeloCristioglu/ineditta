using CSharpFunctionalExtensions;

namespace Ineditta.Application.InformacoesAdicionais.Cliente.Entities
{
    public class ObservacaoAdicional : ValueObject<ObservacaoAdicional>
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        protected ObservacaoAdicional()
        {

        }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        private ObservacaoAdicional(int clausulaId, string valor, TipoObservacaoAdicional tipo)
        {
            ClausulaId = clausulaId;
            Valor = valor;
            Tipo = tipo;
        }

        public int ClausulaId { get; private set; }
        public string Valor { get; private set; }
        public TipoObservacaoAdicional Tipo { get; set; }

        public static Result<ObservacaoAdicional> Criar(int clausulaId, string valor, TipoObservacaoAdicional tipo)
        {
            if (clausulaId <= 0)
            {
                return Result.Failure<ObservacaoAdicional>("Informe o id da clausula");
            }

            if (valor is null)
            {
                return Result.Failure<ObservacaoAdicional>("Informe o valor da observação");
            }

            if (tipo <= 0)
            {
                return Result.Failure<ObservacaoAdicional>("Informe o tipo da observação");
            }

            var observacao = new ObservacaoAdicional(clausulaId, valor, tipo);

            return Result.Success(observacao);
        }

        public Result Atualizar(string valor)
        {
            if (valor is null)
            {
                return Result.Failure<ObservacaoAdicional>("Informe o valor da observação");
            }

            Valor = valor;

            return Result.Success();
        }
        protected override bool EqualsCore(ObservacaoAdicional other)
        {
            return ClausulaId == other.ClausulaId && Valor == other.Valor && Tipo == other.Tipo;
        }

        protected override int GetHashCodeCore()
        {
            return GetType().GetHashCode();
        }
    }
}
