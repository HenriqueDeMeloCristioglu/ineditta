using CSharpFunctionalExtensions;

using MediatR;

namespace Ineditta.Application.Acompanhamentos.CctsSindicatosLaborais.UseCases.Incluir
{
    public class IncluirAcompanhamentoCctSindicatoLaboralRequest : IRequest<Result>
    {
        public long AcompanhamentoCctId { get; set; }
        public int SindicatoId { get; set; }
    }
}
