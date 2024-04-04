using CSharpFunctionalExtensions;

using Ineditta.Application.Sindicatos.Patronais.Entities;
using Ineditta.BuildingBlocks.Core.Domain.Contracts;

namespace Ineditta.Application.BasesTerritoriaisPatronais.Entities
{
    public class BaseTerritorialPatronal : Entity<int>, IAuditable
    {
        private BaseTerritorialPatronal(int sindicatoId, int localizacaoId, int cnaeId)
        {
            SindicatoId = sindicatoId;
            LocalizacaoId = localizacaoId;
            CnaeId = cnaeId;
            DataInicial = DateOnly.FromDateTime(DateTime.Today);
        }

        protected BaseTerritorialPatronal() { }

        public int SindicatoId { get; private set; }
        public int LocalizacaoId { get; private set; }
        public int CnaeId { get; private set; }
        public DateOnly DataInicial { get; private set; }
        public DateOnly? DataFinal { get; private set; }


        public static Result<BaseTerritorialPatronal> Criar(SindicatoPatronal sindicatoPatronal, int localizacaoId, int cnaeId)
        {
            if (sindicatoPatronal is null)
            {
                return Result.Failure<BaseTerritorialPatronal>("Informe o sindicato patronal");
            }

            if (localizacaoId <= 0)
            {
                return Result.Failure<BaseTerritorialPatronal>("O id da localização deve ser válido");
            }

            if (cnaeId <= 0)
            {
                return Result.Failure<BaseTerritorialPatronal>("O id do cnae deve ser válido");
            }

            var baseTerritorialPatronal = new BaseTerritorialPatronal(sindicatoPatronal.Id, localizacaoId, cnaeId);

            return Result.Success(baseTerritorialPatronal);
        }

        public Result Finalizar()
        {
            DataFinal = DateOnly.FromDateTime(DateTime.Today);

            return Result.Success();
        }
    }
}
