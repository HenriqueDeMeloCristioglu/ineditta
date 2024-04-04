using System.Net.Mime;

using Ineditta.API.ViewModels.DiretoriasEmpregados.ViewModels;
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

namespace Ineditta.API.Controllers.V1.Diretorias
{
    [Route("v{version:apiVersion}/diretorias-empregados")]
    [ApiController]
    public class DiretoriaEmpregadoController : ApiBaseController
    {
        private readonly InedittaDbContext _context;

        public DiretoriaEmpregadoController(IMediator mediator, RequestStateValidator requestStateValidator, InedittaDbContext context) : base(mediator, requestStateValidator)
        {
            _context = context;
        }

        [HttpGet("{id:int}")]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(DiretoriaEmpregadoViewModel), MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json)]
        public async ValueTask<IActionResult> ObterPorIdAsync([FromRoute] int id)
        {
            var result = await (from sdet in _context.SindDiremps
                                join set in _context.SindEmps on sdet.SindEmpIdSinde equals set.Id
                                join cut in _context.ClienteUnidades on sdet.ClienteUnidadesIdUnidade equals cut.Id into _cut
                                from cut in _cut.DefaultIfEmpty()
                                select new DiretoriaEmpregadoViewModel
                                {
                                    Id = sdet.IdDiretoriae,
                                    Nome = sdet.DirigenteE,
                                    InicioMandato = sdet.InicioMandatoe,
                                    TerminoMandato = sdet.TerminoMandatoe,
                                    Funcao = sdet.FuncaoE,
                                    Situacao = sdet.SituacaoE,
                                    SindicatoDirigenteId = sdet.SindEmpIdSinde,
                                    SindicatoDirigenteSigla = set.Sigla,
                                    EmpresaId = sdet.ClienteUnidadesIdUnidade,
                                    EmpresaFilial = cut == null ? default : cut.Nome

                                }).SingleOrDefaultAsync(devw => devw.Id == id);

            return result == null ?
                NotFound() :
                Ok(result);
        }

        [HttpGet]
        [Produces(DatatableMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(DataTableResponse<DiretoriaEmpregadoViewModel>), DatatableMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status400BadRequest, typeof(Envelope), MediaTypeNames.Application.Json, DatatableMediaType.ContentType)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async ValueTask<IActionResult> ObterTodosAsync([FromQuery] DataTableRequest request)
        {
            if (Request.Headers.Accept == DatatableMediaType.ContentType)
            {
                var result = await (from sdt in _context.SindDiremps
                                    join set in _context.SindEmps on sdt.SindEmpIdSinde equals set.Id
                                    join cut in _context.ClienteUnidades on sdt.ClienteUnidadesIdUnidade equals cut.Id into _cut
                                    from cut in _cut.DefaultIfEmpty()
                                    select new DiretoriaEmpregadoViewModel
                                    {
                                        Id = sdt.IdDiretoriae,
                                        Nome = sdt.DirigenteE,
                                        InicioMandato = sdt.InicioMandatoe,
                                        TerminoMandato = sdt.TerminoMandatoe,
                                        Funcao = sdt.FuncaoE,
                                        Situacao = sdt.SituacaoE,
                                        NomeUnidade = cut == null ? default : cut.Nome,
                                        Sigla = set.Sigla
                                    })
                                    .ToDataTableResponseAsync(request);

                return Ok(result);
            }

            return NoContent();
        }
    }
}
