using CSharpFunctionalExtensions;

namespace Ineditta.Application.Acompanhamentos.CctsSindicatosLaborais.Entities
{
    public class AcompanhamentoCctSinditoLaboral : Entity
    {
        protected AcompanhamentoCctSinditoLaboral() { }
        private AcompanhamentoCctSinditoLaboral(long acompanhamentoCctId, int sindicatoId)
        {
            AcompanhamentoCctId = acompanhamentoCctId;
            SindicatoId = sindicatoId;
        }

        public long AcompanhamentoCctId { get; private set; }
        public int SindicatoId { get; private set; }

        public static Result<AcompanhamentoCctSinditoLaboral> Criar(long acompanhamentoCctId, int sindicatoId)
        {
            if (acompanhamentoCctId <= 0)
            {
                return Result.Failure<AcompanhamentoCctSinditoLaboral>("O Id do acompanhamento cct não pode ser nulo");
            }

            if (sindicatoId <= 0)
            {
                return Result.Failure<AcompanhamentoCctSinditoLaboral>("O Id do sindicato não pode ser nulo");
            }

            var acompanhamentoCctEstabelecimento = new AcompanhamentoCctSinditoLaboral(acompanhamentoCctId, sindicatoId);

            return Result.Success(acompanhamentoCctEstabelecimento);
        }
    }
}
