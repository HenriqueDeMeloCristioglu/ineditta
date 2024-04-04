using CSharpFunctionalExtensions;

namespace Ineditta.Application.Acompanhamentos.CctsStatusOpcoes.Entities
{
    public class AcompanhamentoCctStatus : Entity
    {
        public const long IndiceStatusCliente = 1;
        public const long IndiceStatusAlta = 2;
        public const long IndiceStatusMedia = 3;
        public const long IndiceStatusBaixa = 4;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        protected AcompanhamentoCctStatus() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        private AcompanhamentoCctStatus(string descricao)
        {
            Descricao = descricao;
        }

        public string Descricao { get; private set; }

        public static Result<AcompanhamentoCctStatus> Criar(string descricao)
        {
            if (descricao == null)
            {
                return Result.Failure<AcompanhamentoCctStatus>("A descrição não pode ser nula");
            }

            var acompanhamentoCctStatus = new AcompanhamentoCctStatus(descricao);

            return Result.Success(acompanhamentoCctStatus);
        }

        public Result Atualizar(string descricao)
        {
            if (descricao == null)
            {
                return Result.Failure<AcompanhamentoCctStatus>("A descrição não pode ser nula");
            }

            Descricao = descricao;

            return Result.Success();
        }
    }
}
