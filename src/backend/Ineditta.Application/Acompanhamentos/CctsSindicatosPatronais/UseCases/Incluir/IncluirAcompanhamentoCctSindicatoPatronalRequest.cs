using CSharpFunctionalExtensions;

using MediatR;

namespace Ineditta.Application.Acompanhamentos.CctsSindicatosPatronais.UseCases.Incluir
{
    public class IncluirAcompanhamentoCctSindicatoPatronalRequest : IRequest<Result>
    {
        public long AcompanhamentoCctId { get; set; }
        public int SindicatoId { get; set; }
    }
}
