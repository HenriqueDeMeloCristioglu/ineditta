using ClosedXML.Excel;

using CSharpFunctionalExtensions;

using Ineditta.API.Builders.Worksheets;
using Ineditta.API.Configurations;
using Ineditta.API.ViewModels.ClausulasGerais.ViewModels.Excel;
using Ineditta.BuildingBlocks.Core.Domain.Models;
using Ineditta.BuildingBlocks.Core.Domain.ValueObjects;
using Ineditta.Repository.Contexts;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Ineditta.API.Builders.Clausulas
{
    public class RelatorioClausulaBuilder
    {
        private readonly InedittaDbContext _context;
        private readonly TemplateExcelConfiguration _templateExcelConfiguration;

        public RelatorioClausulaBuilder(InedittaDbContext context, IOptions<TemplateExcelConfiguration> templateExcelConfiguration)
        {
            _context = context;
            _templateExcelConfiguration = templateExcelConfiguration?.Value ?? throw new ArgumentNullException(nameof(templateExcelConfiguration));
        }

        public async ValueTask<Result<byte[], Error>> HandleAsync(int documentoId)
        {
            var clausulas = await ObterTodos(documentoId);

            if (clausulas == null)
            {
                return Result.Failure<byte[], Error>(Errors.Http.NotFound());
            }

            using var workbook = WorksheetBuilder.ReadFrom(_templateExcelConfiguration.RelatorioClausulas);

            var bytes = workbook.GetFirst()
                            .Build(ws =>
                            {
                                var currentLine = 2;

                                foreach (var clausula in clausulas)
                                {
                                    ws.Cell(currentLine, 1).SetValue(clausula.IdDocumento);
                                    ws.Cell(currentLine, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                                    ws.Cell(currentLine, 1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                                    ws.Cell(currentLine, 1).Style.Font.FontColor = XLColor.Black;

                                    ws.Cell(currentLine, 2).SetValue(clausula.NomeDocumento);
                                    ws.Cell(currentLine, 2).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                                    ws.Cell(currentLine, 2).Style.Font.FontColor = XLColor.Black;

                                    ws.Cell(currentLine, 3).SetValue(clausula.ValidadeInicial.ToShortDateString());
                                    ws.Cell(currentLine, 3).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                                    ws.Cell(currentLine, 3).Style.Font.FontColor = XLColor.Black;

                                    if (clausula.ValidadeFinal is not null)
                                    {
                                        ws.Cell(currentLine, 4).SetValue(clausula.ValidadeFinal.Value.ToShortDateString());
                                        ws.Cell(currentLine, 4).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                                        ws.Cell(currentLine, 4).Style.Font.FontColor = XLColor.Black;
                                    }

                                    if (clausula.Abrangencia is not null && clausula.Abrangencia.Any())
                                    {
                                        List<string> abrangencia = new();

                                        foreach (var abrangenciaItem in clausula.Abrangencia)
                                        {
                                            abrangencia.Add(abrangenciaItem.Municipio + "/" + abrangenciaItem.Uf);
                                        }

                                        ws.Cell(currentLine, 5).SetValue(string.Join(", ", abrangencia.Distinct()));
                                        ws.Cell(currentLine, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                                        ws.Cell(currentLine, 5).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                                        ws.Cell(currentLine, 5).Style.Font.FontColor = XLColor.Black;
                                    }

                                    if (clausula.SindicatoLaboral is not null && clausula.SindicatoLaboral.Any())
                                    {
                                        var siglas = clausula.SindicatoLaboral.Select(s => s.Sigla);
                                        var cnpj = clausula.SindicatoLaboral.Select(s => CNPJ.Formatar(s.Cnpj.Value));
                                        var nome = clausula.SindicatoLaboral.Select(s => s.Denominacao);

                                        ws.Cell(currentLine, 6).SetValue(string.Join(", ", siglas.Distinct()));
                                        ws.Cell(currentLine, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                                        ws.Cell(currentLine, 6).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                                        ws.Cell(currentLine, 6).Style.Font.FontColor = XLColor.Black;

                                        ws.Cell(currentLine, 7).SetValue(string.Join(", ", cnpj.Distinct()));
                                        ws.Cell(currentLine, 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                                        ws.Cell(currentLine, 7).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                                        ws.Cell(currentLine, 7).Style.Font.FontColor = XLColor.Black;

                                        ws.Cell(currentLine, 8).SetValue(string.Join(", ", nome.Distinct()));
                                        ws.Cell(currentLine, 8).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                                        ws.Cell(currentLine, 8).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                                        ws.Cell(currentLine, 8).Style.Font.FontColor = XLColor.Black;
                                    }

                                    if (clausula.SindicatoPatronal is not null && clausula.SindicatoPatronal.Any())
                                    {
                                        var siglas = clausula.SindicatoPatronal.Select(s => s.Sigla);
                                        var cnpj = clausula.SindicatoPatronal.Select(s => CNPJ.Formatar(s.Cnpj.Value));
                                        var nome = clausula.SindicatoPatronal.Select(s => s.Denominacao);

                                        ws.Cell(currentLine, 9).SetValue(string.Join(", ", siglas.Distinct()));
                                        ws.Cell(currentLine, 9).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                                        ws.Cell(currentLine, 9).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                                        ws.Cell(currentLine, 9).Style.Font.FontColor = XLColor.Black;

                                        ws.Cell(currentLine, 10).SetValue(string.Join(", ", cnpj.Distinct()));
                                        ws.Cell(currentLine, 10).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                                        ws.Cell(currentLine, 10).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                                        ws.Cell(currentLine, 10).Style.Font.FontColor = XLColor.Black;

                                        ws.Cell(currentLine, 11).SetValue(string.Join(", ", nome.Distinct()));
                                        ws.Cell(currentLine, 11).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                                        ws.Cell(currentLine, 11).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                                        ws.Cell(currentLine, 11).Style.Font.FontColor = XLColor.Black;
                                    }

                                    ws.Cell(currentLine, 12).SetValue(clausula.CodigoDocumentoLegado);
                                    ws.Cell(currentLine, 12).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                                    ws.Cell(currentLine, 12).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                                    ws.Cell(currentLine, 12).Style.Font.FontColor = XLColor.Black;

                                    ws.Cell(currentLine, 13).SetValue(clausula.IdClausula);
                                    ws.Cell(currentLine, 13).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                                    ws.Cell(currentLine, 13).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                                    ws.Cell(currentLine, 13).Style.Font.FontColor = XLColor.Black;

                                    ws.Cell(currentLine, 14).SetValue(clausula.NomeClausula);
                                    ws.Cell(currentLine, 14).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                                    ws.Cell(currentLine, 14).Style.Font.FontColor = XLColor.Black;

                                    ws.Cell(currentLine, 15).SetValue(clausula.GrupoClausula);
                                    ws.Cell(currentLine, 15).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                                    ws.Cell(currentLine, 15).Style.Font.FontColor = XLColor.Black;

                                    ws.Cell(currentLine, 16).SetValue(clausula.Sinonimo);
                                    ws.Cell(currentLine, 16).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                                    ws.Cell(currentLine, 16).Style.Font.FontColor = XLColor.Black;

                                    ws.Cell(currentLine, 17).SetValue(clausula.Assunto);
                                    ws.Cell(currentLine, 17).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                                    ws.Cell(currentLine, 17).Style.Font.FontColor = XLColor.Black;

                                    if (clausula.DataProcessamento is not null)
                                    {
                                        ws.Cell(currentLine, 18).SetValue(clausula.DataProcessamento.Value.ToShortDateString());
                                        ws.Cell(currentLine, 18).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                                        ws.Cell(currentLine, 18).Style.Font.FontColor = XLColor.Black;
                                    }

                                    ws.Cell(currentLine, 19).SetValue(clausula.ResponsavelProcessamento);
                                    ws.Cell(currentLine, 19).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                                    ws.Cell(currentLine, 19).Style.Font.FontColor = XLColor.Black;

                                    ws.Cell(currentLine, 20).SetValue(clausula.Aprovado);
                                    ws.Cell(currentLine, 20).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                                    ws.Cell(currentLine, 20).Style.Font.FontColor = XLColor.Black;

                                    ws.Cell(currentLine, 21).SetValue(clausula.NumeroClausula);
                                    ws.Cell(currentLine, 21).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                                    ws.Cell(currentLine, 21).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                                    ws.Cell(currentLine, 21).Style.Font.FontColor = XLColor.Black;

                                    ws.Cell(currentLine, 22).SetValue(clausula.TextoClausula);
                                    ws.Cell(currentLine, 22).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                                    ws.Cell(currentLine, 22).Style.Font.FontColor = XLColor.Black;

                                    currentLine++;
                                }
                            })
                            .ToByteArray(workbook);

            return Result.Success<byte[], Error>(bytes);
        }

        private async Task<IEnumerable<RelatorioClausulasViewModel>?> ObterTodos(int documentoId)
        {
            var clausulas = await (from cg in _context.ClausulaGerals
                                   join etc in _context.EstruturaClausula on cg.EstruturaClausulaId equals etc.Id into _etc
                                   from etc in _etc.DefaultIfEmpty()
                                   join asst in _context.Assuntos on cg.AssuntoId equals asst.Idassunto into _asst
                                   from asst in _asst.DefaultIfEmpty()
                                   join ds in _context.DocSinds on cg.DocumentoSindicalId equals ds.Id into _ds
                                   from ds in _ds.DefaultIfEmpty()
                                   join dl in _context.DocumentosLocalizados on (long?)ds.DocumentoLocalizacaoId equals dl.Id into _dl
                                   from dl in _dl.DefaultIfEmpty()
                                   join ua in _context.UsuarioAdms on cg.ResponsavelProcessamento equals ua.Id into _ua
                                   from ua in _ua.DefaultIfEmpty()
                                   join td in _context.TipoDocs on ds.TipoDocumentoId equals td.Id into _td
                                   from td in _td.DefaultIfEmpty()
                                   join ec in _context.EstruturaClausula on cg.EstruturaClausulaId equals ec.Id
                                   join gc in _context.GrupoClausula on ec.GrupoClausulaId equals gc.Id into _gc
                                   from gc in _gc.DefaultIfEmpty()
                                   join sn in _context.Sinonimos on cg.SinonimoId equals sn.Id into _sn
                                   from sn in _sn.DefaultIfEmpty()
                                   where cg.DocumentoSindicalId == documentoId
                                   select new RelatorioClausulasViewModel
                                   {
                                       IdDocumento = ds.Id,
                                       NomeDocumento = td.Nome,
                                       ValidadeInicial = ds.DataValidadeInicial,
                                       ValidadeFinal = ds.DataValidadeFinal,
                                       Abrangencia = ds.Abrangencias,
                                       SindicatoLaboral = (from dsl in _context.DocumentosSindicatosLaborais
                                                           join sl in _context.SindEmps on dsl.SindicatoLaboralId equals sl.Id
                                                           where dsl.DocumentoSindicalId == documentoId
                                                           select sl)
                                                           .AsSplitQuery()
                                                           .ToList(),
                                       SindicatoPatronal = (from dsl in _context.DocumentosSindicatosPatronais
                                                            join sp in _context.SindPatrs on dsl.SindicatoPatronalId equals sp.Id
                                                            where dsl.DocumentoSindicalId == documentoId
                                                            select sp)
                                                           .AsSplitQuery()
                                                           .ToList(),
                                       CodigoDocumentoLegado = dl.IdLegado,
                                       IdClausula = cg.Id,
                                       NomeClausula = etc.Nome,
                                       Assunto = asst.Assunto1,
                                       DataProcessamento = cg.DataProcessamento,
                                       ResponsavelProcessamento = ua.Nome,
                                       Aprovado = cg.Aprovado != null && cg.Aprovado == true ? "sim" : "nao",
                                       NumeroClausula = cg.Numero,
                                       TextoClausula = cg.Texto,
                                       GrupoClausula = gc.Nome,
                                       Sinonimo = sn.Nome
                                   }).ToListAsync();

            if (clausulas == null)
            {
                return null;
            }

            return clausulas;
        }
    }
}
