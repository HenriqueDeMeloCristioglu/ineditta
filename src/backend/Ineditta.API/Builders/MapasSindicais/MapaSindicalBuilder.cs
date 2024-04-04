using System.Collections.Concurrent;
using System.Globalization;

using ClosedXML.Excel;

using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Spreadsheet;

using Ineditta.Application.Clausulas.Entities.InformacoesAdicionais;
using Ineditta.BuildingBlocks.Core.Domain.ValueObjects;
using Ineditta.BuildingBlocks.Core.Extensions;
using Ineditta.BuildingBlocks.Core.Parallels;
using Ineditta.Repository.Clausulas.Geral.Views.Clausula;
using Ineditta.Repository.Clausulas.Views.InformacoesAdicionais;

using Microsoft.IdentityModel.Tokens;

namespace Ineditta.API.Builders.MapasSindicais
{
    public static class MapaSindicalBuilder
    {
        private const int maxCaracteresPorCelula = 32767;
        private const int NOME_ESTRUTURA_CLAUSULA_ID = 170;
        private const int IF_COMPARATIVO_ID = 310;

        private static readonly Dictionary<TipoDado, Action<IXLWorksheet, int, int, ClausulaGeralInformacaoAdicionalVw>> _cellsActions;
        private static readonly ConcurrentBag<ColumnWidth> ColumnsWidth = new();

#pragma warning disable S3963 // "static" fields should be initialized inline
        static MapaSindicalBuilder()
#pragma warning restore S3963 // "static" fields should be initialized inline
        {
            _cellsActions = new Dictionary<TipoDado, Action<IXLWorksheet, int, int, ClausulaGeralInformacaoAdicionalVw>>
            {
                {
                    TipoDado.ComboMultipla,
                    (ws, row, column, info) =>
#pragma warning disable S125 // Sections of code should not be commented out
                    {
                        ws.Cell(row, column).SetValue(info.ValorCombo);
                        //ws.Column(column).Width = 15;
                        ColumnsWidth.Add(new ColumnWidth{Column = column, Width = 15});
                    }
                },
                {
                    TipoDado.Combo,
                    (ws, row, column, info) => 
                    {
                        ws.Cell(row, column).SetValue(info.ValorCombo);
                        //ws.Column(column).Width = 15;
                        ColumnsWidth.Add(new ColumnWidth{Column = column, Width = 15});
                    }
                },
                {
                    TipoDado.Descricao,
                    (ws, row, column, info) =>
                    {
                        ws.Cell(row, column).SetValue(info.ValorDescricao);
                        ws.Cell(row, column).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Justify;
                        ColumnsWidth.Add(new ColumnWidth{Column = column, Width = 50});
                        //ws.Column(column).Width = 50;
                    }
                },
                {
                    TipoDado.Texto,
                    (ws, row, column, info) =>
                    {
                        var valor = info.ValorTexto ?? info.ValorDescricao;
                        ws.Cell(row, column).SetValue(valor);
                        ws.Cell(row, column).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Justify;
                        ColumnsWidth.Add(new ColumnWidth{Column = column, Width = 25});
                        //ws.Column(column).Width = 25;
                    }
                },
                {
                    TipoDado.Data,
                    (ws, row, column, info) =>
                {
                    if (DateOnly.TryParse(info.ValorData, CultureInfo.InvariantCulture, out DateOnly Valor))
                    {
                        if (Valor.Year < 1950)
                        {
                            return;
                        }

                        ws.Cell(row, column).SetValue(Valor.ToDateTime(TimeOnly.MinValue));
                        ws.Cell(row, column).Style.DateFormat.Format = "dd/MM/yyyy";
                        //ws.Column(column).Width = 15;
                    }

                }
                },

                {
                    TipoDado.Percentual,
                    (ws, row, column, info) =>
                {
                    if (!info.ValorPercentual.HasValue)
                    {
                        return;
                    }

                    ws.Cell(row, column).SetValue(info.ValorPercentual.Value);
                    ws.Cell(row, column).Style.NumberFormat.SetFormat("0.00\\%");
                    //ws.Column(column).Width = 15;
                }
                },

                {
                    TipoDado.Monetario,
                    (ws, row, column, info) =>
                {
                    if (!info.ValorNumerico.HasValue)
                    {
                        return;
                    }

                    ws.Cell(row, column).SetValue(info.ValorNumerico.Value);
                    ws.Cell(row, column).Style.NumberFormat.SetFormat("R$ #,##0.00");
                }
                },

                {
                    TipoDado.Inteiro,
                    (ws, row, column, info) =>
                    {
                        if (!info.ValorNumerico.HasValue)
                        {
                            return;
                        }

                        ws.Cell(row, column).SetValue(info.ValorNumerico.Value);
                    }
                },

                {
                    TipoDado.Hora,
                    (ws, row, column, info) =>
                    {
                        if (string.IsNullOrEmpty(info.ValorHora))
                        {
                            return;
                        }

                        ws.Cell(row, column).SetValue(info.ValorHora);
                    }
                }
            };
        }

