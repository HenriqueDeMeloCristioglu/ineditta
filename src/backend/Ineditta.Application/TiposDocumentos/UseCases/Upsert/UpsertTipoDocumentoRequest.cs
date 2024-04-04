using CSharpFunctionalExtensions;

using Ineditta.Application.TiposDocumentos.Entities;

using MediatR;

namespace Ineditta.Application.TiposDocumentos.UseCases.Upsert
{
    public class UpsertTipoDocumentoRequest : IRequest<Result<int>>
    {
        public int? Id { get; set; }
        public required string Tipo { get; set; }
        public required string Nome { get; set; }
        public string? Sigla { get; set; }
        public string? Processado { get; set; }
        public required TipoModuloCadastro Modulo { get; set; }
    }
}
