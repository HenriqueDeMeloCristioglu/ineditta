using CSharpFunctionalExtensions;

using Ineditta.Application.Acompanhamentos.CctsEtiquetasOpcoes.Entities;

namespace Ineditta.Application.Acompanhamentos.CctsEtiquetas.Entities
{
    public class AcompanhamentoCctEtiqueta : Entity
    {
        protected AcompanhamentoCctEtiqueta() { }
        public AcompanhamentoCctEtiqueta(long acompanhamentoCctEtiquetaOpcaoId)
        {
            AcompanhamentoCctEtiquetaOpcaoId = acompanhamentoCctEtiquetaOpcaoId;
        }

        public long AcompanhamentoCctEtiquetaOpcaoId { get; set; }

        internal static Result<AcompanhamentoCctEtiqueta> Criar(AcompanhamentoCctEtiquetaOpcao acompanhamentoCctEtiquetaOpcao)
        {
            if (acompanhamentoCctEtiquetaOpcao is null)
            {
                return Result.Failure<AcompanhamentoCctEtiqueta>("Informe a opção da etiqueta");
            }

            var acompanhamentoCctEtiqueta = new AcompanhamentoCctEtiqueta(acompanhamentoCctEtiquetaOpcao.Id);

            return Result.Success(acompanhamentoCctEtiqueta);
        }
    }
}
