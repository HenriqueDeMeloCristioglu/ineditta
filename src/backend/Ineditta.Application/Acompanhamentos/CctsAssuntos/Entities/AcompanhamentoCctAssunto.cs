using CSharpFunctionalExtensions;

using Ineditta.Application.EstruturasClausulas.Gerais.Entities;

namespace Ineditta.Application.Acompanhamentos.CctsAssuntos.Entities
{
    public class AcompanhamentoCctAssunto : Entity
    {
        protected AcompanhamentoCctAssunto() { }
        private AcompanhamentoCctAssunto(int estrutucaClausulaId)
        {
            EstrutucaClausulaId = estrutucaClausulaId;
        }
        public int EstrutucaClausulaId { get; private set; }

        internal static Result<AcompanhamentoCctAssunto> Criar(EstruturaClausula estrutucaClausula)
        {
            if (estrutucaClausula is null)
            {
                return Result.Failure<AcompanhamentoCctAssunto>("Informe a estrutura de clausula");
            }

            var acompanhamentoCctAssunto = new AcompanhamentoCctAssunto(estrutucaClausula.Id);

            return Result.Success(acompanhamentoCctAssunto);
        }
    }
}
