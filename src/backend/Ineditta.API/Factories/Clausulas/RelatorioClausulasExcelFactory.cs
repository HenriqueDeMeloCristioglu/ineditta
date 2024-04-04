using System.Globalization;

using ClosedXML.Excel;

using Ineditta.API.ViewModels.Clausulas.ViewModels;
using System;

namespace Ineditta.API.Factories.Clausulas
{
    public static class RelatorioClausulasExcelFactory
    {
        public static byte[]? Create(IEnumerable<RelatorioClausulasExcelInfoViewModel> infos)
        {
            const int maxCaracteresPorCelula = 32767;
            const int larguraColunaPadrao = 40;
            const int alturaLinhaPadrao = 50;

            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Mapa sindical");

            ws.RowHeight = alturaLinhaPadrao;
            ws.Rows().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            ws.Rows().Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
            ws.Rows().Style.Alignment.WrapText = true;
            ws.Rows().Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            ws.Rows().Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            ws.Row(1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Row(1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            var headersPadroes = ObterColunasPadroes();
            for (int i = 0; i < headersPadroes.Count; i++)
            {
                var cell = ws.Cell(1, (i + 1)).SetValue(headersPadroes[i]);

                cell.Style.Font.Bold = true;
                cell.Style.Fill.SetBackgroundColor(XLColor.FromArgb(4, 128, 190));
                cell.Style.Font.FontColor = XLColor.White;
            }

            var linhaAtual = 2;

            foreach (var info in infos)
            {
                int colunaAtual = 1;

                if (!string.IsNullOrEmpty(info.CodigosSindicatoCliente))
                {
                    ws.Cell(linhaAtual, colunaAtual).SetValue(info.CodigosSindicatoCliente);
                    ws.Column(colunaAtual).Width = larguraColunaPadrao;
                }
                colunaAtual++;

                if (!string.IsNullOrEmpty(info.CodigosUnidades))
                {
                    ws.Cell(linhaAtual, colunaAtual).SetValue(info.CodigosUnidades);
                    ws.Column(colunaAtual).Width = larguraColunaPadrao;
                }
                colunaAtual++;

                if (!string.IsNullOrEmpty(info.CnpjsUnidades))
                {
                    ws.Cell(linhaAtual, colunaAtual).SetValue(info.CnpjsUnidades);
                    ws.Column(colunaAtual).Width = larguraColunaPadrao;
                }
                colunaAtual++;

                if (!string.IsNullOrEmpty(info.UfsUnidades))
                {
                    ws.Cell(linhaAtual, colunaAtual).SetValue(info.UfsUnidades);
                    ws.Column(colunaAtual).Width = larguraColunaPadrao;
                }
                colunaAtual++;

                if (!string.IsNullOrEmpty(info.MunicipiosUnidades))
                {
                    ws.Cell(linhaAtual, colunaAtual).SetValue(info.MunicipiosUnidades);
                    ws.Column(colunaAtual).Width = larguraColunaPadrao;
                }
                colunaAtual++;

                if (!string.IsNullOrEmpty(info.SiglasSindicatosLaborais))
                {
                    ws.Cell(linhaAtual, colunaAtual).SetValue(info.SiglasSindicatosLaborais);
                    ws.Column(colunaAtual).Width = larguraColunaPadrao;
                }
                colunaAtual++;

                if (!string.IsNullOrEmpty(info.CnpjsSindicatosLaborais))
                {
                    ws.Cell(linhaAtual, colunaAtual).SetValue(info.CnpjsSindicatosLaborais);
                    ws.Column(colunaAtual).Width = larguraColunaPadrao;
                }
                colunaAtual++;

                if (!string.IsNullOrEmpty(info.DenominacoesSindicatosLaborais))
                {
                    ws.Cell(linhaAtual, colunaAtual).SetValue(info.DenominacoesSindicatosLaborais);
                    ws.Column(colunaAtual).Width = larguraColunaPadrao;
                }
                colunaAtual++;

                if (!string.IsNullOrEmpty(info.SiglasSindicatosPatronais))
                {
                    ws.Cell(linhaAtual, colunaAtual).SetValue(info.SiglasSindicatosPatronais);
                    ws.Column(colunaAtual).Width = larguraColunaPadrao;
                }
                colunaAtual++;

                if (!string.IsNullOrEmpty(info.CnpjsSindicatosPatronais))
                {
                    ws.Cell(linhaAtual, colunaAtual).SetValue(info.CnpjsSindicatosPatronais);
                    ws.Column(colunaAtual).Width = larguraColunaPadrao;
                }
                colunaAtual++;

                if (!string.IsNullOrEmpty(info.DenominacoesSindicatosPatronais))
                {
                    ws.Cell(linhaAtual, colunaAtual).SetValue(info.DenominacoesSindicatosPatronais);
                    ws.Column(colunaAtual).Width = larguraColunaPadrao;
                }
                colunaAtual++;

                if (info.DataLiberacaoClausulas.HasValue)
                {
                    ws.Cell(linhaAtual, colunaAtual).SetValue(info.DataLiberacaoClausulas.Value.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture));
                    ws.Column(colunaAtual).Width = larguraColunaPadrao;
                }
                colunaAtual++;

                if (!string.IsNullOrEmpty(info.NomeDocumento))
                {
                    ws.Cell(linhaAtual, colunaAtual).SetValue(info.NomeDocumento);
                    ws.Column(colunaAtual).Width = larguraColunaPadrao;
                }
                colunaAtual++;

                if (!string.IsNullOrEmpty(info.ReferenciasDocumento))
                {
                    ws.Cell(linhaAtual, colunaAtual).SetValue(info.ReferenciasDocumento);
                    ws.Column(colunaAtual).Width = larguraColunaPadrao;
                }
                colunaAtual++;

                if (info.AtividadesEconomicasDocumento is not null && info.AtividadesEconomicasDocumento!.Any())
                {
                    ws.Cell(linhaAtual, colunaAtual).SetValue(string.Join("; ", info.AtividadesEconomicasDocumento!.Select(a => a.Subclasse)));
                    ws.Column(colunaAtual).Width = larguraColunaPadrao;
                }
                colunaAtual++;

                if (info.ValidadeInicialDocumento.HasValue)
                {
                    ws.Cell(linhaAtual, colunaAtual).SetValue(info.ValidadeInicialDocumento.Value.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture));
                    ws.Column(colunaAtual).Width = larguraColunaPadrao;
                }
                colunaAtual++;

                if (info.ValidadeFinalDocumento.HasValue)
                {
                    ws.Cell(linhaAtual, colunaAtual).SetValue(info.ValidadeFinalDocumento.Value.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture));
                    ws.Column(colunaAtual).Width = larguraColunaPadrao;
                }
                colunaAtual++;

                if (!string.IsNullOrEmpty(info.DatabaseDocumento))
                {
                    ws.Cell(linhaAtual, colunaAtual).SetValue(info.DatabaseDocumento);
                    ws.Column(colunaAtual).Width = larguraColunaPadrao;
                }
                colunaAtual++;

                if (info.AbrangenciaDocumento is not null && info.AbrangenciaDocumento.Any())
                {
                    var overflowMessage = "Conteúdo limitado pelo Excel, consultar as demais informações no sistema. ";
                    var abrangencia = string.Join(", ", info.AbrangenciaDocumento.Select(a => a.Municipio + "/" + a.Uf));

                    if (abrangencia.Length >= (maxCaracteresPorCelula - overflowMessage.Length)) abrangencia = string.Concat(overflowMessage, abrangencia.AsSpan(0, maxCaracteresPorCelula - overflowMessage.Length));

                    var cell = ws.Cell(linhaAtual, colunaAtual).SetValue(abrangencia);

                    if (abrangencia.Length >= (maxCaracteresPorCelula - overflowMessage.Length))
                    {
                        cell.CreateRichText().Substring(0, overflowMessage.Length).SetBold();
                    }

                    ws.Column(colunaAtual).Width = larguraColunaPadrao;
                }
                colunaAtual++;

                if (!string.IsNullOrEmpty(info.NomeClausula))
                {
                    ws.Cell(linhaAtual, colunaAtual).SetValue(info.NomeClausula);
                    ws.Column(colunaAtual).Width = larguraColunaPadrao;
                }
                colunaAtual++;

                if (!string.IsNullOrEmpty(info.NomeGrupoClausula))
                {
                    ws.Cell(linhaAtual, colunaAtual).SetValue(info.NomeGrupoClausula);
                    ws.Column(colunaAtual).Width = larguraColunaPadrao;
                }
                colunaAtual++;

                if (!string.IsNullOrEmpty(info.NumeroClausula))
                {
                    ws.Cell(linhaAtual, colunaAtual).SetValue(info.NumeroClausula);
                    ws.Column(colunaAtual).Width = larguraColunaPadrao;
                }
                colunaAtual++;

                if (!string.IsNullOrEmpty(info.TextoClausula))
                {
                    ws.Cell(linhaAtual, colunaAtual).SetValue(info.TextoClausula);
                    ws.Column(colunaAtual).Width = larguraColunaPadrao;
                }
                colunaAtual++;

                if (!string.IsNullOrEmpty(info.ComentariosClausula))
                {
                    ws.Cell(linhaAtual, colunaAtual).SetValue(info.ComentariosClausula);
                }
                ws.Column(colunaAtual).Width = larguraColunaPadrao;

                linhaAtual++;
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
                "Código Estabelecimentos",
                "CNPJ Estabelecimento",
                "UF Estabelecimento",
                "Município Estabelecimento",
                "Sigla Sind. Laboral",
                "CNPJ Laboral",
                "Nome Sind. Laboral",
                "Sigla Sind. Patronal",
                "CNPJ Patronal",
                "Nome Sind. Patronal",
                "Data de processamento Ineditta",
                "Nome documento",
                "Assuntos",
                "Atividade Econômica",
                "Validade inicial",
                "Validade final",
                "Data-base",
                "Abrangência documento",
                "Nome cláusula",
                "Grupo cláusula",
                "Número da cláusula",
                "Texto da cláusula",
                "Comentário cláusula"
            };
        }
    }
}
