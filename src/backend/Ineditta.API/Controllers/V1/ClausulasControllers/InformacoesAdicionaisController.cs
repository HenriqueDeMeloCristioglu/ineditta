using System.Net.Mime;

using Ineditta.BuildingBlocks.Core.Web.API.Controllers;
using Ineditta.BuildingBlocks.Core.Web.API.Validators;
using Ineditta.BuildingBlocks.Core.Web.OpenAPI.Filters;
using Ineditta.Repository.Contexts;

using MediatR;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ineditta.API.ViewModels.InformacoesAdicionais.ViewModels;

namespace Ineditta.API.Controllers.V1
{
    [Route("v{version:apiVersion}/informacoes-adicionais")]
    [ApiController]
    public class InformacoesAdicionaisController : ApiBaseController
    {
        private readonly InedittaDbContext _context;

        public InformacoesAdicionaisController(IMediator mediator, RequestStateValidator requestStateValidator, InedittaDbContext context) : base(mediator, requestStateValidator)
        {
            _context = context;
        }

        [HttpGet("clausula/{id}")]
        [Produces(MediaTypeNames.Application.Json)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(IEnumerable<InformacaoAdicionalPorClausulaViewModel>), MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async ValueTask<IActionResult> ObterInformacoesAdicionaisPorClausula([FromRoute] int id)
        {
            var result = await (from infad in _context.EstruturaClausulasAdTipoinformacaoadicionals
                                join tp_if_ad in _context.AdTipoinformacaoadicionals on infad.AdTipoinformacaoadicionalCdtipoinformacaoadicional equals tp_if_ad.Cdtipoinformacaoadicional into _tp_if_ad
                                from tp_if_ad in _tp_if_ad.DefaultIfEmpty()
                                where infad.EstruturaClausulaIdEstruturaclausula == id
                                select new InformacaoAdicionalPorClausulaViewModel
                                {
                                    Id = tp_if_ad.Cdtipoinformacaoadicional,
                                    NomeTipoInformacao = tp_if_ad.Nmtipoinformacaoadicional,
                                    EstruturaId = infad.EstruturaClausulaIdEstruturaclausula,
                                    TipoInformacaoId = infad.AdTipoinformacaoadicionalCdtipoinformacaoadicional
                                }).ToListAsync();

            if (result == null)
            {
                return NoContent();
            }

            return Ok(result);
        }

        [HttpGet]
        [Produces(MediaTypeNames.Application.Json)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(IEnumerable<InformacaoAdicionalCamposViewModel>), MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async ValueTask<IActionResult> ObjterCamposAdicionais([FromQuery] int tipoId, [FromQuery] int estruturaId)
        {
            var result = await (from infad in _context.EstruturaClausulasAdTipoinformacaoadicionals
                                join tp_if_ad in _context.AdTipoinformacaoadicionals on infad.AdTipoinformacaoadicionalCdtipoinformacaoadicional equals tp_if_ad.Cdtipoinformacaoadicional into _tp_if_ad
                                from tp_if_ad in _tp_if_ad.DefaultIfEmpty()
                                where infad.EstruturaClausulaIdEstruturaclausula == estruturaId && infad.AdTipoinformacaoadicionalCdtipoinformacaoadicional == tipoId
                                select new InformacaoAdicionalCamposViewModel
                                {
                                    Id = infad.IdEstruturaTagsClausulasAdTipoinformacaoadicional,
                                    EstruturaId = infad.EstruturaClausulaIdEstruturaclausula,
                                    CdInformacaoId = infad.AdTipoinformacaoadicionalCdtipoinformacaoadicional,
                                    TipoDadadoId = tp_if_ad.Idtipodado,
                                    TipoInformacaoNome = tp_if_ad.Nmtipoinformacaoadicional
                                }).SingleOrDefaultAsync();

            if (result == null) return NoContent();

            if (result.TipoDadadoId == "G")
            {
                var resultInfoAdiconalGrupo = await (from iag in _context.InformacaoAdicionalGrupos
                                             join at2 in _context.AdTipoinformacaoadicionals on iag.InformacaoadicionalNoGrupo equals at2.Cdtipoinformacaoadicional into _at2
                                             from at2 in _at2.DefaultIfEmpty()
                                             where iag.AdTipoinformacaoadicionalId == tipoId
                                             orderby iag.Sequencia
                                             select new InformacaoAdicionalCamposViewModel
                                             {
                                                Id = iag.AdTipoinformacaoadicionalId,
                                                EstruturaId = estruturaId,
                                                CdInformacaoId = at2.Cdtipoinformacaoadicional,
                                                TipoDadadoId = at2.Idtipodado,
                                                TipoInformacaoNome = at2.Nmtipoinformacaoadicional
                                             }).ToListAsync();

                if (resultInfoAdiconalGrupo == null) return NoContent();

                resultInfoAdiconalGrupo = await PopulateComboOptions(resultInfoAdiconalGrupo);

                return Ok(resultInfoAdiconalGrupo);
            }

            if (result.TipoDadadoId == "C" || result.TipoDadadoId == "CM")
            {
                var resultInformacaoAdicionalCombo = await (from iac in _context.InformacaoAdicionalCombos
                                                            where iac.AdTipoinformacaoadicionalId == tipoId
                                                            select new InformacaoAdicionalCamposViewModel
                                                            {
                                                                Id = result.Id,
                                                                EstruturaId = result.EstruturaId,
                                                                CdInformacaoId = result.CdInformacaoId,
                                                                TipoDadadoId = result.TipoDadadoId,
                                                                TipoInformacaoNome = result.TipoInformacaoNome,
                                                                Combo = new InformacaoAdicionalCombosViewModel
                                                                {
                                                                    Id = iac.IdCombo,
                                                                    Options = iac.Options
                                                                }
                                                            }).ToListAsync();

                if (resultInformacaoAdicionalCombo == null) return NoContent();

                return Ok(resultInformacaoAdicionalCombo);
            }

            return Ok(result);
        }

        [HttpGet("dados/{id}")]
        [Produces(MediaTypeNames.Application.Json)]
        [SwaggerResponseMimeType(StatusCodes.Status200OK, typeof(IEnumerable<InformacaoAdicionalDadoViewModel>), MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async ValueTask<IActionResult> ObjterDadosCamposAdicionais([FromQuery] int grupoId, [FromQuery] int estruturaId, [FromRoute] int id)
        {
#pragma warning disable CA1305 // Specify IFormatProvider
            var result = await (from ifa in _context.InformacaoAdicionalSisap
                                where ifa.InforamcacaoAdicionalGrupoId == grupoId && ifa.EstruturaClausulaId == estruturaId && ifa.ClausulaGeralId == id
                                select new InformacaoAdicionalDadoViewModel
                                {
                                    Id = ifa.Id,
                                    SequenciaLinha = ifa.SequenciaLinha.ToString(),
                                    SequenciaItem = ifa.SequenciaItem,
                                    TipoInfomacaoAdicionalId = ifa.TipoinformacaoadicionalId,
                                    Nome = ifa.NomeInformacaoEstruturaClausulaId,
                                    Combo = ifa.Combo,
                                    Data = ifa.Data,
                                    Descricao = ifa.Descricao,
                                    Hora = ifa.Hora,
                                    Numerico = ifa.Numerico,
                                    Percentual = ifa.Percentual,
                                    Texto = ifa.Texto,
                                    GrupoId = ifa.InforamcacaoAdicionalGrupoId
                                }).ToListAsync();
#pragma warning restore CA1305 // Specify IFormatProvider

            if (result == null)
            {
                return NoContent();
            }

            return Ok(result);
        }

        private async ValueTask<List<InformacaoAdicionalCamposViewModel>> PopulateComboOptions(List<InformacaoAdicionalCamposViewModel> result)
        {
            foreach (var campo in result)
            {
                if (campo.TipoDadadoId == "C" || campo.TipoDadadoId == "CM")
                {
                    campo.Combo = await (from iac in _context.InformacaoAdicionalCombos
                                        where iac.AdTipoinformacaoadicionalId == campo.CdInformacaoId
                                         select new InformacaoAdicionalCombosViewModel
                                        {
                                            Id = iac.IdCombo,
                                            Options = iac.Options
                                        })
                                         .SingleOrDefaultAsync();
                }
            }

            return result;
        }
    }
}
