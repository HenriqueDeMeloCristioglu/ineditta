using CSharpFunctionalExtensions;

namespace Ineditta.Application.InformacoesAdicionais.Cliente.Entities
{
    public class InformacaoAdicional : ValueObject<InformacaoAdicional>
    {
        public int ClausulaGeralEstruturaId { get; private set; }
        public string Valor { get; private set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        protected InformacaoAdicional() {}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        private InformacaoAdicional(int clausulaGeralEstruturaId, string valor)
        {
            ClausulaGeralEstruturaId = clausulaGeralEstruturaId;
            Valor = valor;
        }

        public static Result<InformacaoAdicional> Criar(int clausulaGeralEstruturaId, string valor)
        {
            if (clausulaGeralEstruturaId <= 0) return Result.Failure<InformacaoAdicional>("Informe o id");
            if (valor is null) return Result.Failure<InformacaoAdicional>("Informe o Valor");

            var informacaoAdicionalItem = new InformacaoAdicional(clausulaGeralEstruturaId, valor);

            return Result.Success(informacaoAdicionalItem);
        }

        public Result Atualizar(int clausulaGeralEstruturaId, string valor)
        {
            if (clausulaGeralEstruturaId <= 0) return Result.Failure("Informe o id");
            if (valor is null) return Result.Failure("Informe o Valor");

            ClausulaGeralEstruturaId = clausulaGeralEstruturaId;
            Valor = valor;

            return Result.Success();
        }

        protected override bool EqualsCore(InformacaoAdicional other)
        {
            return ClausulaGeralEstruturaId == other.ClausulaGeralEstruturaId
                && Valor == other.Valor;
    }

        protected override int GetHashCodeCore()
        {
            return GetType().GetHashCode();
        }
    }
}
