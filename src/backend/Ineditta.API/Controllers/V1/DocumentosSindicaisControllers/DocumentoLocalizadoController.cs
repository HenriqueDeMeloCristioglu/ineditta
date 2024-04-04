using System.Net.Mime;
using Ineditta.BuildingBlocks.Core.Web.API.Controllers;
using Ineditta.BuildingBlocks.Core.Web.API.Datatables.Medias;
using Ineditta.BuildingBlocks.Core.Web.API.Datatables;
using Ineditta.BuildingBlocks.Core.Web.API.Validators;
using Ineditta.BuildingBlocks.Core.Web.OpenAPI.Filters;
using Ineditta.Repository.Contexts;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ineditta.Application.DocumentosLocalizados.Entities;
using Ineditta.BuildingBlocks.Core.Domain.Models;
using Ineditta.BuildingBlocks.Core.FileStorage;
using Ineditta.BuildingBlocks.Core.Web.API.Dtos;
using Microsoft.Extensions.Options;
using Ineditta.API.ViewModels.DocumentosLocalizados.ViewModels;
using Ineditta.API.ViewModels.DocumentosLocalizados.Requests;
using Ineditta.Application.Documentos.Localizados.UseCases.Upsert;
using Ineditta.Application.Documentos.Sindicais.Entities;
using Ineditta.Application.Documentos.Localizados.UseCases.Aprovar;
using Ineditta.Application.Documentos.Localizados.UseCases.Deletar;

namespace Ineditta.API.Controllers.V1.DocumentosSindicais
{
    [Route("v{version:apiVersion}/documentos-localizados")]
    [ApiController]
    public class DocumentoLocalizadoController : ApiBaseController
    {
        private readonly InedittaDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly FileStorageConfiguration _fileStorageConfiguration;

