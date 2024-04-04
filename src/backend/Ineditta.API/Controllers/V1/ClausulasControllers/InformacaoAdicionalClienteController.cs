using System.Net.Mime;

using Ineditta.BuildingBlocks.Core.Web.API.Controllers;
using Ineditta.BuildingBlocks.Core.Web.API.Validators;

using MediatR;

using Microsoft.AspNetCore.Mvc;
using Ineditta.Application.InformacoesAdicionais.Cliente.UseCases;
using Ineditta.BuildingBlocks.Core.Web.API.Dtos;
using Ineditta.API.Filters;
using Ineditta.Application.InformacoesAdicionais.Cliente.Entities;
using Ineditta.API.Services;
using Ineditta.BuildingBlocks.Core.Web.OpenAPI.Filters;
using Ineditta.Application.InformacoesAdicionais.Cliente.Aprovar;
using Ineditta.API.Builders.FormulariosAplicacoes;
using Ineditta.API.Factories.InformacoesAdicionais.Cliente;

namespace Ineditta.API.Controllers.V1
{
    [Route("v{version:apiVersion}/informacoes-adicionais-clientes")]
    [ApiController]
    public class InformacaoAdicionalClienteController : ApiBaseController
    {
        private readonly HttpRequestItemService _httpRequestItemService;
        private readonly FormularioApliacacaoBuilder _formularioApliacacaoBuilder;
        private readonly InformacoesAdicionaisClienteFactory _informacoesAdicionaisClienteFactory;

        public InformacaoAdicionalClienteController(IMediator mediator, RequestStateValidator requestStateValidator, HttpRequestItemService httpRequestItemService, FormularioApliacacaoBuilder formularioApliacacaoBuilder, InformacoesAdicionaisClienteFactory informacoesAdicionaisClienteFactory) : base(mediator, requestStateValidator)
        {
            _httpRequestItemService = httpRequestItemService;
            _formularioApliacacaoBuilder = formularioApliacacaoBuilder;
            _informacoesAdicionaisClienteFactory = informacoesAdicionaisClienteFactory;
        }

        [HttpGet("{documentoId}")]
        [Produces(MediaTypeNames.Application.Json)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(IEnumerable<InformacaoAdicionalCliente>), MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
        [ServiceFilter(typeof(UserInfoActionFilter))]
        public async ValueTask<IActionResult> ObterPorDocumento([FromRoute] int documentoId)
        {
            if (!Request.Headers.Accept.Any(accept => accept!.Contains(MediaTypeNames.Application.Json)))
            {
                return NotAcceptable();
            }

            var usuario = _httpRequestItemService.ObterUsuario();

            if (usuario == null)
            {
                return Unauthorized();
            }

            var result = await _informacoesAdicionaisClienteFactory.CriarAsync(documentoId, usuario);

            if (result is null)
            {
                return NoContent();
            }

            return Ok(result);
        }

        [HttpPost("relatorio/{id}")]
        [Produces(MediaTypeNames.Application.Json)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(Stream), MediaTypeNames.Application.Octet)]
        [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ServiceFilter(typeof(UserInfoActionFilter))]
        public async ValueTask<IActionResult> ObterRelatorioPorId([FromRoute] int id)
        {
            if (!Request.Headers.Accept.Any(accept => accept!.Contains(MediaTypeNames.Application.Octet)))
            {
                return NotAcceptable();
            }

            var usuario = _httpRequestItemService.ObterUsuario();

            if (usuario == null)
            {
                return Unauthorized();
            }

            var relatorio = await _formularioApliacacaoBuilder.HandleAsync(id, usuario);

            if (relatorio.IsFailure)
            {
                return NotFound();
            }

            return DownloadExcelAsync("formulario-aplicacao", relatorio.Value);
        }

        [HttpPost]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
        public async ValueTask<IActionResult> Criar([FromBody] UpsertInformacaoAdicionalClienteRequest request)
        {
            if (!Request.Headers.Accept.Any(accept => accept!.Contains(MediaTypeNames.Application.Json)))
            {
                return NotAcceptable();
            }

            return await Dispatch(request);
        }

        [HttpPut("{id}")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
        public async ValueTask<IActionResult> Editar([FromRoute] int id, [FromBody] UpsertInformacaoAdicionalClienteRequest request)
        {
            if (!Request.Headers.Accept.Any(accept => accept!.Contains(MediaTypeNames.Application.Json)))
            {
                return NotAcceptable();
            }

            return await Dispatch(request);
        }

        [HttpPatch("{id}/aprovar")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
        public async ValueTask<IActionResult> Aprovar([FromRoute] int id)
        {
            var request = new AprovarInformacaoAdicionalClienteRequest
            {
                DocumentoSindicalId = id
            };

            if (!Request.Headers.Accept.Any(accept => accept!.Contains(MediaTypeNames.Application.Json)))
            {
                return NotAcceptable();
            }

            return await Dispatch(request);
        }
    }
}
