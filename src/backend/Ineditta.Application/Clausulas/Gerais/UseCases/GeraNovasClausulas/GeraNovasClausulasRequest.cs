using CSharpFunctionalExtensions;

using Ineditta.Application.Usuarios.Entities;

using MediatR;

namespace Ineditta.Application.Clausulas.Gerais.UseCases.GeraNovasClausulas
{
    public class GeraNovasClausulasRequest : IRequest<Result>
    {
        public int DocumentoId { get; set; }
        public int UsuarioId { get; set; }
    }
}
