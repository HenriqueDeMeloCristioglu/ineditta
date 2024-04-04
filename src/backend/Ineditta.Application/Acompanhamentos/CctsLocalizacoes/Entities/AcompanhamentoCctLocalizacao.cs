using CSharpFunctionalExtensions;

namespace Ineditta.Application.Acompanhamentos.CctsLocalizacoes.Entities
{
    public class AcompanhamentoCctLocalizacao : Entity
    {
        private AcompanhamentoCctLocalizacao(long acompanhamentoCctId, int localizacaoId)
        {
            AcompanhamentoCctId = acompanhamentoCctId;
            LocalizacaoId = localizacaoId;
        }

        public long AcompanhamentoCctId { get; private set; }
        public int LocalizacaoId { get; private set; }

        public static Result<AcompanhamentoCctLocalizacao> Criar(long acompanhamentoCctId, int localizacaoId)
        {
            if (acompanhamentoCctId <= 0)
            {
                return Result.Failure<AcompanhamentoCctLocalizacao>("Id do acompanhamento não pode ser nulo");
            }

            if (localizacaoId <= 0)
            {
                return Result.Failure<AcompanhamentoCctLocalizacao>("Id do localizacao não pode ser nulo");
            }

            var acompanhamentoCctLocalizacao = new AcompanhamentoCctLocalizacao(acompanhamentoCctId, localizacaoId);

            return Result.Success(acompanhamentoCctLocalizacao);
        }
    }
}
