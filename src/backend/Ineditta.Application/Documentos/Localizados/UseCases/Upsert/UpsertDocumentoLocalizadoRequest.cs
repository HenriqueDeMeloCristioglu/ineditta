using CSharpFunctionalExtensions;

using Ineditta.Application.DocumentosLocalizados.Entities;

using MediatR;

using Microsoft.AspNetCore.Http;

namespace Ineditta.Application.Documentos.Localizados.UseCases.Upsert
{
    public sealed class UpsertDocumentoLocalizadoRequest : IRequest<Result>
    {
        public long? Id { get; set; }
        public string? NomeDocumento { get; set; }
        public string? Origem { get; set; }
        public IFormFile? Arquivo { get; set; } = null!;
        public string? CaminhoArquivo { get; set; }
        public Situacao? Situacao { get; set; }
        public DateOnly? DataAprovacao { get; set; }
        public bool Referenciado { get; set; }
        public long? IdLegado { get; set; }
        public string? Uf { get; set; }
    }
}
