using CSharpFunctionalExtensions;

using Ineditta.Application.Usuarios.Entities;
using Ineditta.BuildingBlocks.Core.Domain.Contracts;

namespace Ineditta.Application.Clausulas.Clientes.Entities
{
    public class ClausulaCliente : Entity, IAuditable
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        protected ClausulaCliente() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private ClausulaCliente(int clausulaId, string texto, int grupoEconomicoId)
        {
            ClausulaId = clausulaId;
            Texto = texto;
            GrupoEconomicoId = grupoEconomicoId;
        }

        public int ClausulaId { get; private set; }
        public string Texto { get; private set; }
        public int GrupoEconomicoId { get; private set; }

        public static Result<ClausulaCliente> Criar(int clausulaId, string texto, int grupoEconomicoId, Nivel usuarioNivel)
        {
            if (usuarioNivel == Nivel.Ineditta)
            {
                return Result.Failure<ClausulaCliente>("Usuário não pode ser Ineditta");
            }

            if (clausulaId <= 0)
            {
                return Result.Failure<ClausulaCliente>("Id da cláusula não pode ser nulo");
            }

            if (texto is null)
            {
                return Result.Failure<ClausulaCliente>("Texto não pode ser nulo");
            }

            if (grupoEconomicoId <= 0)
            {
                return Result.Failure<ClausulaCliente>("Id do grupo econômico não pode ser nulo");
            }

            var clausulaCliente = new ClausulaCliente(clausulaId, texto, grupoEconomicoId);

            return Result.Success(clausulaCliente);
        }

        public Result Atualizar(string texto, int grupoEconomicoId, Nivel usuarioNivel)
        {
            if (texto is null)
            {
                return Result.Failure<ClausulaCliente>("Texto não pode ser nulo");
            }

            if (usuarioNivel == Nivel.Ineditta)
            {
                return Result.Failure<ClausulaCliente>("Usuário não pode ser Ineditta");
            }

            if (grupoEconomicoId != GrupoEconomicoId)
            {
                return Result.Failure<ClausulaCliente>("Usuário não só pode alterar cláusula do mesmo grupo econômico");
            }

            Texto = texto;

            return Result.Success();
        }
    }
}
