using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;

using CSharpFunctionalExtensions;

using Ineditta.Application.ClientesMatriz.Errors;
using Ineditta.BuildingBlocks.Core.Domain.ValueObjects;

namespace Ineditta.Application.ClientesMatriz.Entities
{
    public class ClienteMatriz : Entity<int>
    {
        public const string PastaDocumento = "logotipos_cliente_matriz";
        protected ClienteMatriz() { }
        private ClienteMatriz(string? codigo, string nome, int aberturaNegociacao, string slaPrioridade, int dataCorteForpag, TipoProcessamento tipoProcessamento, int grupoEconomicoId, string? logo)
        {
            Codigo = codigo;
            Nome = nome;
            AberturaNegociacao = aberturaNegociacao;
            SlaPrioridade = slaPrioridade;
            DataCorteForpag = dataCorteForpag;
            TipoProcessamento = tipoProcessamento;
            GrupoEconomicoId = grupoEconomicoId;
            Logo = logo;
        }

        public string? Codigo { get; private set; }
        public string Nome { get; private set; } = null!;
        public int AberturaNegociacao { get; private set; }
        public string SlaPrioridade { get; private set; } = null!;
        public DateTime? DataInativacao { get; private set; }
        public int DataCorteForpag { get; private set; }
        public TipoProcessamento TipoProcessamento { get; private set; }
        public int GrupoEconomicoId { get; private set; }
        public string? Logo { get; private set; }

        public static Result<ClienteMatriz> Criar(string? codigo, string nome, int aberturaNegociacao, string slaPrioridade, int dataCorteForpag, TipoProcessamento tipoProcessamento, int grupoEconomicoId, string? logo)
        {
            if (string.IsNullOrEmpty(nome)) return Result.Failure<ClienteMatriz>(ClienteMatrizError.NullOrEmptyArgumentError("nome"));

            if (aberturaNegociacao < 0 || aberturaNegociacao > 365) return Result.Failure<ClienteMatriz>("O valor de Abertura da Negociação não pode ser menor que 0 e nem maior que 365");

            if (string.IsNullOrEmpty(slaPrioridade)) return Result.Failure<ClienteMatriz>(ClienteMatrizError.NullOrEmptyArgumentError("Sla Prioridade"));

            if (dataCorteForpag < 0 || dataCorteForpag > 31) return Result.Failure<ClienteMatriz>("A Data corte Fopag não pode ser menor que 0 e nem maior que 365");

            if (grupoEconomicoId < 0) return Result.Failure<ClienteMatriz>("O id do grupo econômico deve ser maior que 0");

            var clienteMatriz = new ClienteMatriz(
                codigo,
                nome,
                aberturaNegociacao,
                slaPrioridade,
                dataCorteForpag,
                tipoProcessamento,
                grupoEconomicoId,
                logo
            );

            return Result.Success(clienteMatriz);
        }

        public Result Atualizar(string? codigo, string nome, int aberturaNegociacao, string slaPrioridade, int dataCorteForpag, TipoProcessamento tipoProcessamento, int grupoEconomicoId, string? logo)
        {
            if (string.IsNullOrEmpty(nome)) return Result.Failure<ClienteMatriz>(ClienteMatrizError.NullOrEmptyArgumentError("nome"));

            if (aberturaNegociacao < 0 || aberturaNegociacao > 365) return Result.Failure<ClienteMatriz>("O valor de Abertura da Negociação não pode ser menor que 0 e nem maior que 365");

            if (string.IsNullOrEmpty(slaPrioridade)) return Result.Failure<ClienteMatriz>(ClienteMatrizError.NullOrEmptyArgumentError("Sla Prioridade"));

            if (dataCorteForpag < 0 || dataCorteForpag > 31) return Result.Failure<ClienteMatriz>("A Data corte Fopag não pode ser menor que 0 e nem maior que 365");

            if (grupoEconomicoId < 0) return Result.Failure<ClienteMatriz>("O id do grupo econômico deve ser maior que 0");

            Codigo = codigo;
            Nome = nome;
            AberturaNegociacao = aberturaNegociacao;
            SlaPrioridade = slaPrioridade;
            DataCorteForpag = dataCorteForpag;
            TipoProcessamento = tipoProcessamento;
            GrupoEconomicoId = grupoEconomicoId;
            Logo = logo;

            return Result.Success();
        }

        public Result InativarAtivarToggle()
        {
            if (DataInativacao is null)
            {
                DataInativacao = DateTime.Now;
                return Result.Success();
            }
            else
            {
                DataInativacao = null;
                return Result.Success();
            }
        } 
    }
}
