using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CSharpFunctionalExtensions;

using Ineditta.Application.CalendarioSindicais.Usuarios.Entities;

using MediatR;

namespace Ineditta.Application.CalendarioSindicais.Usuarios.UseCases.Upsert
{
    public class UpsertCalendarioSindicalUsuarioRequest : IRequest<Result>
    {
        public long? Id { get; set; }
        public string Titulo { get; set; } = null!;
        public DateTime DataHora { get; set; }
        public TipoRecorrencia? Recorrencia { get; set; }
        public DateTime? ValidadeRecorrencia { get; set; }
        public int? NotificarAntes { get; set; }
        public string Local { get; set; } = null!;
        public string? Comentarios { get; set; } = null!;
        public bool Visivel { get; set; }
    }
}
