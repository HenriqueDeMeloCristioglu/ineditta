using System.Net.Mime;

using Ineditta.API.Filters;
using Ineditta.API.Services;
using Ineditta.API.ViewModels.GruposEconomicos.Requests;
using Ineditta.API.ViewModels.GruposEconomicos.ViewModels;
using Ineditta.API.ViewModels.Shared.ViewModels;
using Ineditta.Application.Usuarios.Entities;
using Ineditta.BuildingBlocks.Core.Auth;
using Ineditta.BuildingBlocks.Core.Extensions;
using Ineditta.BuildingBlocks.Core.Web.API.Controllers;
using Ineditta.BuildingBlocks.Core.Web.API.Datatables;
using Ineditta.BuildingBlocks.Core.Web.API.Datatables.Medias;
using Ineditta.BuildingBlocks.Core.Web.API.Dtos;
using Ineditta.BuildingBlocks.Core.Web.API.Validators;
using Ineditta.BuildingBlocks.Core.Web.OpenAPI.Filters;
using Ineditta.Repository.Contexts;

using MediatR;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ineditta.API.Controllers.V1
{

    [Route("v{version:apiVersion}/grupos-economicos")]
    [ApiController]
    public class GrupoEconomicoController : ApiBaseController
    {
        private readonly InedittaDbContext _context;
        private readonly HttpRequestItemService _httpRequestItemService;
        private readonly IUserInfoService _userInfoService;

        public GrupoEconomicoController(IMediator mediator, RequestStateValidator requestStateValidator, InedittaDbContext context, IUserInfoService userInfoService, HttpRequestItemService httpRequestItemService) : base(mediator, requestStateValidator)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _userInfoService = userInfoService;
            _httpRequestItemService = httpRequestItemService;
        }

        [HttpGet]
        [Produces(SelectMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(IEnumerable<OptionModel<int>>), SelectMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(DataTableResponse<GrupoEconomicoDataTableViewModel>), DatatableMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(IEnumerable<GrupoEconomicoViewModel>), MediaTypeNames.Application.Json)]
        [SwaggerResponseMimeType(StatusCodes.Status400BadRequest, typeof(Envelope), SelectMediaType.ContentType)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ServiceFilter(typeof(UserInfoActionFilter))]
        public async ValueTask<IActionResult> ObterTodos([FromQuery] GrupoEconomicoRequest request)
        {
            if (Request.Headers.Accept == SelectMediaType.ContentType)
            {
                if (request.PorUsuario != null && request.PorUsuario == true)
                {
                    var usuario = _httpRequestItemService.ObterUsuario();

                    if(usuario == null)
                    {
                        return NoContent();
                    }

                    return Ok(await (from cgt in _context.GrupoEconomico
                                     where cgt.Id == usuario!.GrupoEconomicoId
                                     select new OptionModel<int>
                                     {
                                         Id = cgt.Id,
                                         Description = cgt.Nome
                                     })
                            .ToListAsync());
                }

                return Ok(await (from cgt in _context.GrupoEconomico
                                 select new OptionModel<int>
                                 {
                                     Id = cgt.Id,
                                     Description = cgt.Nome
                                 })
                            .ToListAsync());
            }

            if (Request.Headers.Accept == DatatableMediaType.ContentType)
            {
                var result = await (from ge in _context.GrupoEconomico
                                    select new GrupoEconomicoDataTableViewModel
                                    {
                                        Id = ge.Id,
                                        Nome = ge.Nome
                                    }).ToDataTableResponseAsync(request);

                if (result == null)
                {
                    return NoContent();
                }

                return Ok(result);
            }

            if (Request.Headers.Accept.Any(header => header!.Contains(MediaTypeNames.Application.Json)))
            {
                var query = _context.GrupoEconomico.AsNoTracking().Select(cgt => new GrupoEconomicoViewModel { Id = cgt.Id, Nome = cgt.Nome });

                if (request.PorUsuario != null && request.PorUsuario == true)
                {   
                    query = _context.GrupoEconomico.FromSql($@"select grupo_economico.* FROM cliente_grupo grupo_economico
                                                                join usuario_adm adm on adm.email_usuario = {_userInfoService.GetEmail()}
                                                                WHERE CASE WHEN adm.nivel = {nameof(Nivel.Ineditta)} then true 
                                                                ELSE grupo_economico.id_grupo_economico = adm.id_grupoecon END")
                        .AsNoTracking()
                        .Select(cgt => new GrupoEconomicoViewModel { Id = cgt.Id, Nome = cgt.Nome });
                }

                return !string.IsNullOrEmpty(request.Columns)
               ? Ok(await query.DynamicSelect(request.Columns!.Split(",").ToList()).ToListAsync())
               : Ok(await query.ToListAsync());
            }

            return NotAcceptable();
        }
    }
}
