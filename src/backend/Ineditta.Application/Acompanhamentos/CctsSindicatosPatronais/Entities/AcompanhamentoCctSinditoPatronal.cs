using CSharpFunctionalExtensions;

namespace Ineditta.Application.Acompanhamentos.CctsSindicatosPatronais.Entities
{
    public class AcompanhamentoCctSinditoPatronal : Entity
    {
        protected AcompanhamentoCctSinditoPatronal() { }
        private AcompanhamentoCctSinditoPatronal(long acompanhamentoCctId, int sindicatoId)
        {
            AcompanhamentoCctId = acompanhamentoCctId;
            SindicatoId = sindicatoId;
        }

        public long AcompanhamentoCctId { get; private set; }
        public int SindicatoId { get; private set; }

        public static Result<AcompanhamentoCctSinditoPatronal> Criar(long acompanhamentoCctId, int sindicatoId)
        {
            if (acompanhamentoCctId <= 0)
            {
                return Result.Failure<AcompanhamentoCctSinditoPatronal>("O Id do acompanhamento cct não pode ser nulo");
            }

            if (sindicatoId <= 0)
            {
                return Result.Failure<AcompanhamentoCctSinditoPatronal>("O Id do sindicato não pode ser nulo");
            }

            var acompanhamentoCctEstabelecimento = new AcompanhamentoCctSinditoPatronal(acompanhamentoCctId, sindicatoId);

            return Result.Success(acompanhamentoCctEstabelecimento);
        }
    }
}
