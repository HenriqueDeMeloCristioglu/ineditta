using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CSharpFunctionalExtensions;

using Ineditta.Application.Federacoes.Entities;

namespace Ineditta.Application.IndicadoresEconomicos.Entities
{
    public class IndicadorEconomico : Entity
    {
        public IndicadorEconomico( string origem, string indicador, int idUsuario, string fonte, string data, float? dadoProjetado, DateTime? criado)
        {
            Origem = origem;
            Indicador = indicador;
            IdUsuario = idUsuario;
            Fonte = fonte;
            Data = data;
            CriadoEm = criado;
            DadoProjetado = dadoProjetado;
        }

        public IndicadorEconomico(int id, string origem, string indicador, int idUsuario, string fonte, string data, float? dadoProjetado, DateTime? criado)
        {
            Id = id;
            Origem = origem;
            Indicador = indicador;
            IdUsuario = idUsuario;
            Fonte = fonte;
            Data = data;
            CriadoEm = criado;
            DadoProjetado = dadoProjetado;
            ClienteGrupoId = 0;
        }

        public string Origem { get; private set; }
        public string Indicador { get; private set; }
        public int IdUsuario { get; private set; }
        public string Fonte { get; private set; }
        public string Data { get; private set; }
        public float? DadoProjetado { get; private set; }
        public DateTime? CriadoEm { get; private set; }
        public int ClienteGrupoId { get; private set; }

        public static Result<IndicadorEconomico> Criar(string? origem, string? indicador, int idUsuario, string? fonte, string? data, float? dadoProjetado, DateTime criado)
        {
            if (string.IsNullOrEmpty(origem))
            {
                return Result.Failure<IndicadorEconomico>("Informe a origem");
            }

            if (origem.Length > 100)
            {
                return Result.Failure<IndicadorEconomico>("Origem deve ter no máximo 100 caracteres");
            }

            if (string.IsNullOrEmpty(indicador))
            {
                return Result.Failure<IndicadorEconomico>("Informe o indicador");
            }

            if (indicador.Length > 50)
            {
                return Result.Failure<IndicadorEconomico>("Indicador deve ter no máximo 50 caracteres");
            }

            if (idUsuario <= 0)
            {
                return Result.Failure<IndicadorEconomico>("Informe o id do usuário");
            }

            if (string.IsNullOrEmpty(fonte))
            {
                return Result.Failure<IndicadorEconomico>("Informe a fonte");
            }

            if (fonte.Length > 500)
            {
                return Result.Failure<IndicadorEconomico>("Fonte deve ter no máximo 500 caracteres");
            }

            if (string.IsNullOrEmpty(data))
            {
                return Result.Failure<IndicadorEconomico>("Informe a data");
            }

            if (data.Length > 10)
            {
                return Result.Failure<IndicadorEconomico>("Data deve ter no máximo 10 caracteres");
            }

            var indicadorEconomico = new IndicadorEconomico(origem, indicador, idUsuario, fonte, data, dadoProjetado, criado);

            return Result.Success(indicadorEconomico);
        }

        public Result Atualizar(string? origem, string? indicador, int idUsuario, string? fonte, string? data, float? dadoProjetado)
        {
            if (string.IsNullOrEmpty(origem))
            {
                return Result.Failure<IndicadorEconomico>("Informe a origem");
            }

            if (origem.Length > 100)
            {
                return Result.Failure<IndicadorEconomico>("Origem deve ter no máximo 100 caracteres");
            }

            if (string.IsNullOrEmpty(indicador))
            {
                return Result.Failure<IndicadorEconomico>("Informe o indicador");
            }

            if (indicador.Length > 50)
            {
                return Result.Failure<IndicadorEconomico>("Indicador deve ter no máximo 50 caracteres");
            }

            if (idUsuario <= 0)
            {
                return Result.Failure<IndicadorEconomico>("Informe o id do usuário");
            }

            if (string.IsNullOrEmpty(fonte))
            {
                return Result.Failure<IndicadorEconomico>("Informe a fonte");
            }

            if (fonte.Length > 500)
            {
                return Result.Failure<IndicadorEconomico>("Fonte deve ter no máximo 500 caracteres");
            }

            if (string.IsNullOrEmpty(data))
            {
                return Result.Failure<IndicadorEconomico>("Informe a data");
            }

            if (data.Length > 10)
            {
                return Result.Failure<IndicadorEconomico>("Data deve ter no máximo 10 caracteres");
            }

            Origem = origem;
            Indicador = indicador;
            IdUsuario = idUsuario;
            Fonte = fonte;
            Data = data;
            DadoProjetado = dadoProjetado;

            return Result.Success();
        }
    }
}
