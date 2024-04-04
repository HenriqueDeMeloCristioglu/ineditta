using CSharpFunctionalExtensions;

namespace Ineditta.Application.CctsFases.Entities
{
    public partial class FasesCct : Entity
    {
        public const int IndiceAssembleiaPatronal = 1;
        public const int IndiceNegociacaoNaoIniciada = 2;
        public const int IndiceEmNegociacao = 2;
        public const int IndiceFaseFechada = 6;
        public const int IndiceFaseConcluida = 7;
        public const int IndiceFaseArquivada = 8;

        public const int IndiceTipoFaseCct = 1;
        public const int IndiceTipoFaseCcts = 2;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        protected FasesCct() { }
    #pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        private FasesCct(string fase, Prioridade? prioridade, Periodicidade? periodicidade, Tipo? tipo)
        {
            Fase = fase;
            Prioridade = prioridade;
            Periodicidade = periodicidade;
            Tipo = tipo;
        }

        public string Fase { get; private set; }

        public Prioridade? Prioridade { get; private set; }

        public Periodicidade? Periodicidade { get; private set; }

        public Tipo? Tipo { get; private set; }


        public static Result<FasesCct> Criar(string fase, Prioridade? prioridade, Periodicidade? periodicidade, Tipo? tipo)
        {
            if (fase is null)
            {
                return Result.Failure<FasesCct>("A Fase não pode ser nulla");
            }

            if (prioridade < 0)
            {
                return Result.Failure<FasesCct>("A Prioridade não pode ser nulla");
            }

            if (periodicidade < 0)
            {
                return Result.Failure<FasesCct>("A Periodicidade não pode ser nulla");
            }

            var acompanhamentoCct = new FasesCct(fase, prioridade, periodicidade, tipo);

            return Result.Success(acompanhamentoCct);
        }

        public Result Atualizar(string fase, Prioridade? prioridade, Periodicidade? periodicidade, Tipo? tipo)
        {
            if (fase is null)
            {
                return Result.Failure<FasesCct>("A Fase não pode ser nulla");
            }

            if (prioridade < 0)
            {
                return Result.Failure<FasesCct>("A Prioridade não pode ser nulla");
            }

            if (periodicidade < 0)
            {
                return Result.Failure<FasesCct>("A Periodicidade não pode ser nulla");
            }

            Fase = fase;
            Prioridade = prioridade;
            Periodicidade = periodicidade;
            Tipo = tipo;

            return Result.Success();
        }
    }
}
