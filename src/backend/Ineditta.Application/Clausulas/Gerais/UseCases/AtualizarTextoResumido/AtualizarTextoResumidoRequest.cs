using CSharpFunctionalExtensions;

using MediatR;

namespace Ineditta.Application.Clausulas.Gerais.UseCases.AtualizarTextoResumido
{
    public class AtualizarTextoResumidoRequest : IRequest<Result>
    {
        public int EstruturaId { get; set; }
        public int DocumentoId { get; set; }
        public string Texto { get; set; } = null!;
    }
}
