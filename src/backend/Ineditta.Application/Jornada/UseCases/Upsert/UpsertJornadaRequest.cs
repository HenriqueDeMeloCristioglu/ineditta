using CSharpFunctionalExtensions;
using MediatR;

namespace Ineditta.Application.Jornada.UseCases.Upsert
{
    public class UpsertJornadaRequest : IRequest<Result>
    {
        public string? JoranadaSemanal { get; set; }
        public string? Descricao { get; set; }
        public int IsDeault { get; set; }
    }
}
