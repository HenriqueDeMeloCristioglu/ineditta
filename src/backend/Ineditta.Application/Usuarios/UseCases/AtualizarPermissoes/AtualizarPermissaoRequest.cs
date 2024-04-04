using CSharpFunctionalExtensions;

using MediatR;

namespace Ineditta.Application.Usuarios.UseCases.AtualizarPermissoes
{
    public class AtualizarPermissaoRequest: IRequest<Result>
    {
        public int Id { get; set; }
    }
}
