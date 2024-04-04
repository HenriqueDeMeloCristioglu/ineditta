using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CSharpFunctionalExtensions;

namespace Ineditta.Application.ModulosClientes.Entities
{
    public class ModuloCliente : Entity<int>
    {
        private ModuloCliente(DateOnly dataInicio, int moduloId, int clienteMatrizId)
        {
            DataInicio = dataInicio;
            ModuloId = moduloId;
            ClienteMatrizId = clienteMatrizId;
        }

        protected ModuloCliente() { }

        public DateOnly DataInicio { get; private set; }
        public DateOnly? DataFim { get; private set; }
        public int ModuloId { get; private set; }
        public int ClienteMatrizId { get; private set; }

        public static Result<ModuloCliente> Criar(DateOnly dataInicio, int moduloId, int clienteMatrizId)
        {
            if (moduloId <= 0) return Result.Failure<ModuloCliente>("O id do módulo deve ser maior que 0");

            if (clienteMatrizId <= 0) return Result.Failure<ModuloCliente>("O id do cliente matriz deve ser maior que 0");

            var clienteModulo = new ModuloCliente(
                dataInicio,
                moduloId,
                clienteMatrizId
            );

            return clienteModulo;
        }

        public Result Finalizar()
        {
            DataFim = DateOnly.FromDateTime(DateTime.Now);
            return Result.Success();
        }
    }
}
