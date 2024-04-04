using CSharpFunctionalExtensions;

namespace Ineditta.Application.EstruturasClausulas.GruposEconomicos.Entities
{
    public class EstruturaClausulaGrupoEconomico : Entity
    {
        protected EstruturaClausulaGrupoEconomico() { }
        private EstruturaClausulaGrupoEconomico(int grupoEconomicoId)
        {
            GrupoEconomicoId = grupoEconomicoId;
        }

        public int GrupoEconomicoId { get; private set; }

        public static Result<EstruturaClausulaGrupoEconomico> Criar(EstruturaClausulaGrupoEconomico clausulaGrupoEconomico)
        {
            if (clausulaGrupoEconomico is null)
            {
                return Result.Failure<EstruturaClausulaGrupoEconomico>("Estrutura Cláusula Grupo Econômico não pode ser nulo");
            }

            var novaClausulaGrupoEconomico = new EstruturaClausulaGrupoEconomico(clausulaGrupoEconomico.GrupoEconomicoId);

            return Result.Success(novaClausulaGrupoEconomico);
        }
    }
}
