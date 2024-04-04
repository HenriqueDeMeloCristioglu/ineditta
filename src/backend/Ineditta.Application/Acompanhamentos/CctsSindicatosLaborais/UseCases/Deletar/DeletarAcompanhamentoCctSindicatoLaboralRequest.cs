using CSharpFunctionalExtensions;

using Ineditta.Application.Acompanhamentos.CctsSindicatosLaborais.Entities;

using MediatR;

namespace Ineditta.Application.Acompanhamentos.CctsSindicatosLaborais.UseCases.Deletar
{
    public class DeletarAcompanhamentoCctSindicatoLaboralRequest : IRequest<Result>
    {
        public AcompanhamentoCctSinditoLaboral AcompanhamentoCctSinditoLaboral { get; set; } = null!;
    }
}
