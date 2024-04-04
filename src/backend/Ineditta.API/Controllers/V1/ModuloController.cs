using System.Net.Mime;

using Ineditta.BuildingBlocks.Core.Web.API.Controllers;
using Ineditta.BuildingBlocks.Core.Web.API.Datatables.Medias;
using Ineditta.BuildingBlocks.Core.Web.API.Datatables;
using Ineditta.BuildingBlocks.Core.Web.API.Dtos;
using Ineditta.BuildingBlocks.Core.Web.API.Validators;
using Ineditta.BuildingBlocks.Core.Web.OpenAPI.Filters;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ineditta.Repository.Contexts;
using Microsoft.EntityFrameworkCore;
using Ineditta.Application.Usuarios.Entities;
using System.ComponentModel;
using System.Reflection;
using Ineditta.Application.Modulos.Entities;
using Ineditta.API.ViewModels.Modulos.Requests;
using Ineditta.API.ViewModels.Modulos.ViewModels;

namespace Ineditta.API.Controllers.V1
{
    [Route("v{version:apiVersion}/modulos")]
    [ApiController]
    public class ModuloController : ApiBaseController
    {
        private readonly InedittaDbContext _context;
        
        public ModuloController(IMediator mediator, RequestStateValidator requestStateValidator, InedittaDbContext context) : base(mediator, requestStateValidator)
        {
            _context = context;
        }

        [HttpGet]
        [Produces(DatatableMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(DataTableResponse<ModuloViewModel>), DatatableMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(IEnumerable<ModuloViewModel>), MediaTypeNames.Application.Json)]
        [SwaggerResponseMimeType(StatusCodes.Status406NotAcceptable, typeof(Envelope), MediaTypeNames.Application.Json, DatatableMediaType.ContentType)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async ValueTask<IActionResult> ObterAsync([FromQuery] ModuloDatatableRequest request)
        {
            var requestTipo = GetTipoModulo(request.Tipo);

            if (Request.Headers.Accept == DatatableMediaType.ContentType)
            {
#pragma warning disable S1125 // Boolean literals should not be redundant
                return Ok(await _context.Modulos
                            .AsNoTracking()
                            .Where(cct => cct.Tipo == requestTipo)
                            .Select(mt => new ModuloViewModel
                            {
                                Id = mt.Id,
                                Nome = mt.Nome,
                                TipoId = mt.Tipo,
                                Alterar = mt.PermiteAlterar,
                                Aprovar = mt.PermiteAprovar,
                                Comentar = mt.PermiteComentar,
                                Consultar = mt.PermiteConsultar,
                                Criar = mt.PermiteCriar,
                                Excluir = mt.PermiteExcluir,
                                Uri = mt.Uri
                            })
                            .ToDataTableResponseAsync(request));
#pragma warning restore S1125 // Boolean literals should not be redundant
            }

            if (Request.Headers.Accept != MediaTypeNames.Application.Json)
            {
                return NotAcceptable();
            }

            return Ok(await _context.Modulos
                .AsNoTracking()
                .Where(mt => mt.Tipo == requestTipo)
                .Select(mt => new ModuloViewModel
                {
                    Id = mt.Id,
                    Nome = mt.Nome,
                    TipoId = mt.Tipo,
                    Alterar = mt.PermiteAlterar,
                    Aprovar = mt.PermiteAprovar,
                    Comentar = mt.PermiteComentar,
                    Consultar = mt.PermiteConsultar,
                    Criar = mt.PermiteCriar,
                    Excluir = mt.PermiteExcluir,
                    Uri = mt.Uri
                })
                .ToListAsync());
        }

        public static TipoModulo GetTipoModulo(string? value)
        {
            if (value == null)
            {
                return TipoModulo.Comercial;
            }

            foreach (FieldInfo field in typeof(TipoModulo).GetFields())
            {
                if (Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
                {
                    if (attribute.Description?.ToLowerInvariant() == value.ToLowerInvariant())
#pragma warning disable CS8605 // Unboxing a possibly null value.
                        return (TipoModulo)field.GetValue(null)!;
                }
                else
                {
                    if (field.Name.ToLowerInvariant() == value.ToLowerInvariant())
                        return (TipoModulo)field.GetValue(null)!;
                }
#pragma warning restore CS8605 // Unboxing a possibly null value.
            }

            throw new ArgumentException($"No {typeof(Nivel)} with description {value} found");
        }
    }
}
