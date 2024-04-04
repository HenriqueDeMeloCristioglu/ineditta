using Ineditta.API.ViewModels.EstruturaClausulas.Requests;
using Ineditta.API.ViewModels.EstruturaClausulas.ViewModels;
using Ineditta.API.ViewModels.Shared.ViewModels;
using Ineditta.BuildingBlocks.Core.Web.API.Controllers;
using Ineditta.BuildingBlocks.Core.Web.API.Datatables;
using Ineditta.BuildingBlocks.Core.Web.API.Datatables.Medias;
using Ineditta.BuildingBlocks.Core.Web.API.Validators;
using Ineditta.BuildingBlocks.Core.Web.OpenAPI.Filters;
using Ineditta.Repository.Contexts;
using Ineditta.Repository.Extensions;

using MediatR;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using MySqlConnector;

using System.Net.Mime;
using System.Text;

namespace Ineditta.API.Controllers.V1
{
    [Route("v{version:apiVersion}/estrutura-clausulas")]
    [ApiController]
    public class EstruturaClausulaController : ApiBaseController
    {
        private readonly InedittaDbContext _context;

        public EstruturaClausulaController(IMediator mediator, RequestStateValidator requestStateValidator, InedittaDbContext context) : base(mediator, requestStateValidator)
        {
            _context = context;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [Produces(DatatableMediaType.ContentType, MediaTypeNames.Application.Json, SelectMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(IEnumerable<OptionModel<int>>), SelectMediaType.ContentType)]
        public async ValueTask<IActionResult> ObterTodosAsync([FromQuery] DataTableRequest request)
        {
            if (Request.Headers.Accept == SelectMediaType.ContentType)
            {
                var result = await _context.EstruturaClausula
                            .AsNoTracking()
                            .Select(ec => new OptionModel<int>
                            {
                                Id = ec.Id,
                                Description = ec.Nome
                            }).ToListAsync();

                return Ok(result);
            } else if (Request.Headers.Accept == DatatableMediaType.ContentType)
            {
                var result = await _context.EstruturaClausula
                            .AsNoTracking()
                            .Select(ec => new EstruturaClausulaDataTableViewModel
                            {
                                Id = ec.Id,
                                Nome = ec.Nome,
                                Tipo = ec.Tipo.ToString(),
                                Classe = ec.Classe ? "S" : "N"
                            })
                            .ToDataTableResponseAsync(request);
                return Ok(result);
            } else
            {
                return NoContent();
            }
        }

        [HttpGet]
        [Route("por-grupo")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [Produces(DatatableMediaType.ContentType, MediaTypeNames.Application.Json, SelectMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(IEnumerable<OptionModel<int>>), SelectMediaType.ContentType)]
        public async ValueTask<IActionResult> ObterTodosPorClausulaAsync([FromQuery] ObterTodosPorGrupoEconomicoRequest request)
        {
            if (Request.Headers.Accept == SelectMediaType.ContentType)
            {
                var parameters = new List<MySqlParameter>();
                int parametersCount = 0;

                var query = new StringBuilder(@"SELECT DISTINCT ec.* from clausula_geral cg
                                                inner join estrutura_clausula ec on ec.id_estruturaclausula = cg.estrutura_id_estruturaclausula
                                                where true");

                if (request.GrupoClausulaId != null && request.GrupoClausulaId.Any())
                {
                    QueryBuilder.AppendListToQueryBuilder(query, request.GrupoClausulaId, "ec.grupo_clausula_idgrupo_clausula", parameters, ref parametersCount);
                }

                if (request.Calendario)
                {
                    query.Append(@" and ec.calendario = 'S'");
                }

                if (!request.ClausulaNaoConsta)
                {
                    query.Append(" and cg.consta_no_documento = 1 ");
                }

                if (request.ClausulaResumivel)
                {
                    query.Append(" and ec.resumivel = 1 ");
                }

                var result = await _context.EstruturaClausula
                                .FromSqlRaw(query.ToString(), parameters.ToArray())
                                .AsNoTracking()
                                .Select(ec => new OptionModel<int>
                                {
                                    Id = ec.Id,
                                    Description = ec.Nome
                                }).ToListAsync();

                return Ok(result);
            }
            else
            {
                return NoContent();
            }
        }

        [HttpGet]
        [Route("grupos")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [Produces(DatatableMediaType.ContentType, MediaTypeNames.Application.Json, SelectMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(IEnumerable<OptionModel<int>>), SelectMediaType.ContentType)]
        public async ValueTask<IActionResult> ObterGruposClausulaAsync([FromQuery] bool calendario = false)
        {
            if (Request.Headers.Accept == SelectMediaType.ContentType)
            {
                var parameters = new List<MySqlParameter>();

                var query = new StringBuilder(@"
                    SELECT * FROM grupo_clausula gc
                    WHERE TRUE

	                      and EXISTS (
	  	                    SELECT 1 FROM estrutura_clausula ec 
	  	                    WHERE ec.grupo_clausula_idgrupo_clausula = gc.idgrupo_clausula
                ");

                if (calendario)
                {
                    query.Append(@" and ec.calendario = 'S'");
                }

                query.Append(@" ) ");

                var result = await _context.GrupoClausula
                                .FromSqlRaw(query.ToString(), parameters.ToArray())
                                .AsNoTracking()
                                .Select(ec => new OptionModel<int>
                                {
                                    Id = ec.Id,
                                    Description = ec.Nome
                                }).ToListAsync();

                return Ok(result);
            }
            else
            {
                return NoContent();
            }
        }
    }
}
