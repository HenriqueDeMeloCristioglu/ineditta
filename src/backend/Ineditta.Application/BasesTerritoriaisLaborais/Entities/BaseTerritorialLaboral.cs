using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CSharpFunctionalExtensions;
using Ineditta.Application.BasesTerritoriaisPatronais.Entities;
using Ineditta.Application.Sindicatos.Laborais.Entities;
using Ineditta.BuildingBlocks.Core.Domain.Contracts;

namespace Ineditta.Application.BasesTerritoriaisLaborais.Entities
{
    public class BaseTerritorialLaboral : Entity<int>, IAuditable
    {
        private BaseTerritorialLaboral(int sindicatoId, int localizacaoId, int cnaeId, DataNegociacao dataNegociacao)
        {
            SindicatoId = sindicatoId;
            LocalizacaoId = localizacaoId;
            CnaeId = cnaeId;
            DataInicial = DateOnly.FromDateTime(DateTime.Today);
            DataNegociacao = dataNegociacao;
        }

        protected BaseTerritorialLaboral() { }

        public int SindicatoId { get; private set; }
        public int LocalizacaoId { get; private set; }
        public int CnaeId { get; private set; }
        public DataNegociacao DataNegociacao { get; set; }
        public DateOnly DataInicial { get; private set; }
        public DateOnly? DataFinal { get; private set; }


        public static Result<BaseTerritorialLaboral> Criar(SindicatoLaboral sindicatoLaboral, int localizacaoId, int cnaeId, DataNegociacao dataNegociacao)
        {
            if (sindicatoLaboral is null)
            {
                return Result.Failure<BaseTerritorialLaboral>("Informe o sindicato laboral");
            }

            if (localizacaoId <= 0)
            {
                return Result.Failure<BaseTerritorialLaboral>("O id da localização deve ser válido");
            }

            if (cnaeId <= 0)
            {
                return Result.Failure<BaseTerritorialLaboral>("O id do cnae deve ser válido");
            }

            var baseTerritorialLaboral = new BaseTerritorialLaboral(sindicatoLaboral.Id, localizacaoId, cnaeId, dataNegociacao);

            return Result.Success(baseTerritorialLaboral);
        }

        public Result Finalizar()
        {
            DataFinal = DateOnly.FromDateTime(DateTime.Today);

            return Result.Success();
        }
    }
}
