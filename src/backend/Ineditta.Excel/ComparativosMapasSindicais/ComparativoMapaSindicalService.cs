using ClosedXML.Excel;
using ClosedXML.Excel.Exceptions;
using CSharpFunctionalExtensions;

using DocumentFormat.OpenXml.Office2010.ExcelAc;
using DocumentFormat.OpenXml.Spreadsheet;

using Ineditta.Application.Clausulas.InformacoesAdicionais.Entities;
using Ineditta.Excel.ComparativosMapasSindicais.Dtos;
using Ineditta.Excel.Configurations;

using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Ineditta.Excel.ComparativosMapasSindicais
{
    public class ComparativoMapaSindicalService : IComparativoMapaSindicalService
    {
        private readonly TemplateExcelConfiguration _templateExcelConfiguration;

        private readonly Dictionary<TipoDado, Action<IXLWorksheet, int, int, ClausulaSindicalDadosDto>> _cellsActions;

        public ComparativoMapaSindicalService(IOptions<TemplateExcelConfiguration> templateExcelConfiguration)
        {
            _templateExcelConfiguration = templateExcelConfiguration?.Value ?? throw new ArgumentNullException(nameof(templateExcelConfiguration));

            _cellsActions = new Dictionary<TipoDado, Action<IXLWorksheet, int, int, ClausulaSindicalDadosDto>>();

            _cellsActions.Add(TipoDado.Combo, (ws, row, column, info) => ws.Cell(row, column).SetValue(info.ValorCombo));
            _cellsActions.Add(TipoDado.Descricao, (ws, row, column, info) => ws.Cell(row, column).SetValue(info.ValorDescricao));

            _cellsActions.Add(TipoDado.Data, (ws, row, column, info) =>
            {
                if (!info.ValorData.HasValue || info.ValorData.Value.Year < 1900)
                {
                    ws.Cell(row, column).SetValue("-");
                    return;
                }

                ws.Cell(row, column).SetValue(info.ValorData.Value.ToDateTime(TimeOnly.MinValue));
                ws.Cell(row, column).Style.DateFormat.Format = "dd/MM/yyyy";
            });

            _cellsActions.Add(TipoDado.Percentual, (ws, row, column, info) =>
            {
                if (!info.ValorPercentual.HasValue)
                {
                    ws.Cell(row, column).SetValue("-");
                    return;
                }

                ws.Cell(row, column).SetValue(info.ValorPercentual.Value);
                ws.Cell(row, column).Style.NumberFormat.SetFormat("0.00\\%");
            });

            _cellsActions.Add(TipoDado.Monetario, (ws, row, column, info) =>
            {
                if (!info.ValorPercentual.HasValue)
                {
                    ws.Cell(row, column).SetValue("-");
                    return;
                }

                ws.Cell(row, column).SetValue(info.ValorPercentual.Value);
                ws.Cell(row, column).Style.NumberFormat.SetFormat("R$ #,##0.00");
            });

            _cellsActions.Add(TipoDado.Inteiro, (ws, row, column, info) =>
            {
                if (!info.ValorNumerico.HasValue)
                {
                    ws.Cell(row, column).SetValue("-");
                    return;
                }

                ws.Cell(row, column).SetValue(info.ValorNumerico.Value);
            });

            _cellsActions.Add(TipoDado.Hora, (ws, row, column, info) =>
            {
                if (string.IsNullOrEmpty(info.ValorHora))
                {
                    ws.Cell(row, column).SetValue("-");
                    return;
                }

                ws.Cell(row, column).SetValue(info.ValorHora);
            });
        }

        public async ValueTask<Result<byte[]>> GerarAsync(ComparativoMapaSindicalDto model)
        {
            var colunas = model.ListaCabecalhoFixo.Select(x => x.Cabecalho)
                                                         .Distinct()
                                                         .ToList();

            var templateColumnCellHeader = primeiraAbaTemplateWs.Range(primeiraAbaTemplateWs.Cell(1, 3), primeiraAbaTemplateWs.Cell(primeiraAbaTemplateWs.LastRowUsed().RowNumber(), 3));

            if (colunas.Count > 1)
            {
                templateColumnCellHeader.InsertColumnsBefore(colunas.Count - 1);
            }

            for (int c = 0; c < colunas.Count; c++)
            {
                var coluna = colunas[c];

                primeiraAbaTemplateWs.Cell(1, c + 2).Value = coluna;

                for (int r = 0; r < primeiraAbaTemplateWs.LastRowUsed().RowNumber(); r++)
                {
                    var key = primeiraAbaTemplateWs.Cell(r + 2, 1).GetValue<string>();

                    var item = model.ListaCabecalhoFixo.FirstOrDefault(x => x.Cabecalho == coluna && x.DescricaoChave == key);

                    if (item != null)
                    {
                        primeiraAbaTemplateWs.Cell(r + 2, c + 2).Value = item.Valor;
                    }
                }
            }

            var grupoDados = model.Dados.Select(x => x.Grupo)
                                        .Distinct()
                                        .ToList();

            var dados = new List<ClausulaSindicalDadosDto>();

            int celulaGrupoInicial = 8;
            int celulaDadosInicial = 9;

            for (int gd = 0; gd < grupoDados.Count; gd++)
            {
                var grupo = grupoDados[gd];

                celulaGrupoInicial = celulaGrupoInicial + dados.Count + 1;

                primeiraAbaTemplateWs.Cell(celulaGrupoInicial, 1).Value = grupo;
                primeiraAbaTemplateWs.Range(celulaGrupoInicial, 1, celulaGrupoInicial, primeiraAbaTemplateWs.LastCellUsed().Address.ColumnNumber).Style.Fill.BackgroundColor = XLColor.FromArgb(47, 84, 150);
                primeiraAbaTemplateWs.Cell(celulaGrupoInicial, 1).Style.Font.FontColor = XLColor.White;
                primeiraAbaTemplateWs.Cell(celulaGrupoInicial, 1).Style.Font.Bold = true;

                dados = model.Dados.Where(x => x.Grupo == grupo).ToList();

                for (int d = 0; d < dados.Count; d++)
                {
                    var item = dados[d];

                    celulaDadosInicial = (celulaDadosInicial + 1);

                    primeiraAbaTemplateWs.Cell(celulaDadosInicial, 1).Value = item.Descricao;
                    primeiraAbaTemplateWs.Cell(celulaDadosInicial, 1).Style.Fill.BackgroundColor = XLColor.FromArgb(189, 214, 238);
                    primeiraAbaTemplateWs.Cell(celulaDadosInicial, 1).Style.Font.FontColor = XLColor.Black;

                    for (int c = 0; c < colunas.Count; c++)
                    {
                        var coluna = colunas[c];

                        var colunaValor = model.Dados.FirstOrDefault(x => x.Cabecalho == coluna && x.Descricao == item.Descricao);

                        primeiraAbaTemplateWs.Cell(celulaDadosInicial, c + 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                        if (colunaValor == null)
                        {
                            primeiraAbaTemplateWs.Cell(celulaDadosInicial, c + 2).Value = " - ";
                            continue;
                        }

                        if (_cellsActions.TryGetValue(colunaValor.TipoDado, out _))
                        {
                            _cellsActions[colunaValor.TipoDado].Invoke(primeiraAbaTemplateWs, celulaDadosInicial, c + 2, colunaValor);
                        }
                    }

                    if ((d + 1) == dados.Count)
                    {
                        celulaDadosInicial++;
                    }
                }
            }

            primeiraAbaTemplateWs.Range(primeiraAbaTemplateWs.FirstCellUsed(), primeiraAbaTemplateWs.LastCellUsed()).Style.Border.TopBorder = XLBorderStyleValues.Dashed;
            primeiraAbaTemplateWs.Range(primeiraAbaTemplateWs.FirstCellUsed(), primeiraAbaTemplateWs.LastCellUsed()).Style.Border.InsideBorder = XLBorderStyleValues.Dashed;
            primeiraAbaTemplateWs.Range(primeiraAbaTemplateWs.FirstCellUsed(), primeiraAbaTemplateWs.LastCellUsed()).Style.Border.OutsideBorder = XLBorderStyleValues.Dashed;
            primeiraAbaTemplateWs.Range(primeiraAbaTemplateWs.FirstCellUsed(), primeiraAbaTemplateWs.LastCellUsed()).Style.Border.LeftBorder = XLBorderStyleValues.Dashed;
            primeiraAbaTemplateWs.Range(primeiraAbaTemplateWs.FirstCellUsed(), primeiraAbaTemplateWs.LastCellUsed()).Style.Border.RightBorder = XLBorderStyleValues.Dashed;
            primeiraAbaTemplateWs.Range(primeiraAbaTemplateWs.FirstCellUsed(), primeiraAbaTemplateWs.LastCellUsed()).Style.Border.TopBorder = XLBorderStyleValues.Dashed;

            using var stream = new MemoryStream();

            wbTemplate.SaveAs(stream);

            return await Task.FromResult(Result.Success(stream.ToArray()));
        }
    }
}
