using CSharpFunctionalExtensions;

using Ineditta.Application.Acompanhamentos.CctsSindicatosPatronais.Entities;

using MediatR;

namespace Ineditta.Application.Acompanhamentos.CctsSindicatosPatronais.UseCases.Deletar
{
    public class DeletarAcompanhamentoCctSindicatoPatronalRequest : IRequest<Result>
    {
        public AcompanhamentoCctSinditoPatronal AcompanhamentoCctSinditoPatronal { get; set; } = null!;
    }
}
