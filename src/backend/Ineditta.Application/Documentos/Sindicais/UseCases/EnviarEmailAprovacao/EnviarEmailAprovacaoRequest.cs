using CSharpFunctionalExtensions;

using MediatR;

namespace Ineditta.Application.Documentos.Sindicais.UseCases.EnviarEmailAprovacao
{
    public class EnviarEmailAprovacaoRequest : IRequest<Result>
    {
        public long DocumentoId { get; set; }
        public IEnumerable<int>? UsuariosIds { get; set; }
    }
}
