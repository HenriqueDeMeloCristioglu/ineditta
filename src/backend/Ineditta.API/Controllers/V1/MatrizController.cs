using System.Net.Mime;
using System.Text;

using Ineditta.API.ViewModels.ClientesMatriz.ViewModels;
using Ineditta.API.ViewModels.Matrizes.Requests;
using Ineditta.API.ViewModels.Matrizes.ViewModels;
using Ineditta.API.ViewModels.Shared.ViewModels;
using Ineditta.Application.ClientesMatriz.UseCases.InativarAtivarToggle;
using Ineditta.Application.ClientesMatriz.UseCases.Upsert;
using Ineditta.Application.Usuarios.Entities;
using Ineditta.BuildingBlocks.Core.Auth;
using Ineditta.BuildingBlocks.Core.Domain.ValueObjects;
using Ineditta.BuildingBlocks.Core.Extensions;
using Ineditta.BuildingBlocks.Core.Web.API.Controllers;
using Ineditta.BuildingBlocks.Core.Web.API.Datatables;
using Ineditta.BuildingBlocks.Core.Web.API.Datatables.Medias;
using Ineditta.BuildingBlocks.Core.Web.API.Dtos;
using Ineditta.BuildingBlocks.Core.Web.API.Validators;
using Ineditta.BuildingBlocks.Core.Web.OpenAPI.Filters;
using Ineditta.Repository.Contexts;
using Ineditta.Repository.Extensions;

using MediatR;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using MySqlConnector;

namespace Ineditta.API.Controllers.V1
{
    [Route("v{version:apiVersion}/matrizes")]
    [ApiController]
    public class MatrizController : ApiBaseController
    {
        private readonly InedittaDbContext _context;
        private readonly IUserInfoService _userInfoService;

        public MatrizController(IMediator mediator, RequestStateValidator requestStateValidator, InedittaDbContext context, IUserInfoService userInfoService) : base(mediator, requestStateValidator)
        {
            _context = context;
            _userInfoService = userInfoService;
        }

        [HttpGet("todos")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(IEnumerable<OptionModel<int>>), SelectMediaType.ContentType)]
        public async ValueTask<IActionResult> ObterTodosAsync()
        {

            return Request.Headers.Accept == SelectMediaType.ContentType
                ? Ok(await _context.ClienteMatrizes
                            .AsNoTracking()
                            .Select(clm => new OptionModel<int>
                            {
                                Id = clm.Id,
                                Description = EF.Property<string>(clm, "Cnpj") + " - " + clm.Nome
                            }).ToListAsync())
                    : NoContent();
        }

