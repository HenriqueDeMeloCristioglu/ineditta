using Ineditta.BuildingBlocks.Core.Web.API.Controllers;
using Ineditta.BuildingBlocks.Core.Web.API.Datatables.Medias;
using Ineditta.BuildingBlocks.Core.Web.API.Validators;
using Ineditta.BuildingBlocks.Core.Web.OpenAPI.Filters;
using Ineditta.Repository.Contexts;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ineditta.BuildingBlocks.Core.Web.API.Datatables;
using System.Net.Mime;
using Ineditta.BuildingBlocks.Core.Web.API.Dtos;
using Microsoft.EntityFrameworkCore;
using Ineditta.API.ViewModels.IAs.Clausulas;
using Ineditta.Application.AIs.DocumentosSindicais.UseCases.Aprovar;
using Ineditta.Application.AIs.DocumentosSindicais.Services.ClassificacaoClausula;
using Ineditta.Application.AIs.DocumentosSindicais.Services.QuebraClausula;
using Ineditta.API.ViewModels.DiretoriasPatronais.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Ineditta.Application.AIs.DocumentosSindicais.Entities;
using Ineditta.Application.AIs.Clausulas.Entities;
using Ineditta.API.ViewModels.Shared.ViewModels;
using Ineditta.BuildingBlocks.Core.Extensions;
using Ineditta.API.ViewModels.IAs.DocumentosSindicais.ViewModels;
using Ineditta.API.ViewModels.IAs.DocumentosSindicais.Requests;
using Ineditta.API.ViewModels.DocumentosSindicais.ViewModels;
using Ineditta.Application.Usuarios.Entities;
using MySqlConnector;
using System.Text;
using Ineditta.Repository.Extensions;
using Ineditta.Application.AIs.DocumentosSindicais.UseCases.Reprocessar;

namespace Ineditta.API.Controllers.V1.IAControllers
{
    [Route("v{version:apiVersion}/ia-documentos-sindicais")]
    [ApiController]
    public class IADocumentoSindicalController : ApiBaseController
    {
        private readonly InedittaDbContext _context;
        private readonly IQuebraClausulaService _automacacaoQuebraClausulaService;
#pragma warning disable S4487 // Unread "private" fields should be removed
        private readonly IClassificacaoClausulaService _classificacaoClausulaService;
#pragma warning restore S4487 // Unread "private" fields should be removed
        public IADocumentoSindicalController(IMediator mediator,
                                             RequestStateValidator requestStateValidator,
                                             InedittaDbContext context,
                                             IQuebraClausulaService automacacaoQuebraClausulaService,
                                             IClassificacaoClausulaService classificacaoClausulaService) : base(mediator, requestStateValidator)
        {
            _context = context;
            _automacacaoQuebraClausulaService = automacacaoQuebraClausulaService;
            _classificacaoClausulaService = classificacaoClausulaService;
        }

