using CSharpFunctionalExtensions;

using MediatR;

namespace Ineditta.Application.Usuarios.UseCases.AtualizacaoCredenciais
{
    public class EnviarEmailAtualizacaoCredenciaisRequest: IRequest<Result>
    {
        public int Id { get; set; }
    }
}
