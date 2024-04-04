using CSharpFunctionalExtensions;

using MediatR;

namespace Ineditta.Application.Documentos.Sindicais.UseCases.NotificarCriacao
{
    public class NotificarCriacaoRequest : IRequest<Result>
    {
        public long DocumentoId { get; set; }
        public IEnumerable<long> UsuariosParaNotificarIds { get; set; } = null!;
    }
}
