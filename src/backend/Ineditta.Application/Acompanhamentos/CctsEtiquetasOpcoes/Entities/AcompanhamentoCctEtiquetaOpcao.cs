using CSharpFunctionalExtensions;

namespace Ineditta.Application.Acompanhamentos.CctsEtiquetasOpcoes.Entities
{
    public class AcompanhamentoCctEtiquetaOpcao : Entity
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        protected AcompanhamentoCctEtiquetaOpcao() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private AcompanhamentoCctEtiquetaOpcao(string descricao)
        {
            Descricao = descricao;
        }

        public string Descricao { get; private set; }

        public static Result<AcompanhamentoCctEtiquetaOpcao> Criar(string descricao)
        {
            if (descricao == null)
            {
                return Result.Failure<AcompanhamentoCctEtiquetaOpcao>("Descrição não pode ser nula");
            }

            var acompanhamentoCctEtiquetaOpcao = new AcompanhamentoCctEtiquetaOpcao(descricao);

            return Result.Success(acompanhamentoCctEtiquetaOpcao);
        }
    }
}