        [HttpGet]
        [Produces(DatatableMediaType.ContentType)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(DataTableResponse<IADocumentoSindicalDatatableViewModel>), DatatableMediaType.ContentType)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
        public async ValueTask<IActionResult> ObterTodosAsync([FromQuery] IADocumentoSindicalDataTableRequest request)
        {
            if (Request.Headers.Accept == DatatableMediaType.ContentType)
            {
                var parameters = new List<MySqlParameter>();
                int sqlParametersCount = 0;

                var query = new StringBuilder(@"SELECT idsv.* from ia_documento_sindical_vw idsv
                inner join  doc_sind ds on idsv.documento_referencia_id = ds.id_doc 
                inner join tipo_doc td on ds.tipo_doc_idtipo_doc = td.idtipo_doc
                where exists (
	                select * from documento_estabelecimento_tb det
                    inner join cliente_unidades cu on det.estabelecimento_id = cu.id_unidade
                    where det.documento_id = ds.id_doc ");

                if (request.GruposEconomicosIds != null && request.GruposEconomicosIds.Any())
                {
                    QueryBuilder.AppendListToQueryBuilder(query, request.GruposEconomicosIds, "cu.cliente_grupo_id_grupo_economico", parameters, ref sqlParametersCount);
                }

                if (request.EmpresasIds != null && request.EmpresasIds.Any())
                {
                    QueryBuilder.AppendListToQueryBuilder(query, request.EmpresasIds, "cu.cliente_matriz_id_empresa", parameters, ref sqlParametersCount);
                }

                query.Append(" ) ");

                query.Append(@" and exists (
	                SELECT * from documento_atividade_economica_tb daet
	                inner join classe_cnae cc on daet.atividade_economica_id = cc.id_cnae
	                where daet.documento_id = ds.id_doc");

                if (request.AtividadesEconomicasIds != null)
                {
                    QueryBuilder.AppendListToQueryBuilder(query, request.AtividadesEconomicasIds, "cc.id_cnae", parameters, ref sqlParametersCount);
                }

#pragma warning disable CA1305 // Specify IFormatProvider
                if (request.GrupoOperacaoId != null)
                {
                    query.Append($" and cc.divisao_cnae = {request.GrupoOperacaoId} ");
                }

                query.Append(" ) ");

                if (request.DocumentoId is not null)
                {
                    query.Append($" and idsv.id = {request.DocumentoId} ");
                }

                if (request.NomeDocumentoId is not null)
                {
                    query.Append($" and td.idtipo_doc = {request.NomeDocumentoId} ");
                }

                if (request.StatusDocumentoId is not null)
                {
                    query.Append($" and idsv.status = {request.StatusDocumentoId} ");
                }

                if (request.StatusClausulasId is not null)
                {
                    query.Append($@"
                        and exists (
	                        SELECT * from ia_clausula_tb ict 
	                        where ict.ia_documento_sindical_id = ds.id_doc
	                        and ict.status = {request.StatusClausulasId}
                        ) ");
                }

                if (request.DataSlaFim.HasValue && request.DataSlaInicio.HasValue)
                {
                    query.Append($" and ds.data_sla >= '{request.DataSlaInicio.Value.ToString("yyyy-MM-dd")}' and ds.data_sla <= '{request.DataSlaFim.Value.ToString("yyyy-MM-dd")}' ");
                }
#pragma warning restore CA1305 // Specify IFormatProvider

                var result = await _context.IADocumentoSindicalVw.FromSqlRaw(query.ToString(), parameters.ToArray())
                    .Select(e => e)
                    .ToDataTableResponseAsync(request);

                if (result is null)
                {
                    return NoContent();
                }

                return Ok(result);
            }

            if (Request.Headers.Accept == SelectMediaType.ContentType)
            {
                List<OptionModel<int>> opcoes = new()
                {
                    new OptionModel<int>
                    {
                        Id = (int)IADocumentoStatus.AguardandoProcessamento,
                        Description = IADocumentoStatus.AguardandoProcessamento.GetDescription()
                    },
                    new OptionModel<int>
                    {
                        Id = (int)IADocumentoStatus.QuebrandoClausulas,
                        Description = IADocumentoStatus.QuebrandoClausulas.GetDescription()
                    },
                    new OptionModel<int>
                    {
                        Id = (int)IADocumentoStatus.AguardandoAprovacaoQuebraClausula,
                        Description = IADocumentoStatus.AguardandoAprovacaoQuebraClausula.GetDescription()
                    },
                    new OptionModel<int>
                    {
                        Id = (int)IADocumentoStatus.ClassificandoClausulas,
                        Description = IADocumentoStatus.ClassificandoClausulas.GetDescription()
                    },
                    new OptionModel<int>
                    {
                        Id = (int)IADocumentoStatus.AguardandoAprovacaoClassificacao,
                        Description = IADocumentoStatus.AguardandoAprovacaoClassificacao.GetDescription()
                    },
                    new OptionModel<int>
                    {
                        Id = (int)IADocumentoStatus.Aprovado,
                        Description = IADocumentoStatus.Aprovado.GetDescription()
                    },
                    new OptionModel<int>
                    {
                        Id = (int)IADocumentoStatus.Erro,
                        Description = IADocumentoStatus.Erro.GetDescription()
                    }
                };

                return Ok(opcoes);
            }

            return NotAcceptable();
        }

        [HttpGet("{id}")]
        [Produces(MediaTypeNames.Application.Json)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(IADocumentoSindicalViewModel), MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
        public async ValueTask<IActionResult> ObterPorIdAsync([FromRoute] int id)
        {
            if (!Request.Headers.Accept.Any(accept => accept!.Contains(MediaTypeNames.Application.Json)))
            {
                return NotAcceptable();
            }

            var result = await (from iads in _context.IADocumentoSindical
                                join ds in _context.DocSinds on iads.DocumentoReferenciaId equals ds.Id
                                join td in _context.TipoDocs on ds.TipoDocumentoId equals td.Id
                                select new IADocumentoSindicalViewModel
                                {
                                    Id = iads.Id,
                                    DocumentoReferenciaId = iads.DocumentoReferenciaId,
                                    Nome = td.Nome,
                                    VigenciaInicial = ds.DataValidadeInicial,
                                    VigenciaFinal = ds.DataValidadeFinal,
                                    Clausulas = (from iac in _context.IAClausulas
                                                 join ec in _context.EstruturaClausula on iac.EstruturaClausulaId equals ec.Id into _ec
                                                 from ec in _ec.DefaultIfEmpty()
                                                 join sn in _context.Sinonimos on iac.SinonimoId equals sn.Id into _sn
                                                 from sn in _sn.DefaultIfEmpty()
                                                 join ass in _context.Assuntos on sn.AssuntoId equals ass.Idassunto into _ass
                                                 from ass in _ass.DefaultIfEmpty()
                                                 where iac.IADocumentoSindicalId == iads.Id
                                                 select new IAClausulaViewModel
                                                 {
                                                     Id = iac.Id,
                                                     EstruturaClausulaId = iac.EstruturaClausulaId,
                                                     EstruturaClausulaNome = ec.Nome,
                                                     DocumentoSindicalId = iac.IADocumentoSindicalId,
                                                     DocumentoSindicalNome = td.Nome,
                                                     Numero = iac.Numero,
                                                     SinonimoId = iac.SinonimoId,
                                                     SinonimoNome = sn.Nome ?? string.Empty,
                                                     Status = iac.Status,
                                                     Texto = iac.Texto,
                                                     Assunto = ass.Assunto1
                                                 })
                                                    .AsSplitQuery()
                                                    .ToList()
                                })
                                .SingleOrDefaultAsync(d => d.Id == id);

            if (result == null)
            {
                return NoContent();
            }

            return Ok(result);
        }

        [HttpPatch("aprovar/{id}")]
        [Produces(MediaTypeNames.Application.Json)]
        [SwaggerResponseMimeType(StatusCodes.Status201Created, typeof(Envelope), MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
        public async ValueTask<IActionResult> IncluirAsync([FromRoute] int id)
        {
            if (!Request.Headers.Accept.Any(accept => accept!.Contains(MediaTypeNames.Application.Json)))
            {
                return NotAcceptable();
            }

            var request = new AprovarIADocumentoSindicalRequest
            {
                DocumentoId = id
            };

            return await Dispatch(request);
        }

        [HttpPatch("reprocessar/{id}")]
        [Produces(MediaTypeNames.Application.Json)]
        [SwaggerResponseMimeType(StatusCodes.Status201Created, typeof(Envelope), MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
        public async ValueTask<IActionResult> ReprocessarAsync([FromRoute] int id)
        {
            if (!Request.Headers.Accept.Any(accept => accept!.Contains(MediaTypeNames.Application.Json)))
            {
                return NotAcceptable();
            }

            var request = new ReprocessarIADocumentoSindicalRequest
            {
                DocumentoId = id
            };

            return await Dispatch(request);
        }

        [HttpGet("teste/{documentoSindicalId:int}")]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(DiretoriaPatronaisViewModel), MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json)]
        [AllowAnonymous]
        public async ValueTask<IActionResult> ConverterDocumentoMTEAsync([FromRoute] int documentoSindicalId)
        {
            var result = await _automacacaoQuebraClausulaService.QuebrarContratoEmClausulas(documentoSindicalId);

            if (result.IsFailure)
            {
                return NotFound(result.Error);
            }

#pragma warning disable S125 // Sections of code should not be commented out
            //var clausulasClassificadas = await _classificacaoClausulaService.ClassificarClausulas(result.Value.Clausulas);
            //var clausulasClassificadas = await _classificacaoClausulaService.ClassificarClausula(result.Value.Clausulas.ElementAt(0));
#pragma warning restore S125 // Sections of code should not be commented out

            return result.IsFailure ?
                NotFound(result.Error) :
                Ok(result.Value);
        }
    }
}