        public static byte[]? Converter(IReadOnlyList<ClausulaGeralInformacaoAdicionalVw> informacoesAdicionais)
        {
            if (informacoesAdicionais == null || !informacoesAdicionais.Any())
            {
                return default;
            }

            using var wb = new XLWorkbook();

            var ws = wb.Worksheets.Add("Mapa sindical");

            var headersPadroes = ObterColunasPadroes();

            var headersInformacoesAdicionais = (from infoAd in informacoesAdicionais
                                                where infoAd.InformacaoAdicionalId != NOME_ESTRUTURA_CLAUSULA_ID
                                                where infoAd.InformacaoAdicionalId != IF_COMPARATIVO_ID
                                                group infoAd by infoAd.InformacaoAdicionalId into _infoAd
                                                select new
                                                {
                                                    InformacaoAdicionalId = _infoAd.Key,
                                                    _infoAd.First().InformacaoAdicionalNome,
                                                    MenorClausulaId = _infoAd.Min(x => x.ClausulaId),
                                                    _infoAd.OrderBy(x => x.ClausulaId).First().Sequencia,
                                                })
                                                .OrderBy(p => p.InformacaoAdicionalNome)
                                                .ThenBy(p => p.Sequencia)
                                                .ToList();

            var informacaoAdicionalHeaderIndex = new Dictionary<int, int>();

            for (int i = 0; i < headersPadroes.Count; i++)
            {
                var cell = ws.Cell(1, (i + 1)).SetValue(headersPadroes[i]);

                cell.Style.Font.Bold = true;
                cell.Style.Fill.BackgroundColor = XLColor.Gray;
                cell.Style.Font.FontColor = XLColor.White;
            }

            for (int i = 0; i < headersInformacoesAdicionais.Count; i++)
            {
                var posicao = headersPadroes.Count + 1 + i;

                var cell = ws.Cell(1, posicao).SetValue(headersInformacoesAdicionais[i].InformacaoAdicionalNome);
                cell.Style.Font.Bold = true;
                cell.Style.Fill.SetBackgroundColor(XLColor.FromArgb(4, 128, 190));
                cell.Style.Font.FontColor = XLColor.White;

                informacaoAdicionalHeaderIndex.Add(posicao, headersInformacoesAdicionais[i].InformacaoAdicionalId);
            }

            ws.Column(1).Width = 20;
            ws.Column(2).Width = 20;
            ws.Column(3).Width = 20;
            ws.Column(4).Width = 20;
            ws.Column(5).Width = 20;
            ws.Column(6).Width = 20;
            ws.Column(7).Width = 20;
            ws.Column(8).Width = 50;
            ws.Column(9).Width = 20;
            ws.Column(10).Width = 20;
            ws.Column(11).Width = 50;
            ws.Column(12).Width = 50;
            ws.Column(13).Width = 15;
            ws.Column(14).Width = 20;
            ws.Column(15).Width = 15;
            ws.Column(16).Width = 15;
            ws.Column(17).Width = 15;
            ws.Column(19).Width = 20;
            ws.Column(ObterColunasPadroes().Count - 1).Width = 20;

            var clausulasIds = informacoesAdicionais
                .Select(p => p.ClausulaId).Distinct();

            AtomicCounter linhaInicial = new AtomicCounter(2);
            
            ParallelOptions parallelOptions = new ParallelOptions();
            parallelOptions.MaxDegreeOfParallelism = 10;

            Mutex mutex = new Mutex();

            Parallel.ForEach(clausulasIds, parallelOptions, clausulaId =>
            {
                var informacaoClausula = informacoesAdicionais.FirstOrDefault(info => info.ClausulaId == clausulaId);

                var gruposDados = informacoesAdicionais.Where(info => info.ClausulaId == clausulaId)
                                                       .Select(info => info.GrupoDados).Distinct().Order();

                var abrangenciaString = informacaoClausula is not null && informacaoClausula!.Abrangencias is not null ?
                                            string.Join(", ", informacaoClausula!.Abrangencias.Select(a => a.Municipio + "/" + a.Uf)) : "";

                int linhaInicialDoBloco = 0;
                int linhaGrupo = 0;

                lock (linhaInicial)
                {
                    linhaInicialDoBloco = linhaInicial.Value;
                    linhaInicial.Value += gruposDados.Count();
                }
                
                foreach (var grupoDado in gruposDados)
                {
                    mutex.WaitOne();
                    IXLRow novaLinha = ws.Row(linhaInicialDoBloco + linhaGrupo);
                    mutex.ReleaseMutex();

                    if (!string.IsNullOrEmpty(informacaoClausula!.CodigosSindicatoClienteUnidades))
                    {
                        mutex.WaitOne();
                        novaLinha.Cell(1).SetValue(informacaoClausula!.CodigosSindicatoClienteUnidades);
                        mutex.ReleaseMutex();
                    }

                    if (!string.IsNullOrEmpty(informacaoClausula!.CodigosUnidades))
                    {
                        mutex.WaitOne();
                        novaLinha.Cell(2).SetValue(informacaoClausula!.CodigosUnidades);
                        mutex.ReleaseMutex();
                    }

                    if (!string.IsNullOrEmpty(informacaoClausula!.CnpjUnidades))
                    {
                        mutex.WaitOne();
                        novaLinha.Cell(3).SetValue(informacaoClausula!.CnpjUnidades);
                        mutex.ReleaseMutex();
                    }

                    if (!string.IsNullOrEmpty(informacaoClausula!.UfsUnidades))
                    {
                        mutex.WaitOne();
                        novaLinha.Cell(4).SetValue(informacaoClausula!.UfsUnidades);
                        mutex.ReleaseMutex();
                    }

                    if (!string.IsNullOrEmpty(informacaoClausula!.MunicipiosUnidades))
                    {
                        mutex.WaitOne();
                        novaLinha.Cell(5).SetValue(informacaoClausula!.MunicipiosUnidades);
                        mutex.ReleaseMutex();
                    }

                    if (informacaoClausula!.DocumentoSindicatosLaborais != null)
                    {
                        mutex.WaitOne();
                        novaLinha.Cell(6).SetValue(string.Join(", ", informacaoClausula!.DocumentoSindicatosLaborais!.Select(ict => ict.Sigla)));
                        novaLinha.Cell(7).SetValue(string.Join(", ", informacaoClausula!.DocumentoSindicatosLaborais!.Select(ict => CNPJ.Formatar(ict.Cnpj!))));
                        mutex.ReleaseMutex();
                    }

                    if (!string.IsNullOrEmpty(informacaoClausula!.DenominacoesLaborais))
                    {
                        mutex.WaitOne();
                        novaLinha.Cell(8).SetValue(informacaoClausula!.DenominacoesLaborais);
                        mutex.ReleaseMutex();
                    }

                    if (informacaoClausula!.DocumentoSindicatosPatronais != null)
                    {
                        mutex.WaitOne();
                        novaLinha.Cell(9).SetValue(string.Join(", ", informacaoClausula!.DocumentoSindicatosPatronais!.Select(ict => ict.Sigla)));
                        novaLinha.Cell(10).SetValue(string.Join(", ", informacaoClausula!.DocumentoSindicatosPatronais!.Select(ict => CNPJ.Formatar(ict.Cnpj!))));
                        mutex.ReleaseMutex();
                    }

                    if (!string.IsNullOrEmpty(informacaoClausula!.DenominacoesPatronais))
                    {
                        mutex.WaitOne();
                        novaLinha.Cell(11).SetValue(informacaoClausula!.DenominacoesPatronais);
                        mutex.ReleaseMutex();
                    }

                    if (informacaoClausula!.Abrangencias != null)
                    {
                        var overflowMessage = "Conteúdo limitado pelo Excel, consultar as demais informações no sistema. ";

                        if (abrangenciaString.Length >= (maxCaracteresPorCelula - overflowMessage.Length)) abrangenciaString = string.Concat(overflowMessage, abrangenciaString.AsSpan(0, maxCaracteresPorCelula - overflowMessage.Length));

                        mutex.WaitOne();
                        var cell = novaLinha.Cell(12).SetValue(abrangenciaString);
                        mutex.ReleaseMutex();

                        if (abrangenciaString.Length >= (maxCaracteresPorCelula - overflowMessage.Length))
                        {
                            mutex.WaitOne();
                            cell.CreateRichText().Substring(0, overflowMessage.Length).SetBold();
                            mutex.ReleaseMutex();
                        }
                    }

                    if (informacaoClausula.DocumentoDataAprovacao.HasValue)
                    {
                        mutex.WaitOne();
                        novaLinha.Cell(13).SetValue(informacaoClausula.DocumentoDataAprovacao.Value.ToDateTime(TimeOnly.MinValue));
                        novaLinha.Cell(13).Style.DateFormat.Format = "dd/MM/yyyy";
                        mutex.ReleaseMutex();
                    }

                    mutex.WaitOne();
                    novaLinha.Cell(14).SetValue(informacaoClausula.TipoDocumentoNome);
                    mutex.ReleaseMutex();

                    if (informacaoClausula.AtividadeEconomicas != null)
                    {
                        mutex.WaitOne();
                        novaLinha.Cell(15).SetValue(string.Join(", ", informacaoClausula.AtividadeEconomicas!.Select(aet => aet.Subclasse)));
                        mutex.ReleaseMutex();
                    }

                    mutex.WaitOne();
                    novaLinha.Cell(16).SetValue(informacaoClausula.DataBase);
                    mutex.ReleaseMutex();

                    mutex.WaitOne();
                    novaLinha.Cell(17).SetValue(informacaoClausula.DocumentoValidadeFinal.ToDateTime(TimeOnly.MinValue));
                    mutex.ReleaseMutex();

                    mutex.WaitOne();
                    novaLinha.Cell(19).SetValue(informacaoClausula.GrupoClausulaNome);
                    mutex.ReleaseMutex();

                    var informacoesAdicionaisClausulas = informacoesAdicionais.Where(info => info.ClausulaId == clausulaId && info.GrupoDados == grupoDado).OrderBy(p => p.Sequencia);

                    foreach (var informacaoAdicionalClausula in informacoesAdicionaisClausulas)
                    {
                        if (informacaoAdicionalClausula.InformacaoAdicionalId == IF_COMPARATIVO_ID)
                        {
                            continue;
                        }

                        if (informacaoAdicionalClausula.InformacaoAdicionalId == NOME_ESTRUTURA_CLAUSULA_ID)
                        {
                            mutex.WaitOne();
                            novaLinha.Cell(ObterColunasPadroes().Count - 1).SetValue(informacaoAdicionalClausula.ValorCombo);
                            mutex.ReleaseMutex();
                            continue;
                        }

                        var informacaoAdicionalNumeroColuna = informacaoAdicionalHeaderIndex.Single(infoHeader => infoHeader.Value == informacaoAdicionalClausula.InformacaoAdicionalId).Key;

                        mutex.WaitOne();
                        if (_cellsActions.TryGetValue(informacaoAdicionalClausula.InformacaoAdicionalTipoDado, out _))
                        {
                            _cellsActions[informacaoAdicionalClausula.InformacaoAdicionalTipoDado].Invoke(ws, linhaInicialDoBloco + linhaGrupo, informacaoAdicionalNumeroColuna, informacaoAdicionalClausula);
                        }
                        mutex.ReleaseMutex();
                    }

                    linhaGrupo++;
                }
            });

            var columnsWidthSemDuplicatas = ColumnsWidth.DistinctBy(x => x.Column);
            
            foreach (var columnWidth in columnsWidthSemDuplicatas)
            {
                ws.Column(columnWidth.Column).Width = columnWidth.Width;
            }

            using var stream = new MemoryStream();

            wb.SaveAs(stream);

            return stream.ToArray();
        }

        private static List<string> ObterColunasPadroes()
        {
            return new List<string>
            {
                "Código Sindicato",
                "Código Estabelecimento",
                "CNPJ Estabelecimentos",
                "UF Estabelecimento",
                "Município Estabelecimento",
                "Sigla Sind. Laboral",
                "CNPJ Laboral",
                "Nome Sind. Laboral",
                "Sigla Sind. Patronal",
                "CNPJ Patronal",
                "Nome Sind. Patronal",
                "Abrangência do documento",
                "Data de processamento Ineditta",
                "Nome Documento",
                "Atividade econômica",
                "Data Base",
                "Validade Final",
                "Nome da clausula",
                "Grupo cláusulas"
            };
        }
    }

    internal sealed class ColumnWidth
    {
        public int Column { get; set; }
        public int Width { get; set; }
    }
}
