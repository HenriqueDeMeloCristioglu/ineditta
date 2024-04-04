using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CSharpFunctionalExtensions;

using Ineditta.Application.ClientesMatriz.Entities;

using MediatR;

using Microsoft.AspNetCore.Http;

namespace Ineditta.Application.ClientesMatriz.UseCases.Upsert
{
    public class UpsertClienteMatrizRequest : IRequest<Result>
    {
        public int? Id { get; set; }
        public string? Codigo { get; set; }
        public string Nome { get; set; } = null!;
        public int AberturaNegociacao { get; set; }
        public string SlaPrioridade { get; set; } = null!;
        public int DataCorteForpag { get; set; }
        public TipoProcessamento TipoProcessamento { get; set; }
        public IEnumerable<int> TiposDocumentos { get; set; } = null!;
        public int GrupoEconomicoId { get; set; }
        public IFormFile? Logo { get; set; }
        public IEnumerable<int>? ModulosIds { get; set; }
    }
}
