using CSharpFunctionalExtensions;

namespace Ineditta.Application.Acompanhamentos.CctsEstabelecimentos.Entities
{
    public class AcompanhamentoCctEstabelecimento : Entity
    {
        protected AcompanhamentoCctEstabelecimento() { }
        private AcompanhamentoCctEstabelecimento(long acompanhamentoCctId, int grupoEconomicoId, int empresaId, int estabelecimentoId)
        {
            AcompanhamentoCctId = acompanhamentoCctId;
            GrupoEconomicoId = grupoEconomicoId;
            EmpresaId = empresaId;
            EstabelecimentoId = estabelecimentoId;
        }

        public long AcompanhamentoCctId { get; private set; }
        public int GrupoEconomicoId { get; private set; }
        public int EmpresaId { get; private set; }
        public int EstabelecimentoId { get; private set; }

        public static Result<AcompanhamentoCctEstabelecimento> Criar(long acompanhamentoCctId, int grupoEconomicoId, int empresaId, int estabelecimentoId)
        {
            if (acompanhamentoCctId <= 0)
            {
                return Result.Failure<AcompanhamentoCctEstabelecimento>("O Id do acompanhamento cct não pode ser nulo");
            }

            if (grupoEconomicoId <= 0)
            {
                return Result.Failure<AcompanhamentoCctEstabelecimento>("O Id do grupo economico não pode ser nulo");
            }

            if (empresaId <= 0)
            {
                return Result.Failure<AcompanhamentoCctEstabelecimento>("O Id da empresa não pode ser nulo");
            }

            if (estabelecimentoId <= 0)
            {
                return Result.Failure<AcompanhamentoCctEstabelecimento>("O Id do estabelecimento não pode ser nulo");
            }

            var acompanhamentoCctEstabelecimento = new AcompanhamentoCctEstabelecimento(acompanhamentoCctId, grupoEconomicoId, empresaId, estabelecimentoId);

            return Result.Success(acompanhamentoCctEstabelecimento);
        }
    }
}
