using System.Net.Mime;

using Ineditta.API.ViewModels.DiretoriasPatronais.ViewModels;
using Ineditta.Application.DirigentesPatronais.UseCases;
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
    [Route("v{version:apiVersion}/diretorias-patronais")]
    [ApiController]
    public class DiretoriaPatronaisController : ApiBaseController
    {
        private readonly InedittaDbContext _context;

        public DiretoriaPatronaisController(IMediator mediator, RequestStateValidator requestStateValidator, InedittaDbContext context) : base(mediator, requestStateValidator)
        {
            _context = context;
        }

        [HttpGet("{id:int}")]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(DiretoriaPatronaisViewModel), MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json)]
        public async ValueTask<IActionResult> ObterPorIdAsync([FromRoute] int id)
        {
            var result = await (from sdet in _context.SindDirpatros
                                join set in _context.SindPatrs on sdet.SindicatoPatronalId equals set.Id
                                join cut in _context.ClienteUnidades on sdet.EstabelecimentoId equals cut.Id into _cut
                                from cut in _cut.DefaultIfEmpty()
                                select new DiretoriaPatronaisViewModel
                                {
                                    Id = sdet.Id,
                                    Nome = sdet.Nome,
                                    InicioMandato = sdet.DataInicioMandato,
                                    TerminoMandato = sdet.DataFimMandato,
                                    Funcao = sdet.Funcao,
                                    Situacao = sdet.Situacao,
                                    SindicatoDirigenteId = sdet.SindicatoPatronalId,
                                    SindicatoDirigenteSigla = set.Sigla,
                                    EmpresaId = sdet.EstabelecimentoId,
                                    EmpresaFilial = cut == null ? default : cut.Nome

                                }).SingleOrDefaultAsync(devw => devw.Id == id);

            return result == null ?
                NotFound() :
                Ok(result);
        }

        [HttpGet]
        [Produces(DatatableMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(DataTableResponse<DiretoriaPatronaisViewModel>), DatatableMediaType.ContentType)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async ValueTask<IActionResult> ObterTodosAsync([FromQuery] DataTableRequest request)
        {
            if (Request.Headers.Accept == DatatableMediaType.ContentType)
            {
                var result = await (from sdt in _context.SindDirpatros
                                    join set in _context.SindPatrs on sdt.SindicatoPatronalId equals set.Id
                                    join cut in _context.ClienteUnidades on sdt.EstabelecimentoId equals cut.Id into _cut
                                    from cut in _cut.DefaultIfEmpty()
                                    select new DiretoriaPatronaisViewModel
                                    {
                                        Id = sdt.Id,
                                        Nome = sdt.Nome,
                                        InicioMandato = sdt.DataInicioMandato,
                                        TerminoMandato = sdt.DataFimMandato,
                                        Funcao = sdt.Funcao,
                                        Situacao = sdt.Situacao,
                                        NomeUnidade = cut == null ? default : cut.Nome,
                                        Sigla = set.Sigla
                                    })
                                    .ToDataTableResponseAsync(request);

                return Ok(result);
            }

            return NoContent();
        }

        [HttpPost]
        [Produces(MediaTypeNames.Application.Json)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(Envelope), MediaTypeNames.Application.Json)]
        [SwaggerResponseMimeType(StatusCodes.Status400BadRequest, typeof(Envelope), MediaTypeNames.Application.Json)]
        public async ValueTask<IActionResult> Criar(UpsertDirigentePatronalRequest request)
        {
            return await Dispatch(request);
        }
    }
}