        [HttpGet]
        [Produces(SelectMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(IEnumerable<OptionModel<int>>), SelectMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(DataTableResponse<MatrizDataTableViewModel>), DatatableMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(IEnumerable<MatrizViewModel>), MediaTypeNames.Application.Json)]
        [SwaggerResponseMimeType(StatusCodes.Status406NotAcceptable, typeof(Envelope), MediaTypeNames.Application.Json, SelectMediaType.ContentType)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async ValueTask<IActionResult> ObterTodos([FromQuery] MatrizRequest request)
        {
            if (Request.Headers.Accept == SelectMediaType.ContentType)
            {
                var result = await (from cmt in _context.ClienteMatrizes
                                    join cgt in _context.GrupoEconomico on cmt.GrupoEconomicoId equals cgt.Id
                                    where !request.GrupoEconomicoId.HasValue || cmt.GrupoEconomicoId == request.GrupoEconomicoId
                                    select new
                                    {
                                        Id = cmt.Id,
                                        cmt.Nome,
                                        Cnpj = EF.Property<string>(cmt, "Cnpj")
                                    }).ToListAsync();

                return !(result?.Any() ?? false)
                    ? NoContent()
                    : Ok(result.Select(cmt => new OptionModel<int> { Id = cmt.Id, Description = cmt.Nome + " | " + CNPJ.Formatar(EF.Property<string>(cmt, "Cnpj")) }));
            }

            if (Request.Headers.Accept == DatatableMediaType.ContentType)
            {
                var result = await (from mt in _context.ClienteMatrizes
                                    join ge in _context.GrupoEconomico on mt.GrupoEconomicoId equals ge.Id into _ge
                                    from ge in _ge.DefaultIfEmpty()
                                    select new MatrizDataTableViewModel
                                    {
                                        Id = mt.Id,
                                        Nome = mt.Nome,
                                        Uf = EF.Property<string>(mt, "Uf"),
                                        Cnpj = EF.Property<string>(mt, "Cnpj") ?? "",
                                        GrupoEconomico = ge.Nome,
                                        DataInclusao = EF.Property<DateTime>(mt, "DataInclusao"),
                                        DataInativacao = mt.DataInativacao,
                                    }).ToDataTableResponseAsync(request);

                if (result == null)
                {
                    return NoContent();
                }

                return Ok(result);
            }

            if (Request.Headers.Accept.Any(header => header!.Contains(MediaTypeNames.Application.Json)))
            {
                var query = _context.ClienteMatrizes.Where(cmt => !request.GrupoEconomicoId.HasValue || cmt.GrupoEconomicoId == request.GrupoEconomicoId)
                    .Select(cmt => new MatrizViewModel { Id = cmt.Id, Cnpj = EF.Property<string>(cmt, "Cnpj"), Codigo = cmt.Codigo ?? "", Nome = cmt.Nome })
                    .AsNoTracking();

                if (request.PorUsuario)
                {
#pragma warning disable S125 // Specify IFormatProvider
                    /*
                    query = _context.ClienteMatrizes.FromSql($@"select * from cliente_matriz cmt
                                                            where exists(select 1 from cliente_unidades cut
		 	                                                                inner join usuario_adm uat on uat.email_usuario = {_userInfoService.GetEmail()} 
			                                                                and case when uat.nivel = {nameof(Nivel.Ineditta)} then true else JSON_CONTAINS(uat.ids_fmge, cast(cut.id_unidade as char)) end
			                                                                and cmt.id_empresa  = cut.cliente_matriz_id_empresa)
                                                                            and ({request.GrupoEconomicoId} = 0 or cmt.cliente_grupo_id_grupo_economico = {request.GrupoEconomicoId})")
                                                    .AsNoTracking()
                                                    .Select(cmt => new MatrizViewModel { Id = cmt.IdEmpresa, Cnpj = cmt.CnpjEmpresa, Codigo = cmt.CodigoEmpresa, Nome = cmt.NomeEmpresa });*/
#pragma warning restore S125 // Specify IFormatProvider

                    var parameters = new List<MySqlParameter>();
                    int parametersCount = 0;

                    var queryMatrizesText = new StringBuilder($@"
                        select cm.* from cliente_matriz cm
                        left join cliente_unidades as cut on cut.cliente_matriz_id_empresa = cm.id_empresa
                        inner join usuario_adm ua on ua.email_usuario = @userEmail
                        where case when ua.nivel = @nivelIneditta then true
		                           when ua.nivel = @nivelGrupoEconomico then cm.cliente_grupo_id_grupo_economico = ua.id_grupoecon
                                   else JSON_CONTAINS(ua.ids_fmge, CONCAT('',cut.id_unidade,''))
                              end
                    ");

                    parameters.Add(new MySqlParameter("@userEmail", _userInfoService.GetEmail()));
                    parameters.Add(new MySqlParameter("@nivelIneditta", nameof(Nivel.Ineditta)));
                    parameters.Add(new MySqlParameter("@nivelGrupoEconomico", Nivel.GrupoEconomico.GetDescription()));

                    if (request.GruposEconomicosIds is not null && request.GruposEconomicosIds.Any())
                    {
                        QueryBuilder.AppendListToQueryBuilder(queryMatrizesText, request.GruposEconomicosIds, "cm.cliente_grupo_id_grupo_economico", parameters, ref parametersCount);
                    }

                    if (!(request.GrupoEconomicoId is null) && request.GrupoEconomicoId > 0)
                    {
                        queryMatrizesText.Append("and cm.cliente_grupo_id_grupo_economico = @grupoEconomicoId");
                        parameters.Add(new MySqlParameter("@grupoEconomicoId", request.GrupoEconomicoId));
                    }

                    queryMatrizesText.Append(@" GROUP BY cm.id_empresa");

                    query = _context.ClienteMatrizes.FromSqlRaw(queryMatrizesText.ToString(), parameters.ToArray())
                                    .AsNoTracking()
                                    .Select(cmt => new MatrizViewModel { Id = cmt.Id, Cnpj = EF.Property<string>(cmt, "Cnpj"), Codigo = cmt.Codigo ?? "", Nome = cmt.Nome });

                }

                return !string.IsNullOrEmpty(request.Columns)
              ? Ok(await query.DynamicSelect(request.Columns!.Split(",").ToList()).ToListAsync())
              : Ok(await query.ToListAsync());
            }

            return NotAcceptable();
        }

        [HttpGet]
        [Route("{id}")]
        [Produces(MediaTypeNames.Application.Json)]
        public async ValueTask<IActionResult> ObterPorId([FromRoute] long id)
        {
            var query = from cm in _context.ClienteMatrizes
                        where cm.Id == id
                        select new ClienteMatrizModulosTipoDocumentoViewModel
                        {
                            Id = cm.Id,
                            Codigo = cm.Codigo,
                            AberturaNegociacao = cm.AberturaNegociacao,
                            DataCorteFopag = cm.DataCorteForpag,
                            DataInativacao = cm.DataInativacao,
                            GrupoEconomicoId = cm.GrupoEconomicoId,
                            TipoProcessamento = cm.TipoProcessamento,
                            Nome = cm.Nome,
                            SlaPrioridade = cm.SlaPrioridade,
                            ModulosIds = _context.ModulosClientes.Where(mc => mc.ClienteMatrizId == cm.Id && mc.DataFim == null).Select(mc => mc.ModuloId).ToList(),
                            TiposDocumentosIds = _context.TiposDocumentosClientesMatriz.Where(tdcm => tdcm.ClienteMatrizId == cm.Id).Select(tdcm => tdcm.TipoDocumentoId).ToList()
                        };

            var resultado = await query.SingleOrDefaultAsync();

            return Ok(resultado);

        }

        [HttpPost]
        public async ValueTask<IActionResult> Inserir([FromForm] UpsertClienteMatrizRequest request)
        {
            return await Dispatch(request);
        }

        [HttpPut]
        public async ValueTask<IActionResult> Atualizar([FromForm] UpsertClienteMatrizRequest request)
        {
            return await Dispatch(request);
        }

        [HttpPatch]
        [Route("{id}")]
        public async ValueTask<IActionResult> InativarAtivarToggle([FromRoute] int id)
        {
            var request = new InativarAtivarClienteMatrizRequest
            {
                Id = id,
            };
            return await Dispatch(request);
        }
    }
}