        public DocumentoLocalizadoController(IMediator mediator, RequestStateValidator requestStateValidator, InedittaDbContext context, IHttpClientFactory httpClientFactory, IOptions<FileStorageConfiguration> fileStorageConfiguration) : base(mediator, requestStateValidator)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
            _fileStorageConfiguration = fileStorageConfiguration?.Value ?? throw new ArgumentNullException(nameof(fileStorageConfiguration));
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(DataTableResponse<DocumentosLocalizadosDataTableNaoAprovadosViewModel>), DatatableMediaType.ContentType)]
        public async ValueTask<IActionResult> ObterTodosAsync([FromQuery] DocumentosLocalizadosRequestQuery request)
        {
            if (Request.Headers.Accept == DatatableMediaType.ContentType)
            {
                if (request.NaoAprovados != null && request.NaoAprovados == true)
                {
                    var result = await (from dl in _context.DocumentosLocalizados
                                        where dl.Situacao == Situacao.NaoAprovado
                                        select new DocumentosLocalizadosDataTableNaoAprovadosViewModel
                                        {
                                            Id = dl.Id,
                                            Nome = dl.NomeDocumento ?? "",
                                            Caminho = dl.CaminhoArquivo,
                                            DataRegistro = dl.DataRegistro,
                                            Origem = dl.Origem
                                        }).AsNoTracking()
                                        .ToDataTableResponseAsync(request);

                    if (result == null)
                    {
                        return NoContent();
                    }

                    return Ok(result);
                }

                return NoContent();
            }

            return NoContent();
        }

        [HttpGet]
        [Route("aprovados")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(DocumentosLocalizadosAprovadosViewModel), MediaTypeNames.Application.Json)]
        public async ValueTask<IActionResult> ObterTodosAprovadosAsync()
        {
            if (!Request.Headers.Accept.Any(accept => accept!.Contains(MediaTypeNames.Application.Json)))
            {
                return NotAcceptable();
            }

            var result = await (from dl in _context.DocumentosLocalizados
                                where dl.Situacao == Situacao.Aprovado
                                select new DocumentosLocalizadosAprovadosViewModel
                                {
                                    Id = dl.Id,
                                    Nome = dl.NomeDocumento ?? "",
                                    Caminho = dl.CaminhoArquivo,
                                    DataRegistro = dl.DataRegistro,
                                    DataAprovacao = dl.DataAprovacao,
                                    Origem = dl.Origem
                                }).AsNoTracking()
                                .ToListAsync();

            if (result == null)
            {
                return NoContent();
            }

            return Ok(result);
        }

        [HttpGet]
        [Route("aprovados/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(DocumentosLocalizadosAprovadosViewModel), MediaTypeNames.Application.Json)]
        public async ValueTask<IActionResult> ObterAprovadoPorIdAsync([FromRoute] long id)
        {
            if (!Request.Headers.Accept.Any(accept => accept!.Contains(MediaTypeNames.Application.Json)))
            {
                return NotAcceptable();
            }

            var result = await (from dl in _context.DocumentosLocalizados
                                where dl.Situacao == Situacao.Aprovado && dl.Id == id
                                select new DocumentosLocalizadosAprovadosViewModel
                                {
                                    Id = dl.Id,
                                    Nome = dl.NomeDocumento ?? "",
                                    Caminho = dl.CaminhoArquivo,
                                    DataRegistro = dl.DataRegistro,
                                    DataAprovacao = dl.DataAprovacao,
                                    Origem = dl.Origem
                                }).AsNoTracking()
                                .ToListAsync();

            if (result == null)
            {
                return NoContent();
            }

            return Ok(result);
        }

        [HttpPost]
        public async ValueTask<IActionResult> AdicionarDocumentoLocalizado([FromForm] UpsertDocumentoLocalizadoRequest request)
        {
            if (request.Arquivo is null) return NoContent();

            return await Dispatch(request);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Stream), StatusCodes.Status200OK)]
        [SwaggerResponseMimeType(StatusCodes.Status404NotFound, typeof(Envelope), MediaTypeNames.Application.Octet)]
        [SwaggerResponseMimeType(StatusCodes.Status400BadRequest, typeof(Envelope), MediaTypeNames.Application.Octet)]
        public async ValueTask<IActionResult> DonwloadDocumentoAsync([FromRoute] long id)
        {
            if (id <= 0)
            {
                return BadRequest(Errors.Http.BadRequest("Informe um id válido"));
            }

            var documentoLocalizado = await _context.DocumentosLocalizados.FirstOrDefaultAsync(d => d.Id == id);

            if (documentoLocalizado is null) return NotFound(Errors.Http.NotFound());

            if (Uri.TryCreate(documentoLocalizado.CaminhoArquivo, UriKind.Absolute, out var uri))
            {
                return await DownloadFileFromUri(uri, async (uri) =>
                {
                    using var httpClient = _httpClientFactory.CreateClient();

                    var response = await httpClient.GetAsync(uri);

                    return response.IsSuccessStatusCode ? await response.Content.ReadAsByteArrayAsync() : null;
                });
            }
            var filePath = $"{_fileStorageConfiguration.Path}/{DocumentoSindical.PastaDocumento}/{documentoLocalizado.CaminhoArquivo}";

            return DownloadFile(filePath);
        }

        [HttpGet("documentos-sindicais/{id}")]
        [ProducesResponseType(typeof(Stream), StatusCodes.Status200OK)]
        [SwaggerResponseMimeType(StatusCodes.Status404NotFound, typeof(Envelope), MediaTypeNames.Application.Octet)]
        [SwaggerResponseMimeType(StatusCodes.Status400BadRequest, typeof(Envelope), MediaTypeNames.Application.Octet)]
        public async ValueTask<IActionResult> DonwloadDocumentoPorDocumentoSindicalIdAsync([FromRoute] long id)
        {
            if (id <= 0)
            {
                return BadRequest(Errors.Http.BadRequest("Informe um id válido"));
            }

            var documentoSindical = await _context.DocSinds.FirstOrDefaultAsync(d => d.Id == id);

            if (documentoSindical is null) return NotFound(Errors.Http.NotFound());

            var documentoLocalizado = await _context.DocumentosLocalizados.FirstOrDefaultAsync(d => d.Id == documentoSindical.DocumentoLocalizacaoId);

            if (documentoLocalizado is null) return NotFound(Errors.Http.NotFound());

            if (Uri.TryCreate(documentoLocalizado.CaminhoArquivo, UriKind.Absolute, out var uri))
            {
                return await DownloadFileFromUri(uri, async (uri) =>
                {
                    using var httpClient = _httpClientFactory.CreateClient();

                    var response = await httpClient.GetAsync(uri);

                    return response.IsSuccessStatusCode ? await response.Content.ReadAsByteArrayAsync() : null;
                });
            }
            var filePath = $"{_fileStorageConfiguration.Path}/{DocumentoSindical.PastaDocumento}/{documentoLocalizado.CaminhoArquivo}";

            return DownloadFile(filePath);
        }

        [HttpPatch("{id}/aprovar")]
        public async ValueTask<IActionResult> AprovarDocumentoAsync([FromRoute] long id)
        {
            var request = new AprovarDocumentoLocalizadoRequest { Id = id };
            return await Dispatch(request);
        }

        [HttpDelete("{id}")]
        public async ValueTask<IActionResult> DeletarDocumento([FromRoute] long id)
        {
            var request = new DeletarDocumentoLocalizadoRequest { Id = id };
            return await Dispatch(request);
        }
    }
}
