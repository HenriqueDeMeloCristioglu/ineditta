using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CSharpFunctionalExtensions;

using MediatR;

namespace Ineditta.Application.DirigentesPatronais.UseCases
{
    public class UpsertDirigentePatronalRequest : IRequest<Result>
    {
        public long? Id { get; set; }
        public string Nome { get; set; } = null!;
        public string Funcao { get; set; } = null!;
        public string? Situacao { get; set; }
        public DateOnly DataInicioMandato { get; set; }
        public DateOnly DataFimMandato { get; set; }
        public int SindicatoPatronalId { get; set; }
        public int? EstabelecimentoId { get; set; }
    }
}
