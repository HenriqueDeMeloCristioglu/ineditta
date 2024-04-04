using CSharpFunctionalExtensions;

namespace Ineditta.Application.ClientesUnidadesSindicatosPatronais.Entities
{
    public class ClienteUnidadeSindicatoPatronal : Entity
    {
        private ClienteUnidadeSindicatoPatronal(int clienteUnidadeId, int sindicatoPatronalId)
        {
            ClienteUnidadeId = clienteUnidadeId;
            SindicatoPatronalId = sindicatoPatronalId;
        }

        protected ClienteUnidadeSindicatoPatronal() { }

        public int ClienteUnidadeId { get; set; }
        public int SindicatoPatronalId { get; set; }

        public static Result<ClienteUnidadeSindicatoPatronal> Criar(int clienteUnidadeId, int sindicatoPatronalId)
        {
            if (clienteUnidadeId <= 0) return Result.Failure<ClienteUnidadeSindicatoPatronal>("O id do cliente unidade deve ser maior que 0");
            if (sindicatoPatronalId <= 0) return Result.Failure<ClienteUnidadeSindicatoPatronal>("O id do sindicato patronal deve ser maior que 0");

            var clienteUnidadeSindicatoPatronal = new ClienteUnidadeSindicatoPatronal(clienteUnidadeId, sindicatoPatronalId);

            return Result.Success(clienteUnidadeSindicatoPatronal);
        }
    }
}
