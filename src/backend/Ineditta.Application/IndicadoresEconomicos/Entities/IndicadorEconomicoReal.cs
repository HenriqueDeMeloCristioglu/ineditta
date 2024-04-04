using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CSharpFunctionalExtensions;

using Ineditta.Application.Federacoes.Entities;

namespace Ineditta.Application.IndicadoresEconomicos.Entities
{
    public class IndicadorEconomicoReal : Entity
    {
        public IndicadorEconomicoReal(string indicador, DateOnly? data, float dadoReal, DateTime criado)
        {
            Indicador = indicador;
            Data = data;
            DadoReal = dadoReal;
            CriadoEm = criado;
        }
        public IndicadorEconomicoReal(int id, string indicador, DateOnly? data, float dadoReal, DateTime criado)
        {
            Id = id;
            Indicador = indicador;
            Data = data;
            DadoReal = dadoReal;
            CriadoEm = criado;
        }
        public string Indicador { get; private set; }
        public DateOnly? Data { get; private set; }
        public float DadoReal { get; private set; }
        public DateTime CriadoEm { get; private set; }
        public static Result<IndicadorEconomicoReal> Criar(string? indicador, DateOnly? data, float dadoReal, DateTime criado)
        {
            if (string.IsNullOrEmpty(indicador))
            {
                return Result.Failure<IndicadorEconomicoReal>("Informe o indicador");
            }

            if (indicador.Length > 50)
            {
                return Result.Failure<IndicadorEconomicoReal>("Indicador deve ter no máximo 50 caracteres");
            }

            var indicadorEconomicoReal = new IndicadorEconomicoReal(indicador, data, dadoReal, criado);

            return Result.Success(indicadorEconomicoReal);
        }

        public Result Atualizar(string? indicador, DateOnly? data, float dadoReal)
        {
            if (string.IsNullOrEmpty(indicador))
            {
                return Result.Failure<IndicadorEconomicoReal>("Informe o indicador");
            }

            if (indicador.Length > 50)
            {
                return Result.Failure<IndicadorEconomicoReal>("Indicador deve ter no máximo 50 caracteres");
            }

            Indicador = indicador;
            Data = data;
            DadoReal = dadoReal;

            return Result.Success();
        }
    }
}
