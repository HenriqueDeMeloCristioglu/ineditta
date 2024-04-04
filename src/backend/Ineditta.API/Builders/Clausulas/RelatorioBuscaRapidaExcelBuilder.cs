using System.Globalization;

using ClosedXML.Excel;

using Ineditta.API.Builders.Worksheets;
using Ineditta.API.ViewModels.ClausulasClientes.ViewModels;
using Ineditta.API.ViewModels.ClausulasGerais.ViewModels.Excel;
using Ineditta.BuildingBlocks.Core.Domain.ValueObjects;

namespace Ineditta.API.Builders.Clausulas
{
    public static class RelatorioBuscaRapidaExcelBuilder
    {
        public static byte[]? Criar(IEnumerable<RelatorioBuscaRapidaExcelViewModel> documentos, IEnumerable<ClausulClienteRelatorioBuscaRapidaExcel> clausulasCliente)
        {
            const int larguraColunaPadrao = 40;
            const int alturaLinhaPadrao = 250;

            using var wb = WorksheetBuilder.Create();

            var bytes = wb.AddWorkSheet("Busca Rápida")
                .Build(ws =>
                {
                    ws.ColumnWidth = larguraColunaPadrao;
                    ws.RowHeight = alturaLinhaPadrao;
                        
                    ws.Rows().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Rows().Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    ws.Rows().Style.Alignment.WrapText = true;
                    ws.Rows().Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                    ws.Rows().Style.Border.InsideBorder = XLBorderStyleValues.Medium;

                    ws.Row(1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Row(1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                    var headers = ObterColunasPadroes();
                    for (int i = 0; i < headers.Count; i++)
                    {
                        var cell = ws.Cell(1, (i + 1)).SetValue(headers[i]);

                        cell.Style.Font.Bold = true;
                        cell.Style.Fill.SetBackgroundColor(XLColor.FromArgb(165, 165, 165));
                        cell.Style.Font.FontColor = XLColor.White;
                    }
                    var linhaAtual = 2;

                    var clausulasDocumentos = ClausulasDocumentosConverter(documentos);

                    foreach (var documento in documentos)
                    {
                        ws.Cell(linhaAtual, 1).Value = documento.CodigosSindicatoCliente;
                        ws.Cell(linhaAtual, 2).Value = documento.CodigosUnidades;

                        if (documento.CnpjsUnidades is not null)
                        {
                            var cnpjsUnidades = documento.CnpjsUnidades.Split(", ");
                            var cnpjsUnidadesFormatados = (cnpjsUnidades.Select(u => CNPJ.Formatar(u.ToString()))).Distinct();
                            ws.Cell(linhaAtual, 3).Value = string.Join(", ", cnpjsUnidadesFormatados);
                        }

                        if (documento.UfsUnidades is not null)
                        {
                            ws.Cell(linhaAtual, 4).Value = documento.UfsUnidades;
                        }

                        if (documento.MunicipiosUnidades is not null)
                        {
                            ws.Cell(linhaAtual, 5).Value = documento.MunicipiosUnidades;
                        }

                        if (documento.SiglasSindicatosLaborais is not null)
                        {
                            ws.Cell(linhaAtual, 6).Value = documento.SiglasSindicatosLaborais;
                        }

                        if (documento.CnpjsSindicatosLaborais is not null)
                        {
                            var cnpjsLaborais = documento.CnpjsSindicatosLaborais.Split(", ");
                            var cnpjsLaboraisFormatados = (cnpjsLaborais.Select(u => CNPJ.Formatar(u.ToString()))).Distinct();
                            ws.Cell(linhaAtual, 7).Value = string.Join(", ", cnpjsLaboraisFormatados);
                        }

                        if (documento.DenominacoesSindicatosLaborais is not null)
                        {
                            ws.Cell(linhaAtual, 8).Value = documento.DenominacoesSindicatosLaborais;
                        }

                        if (documento.SiglasSindicatosPatronais is not null)
                        {
                            ws.Cell(linhaAtual, 9).Value = documento.SiglasSindicatosPatronais;

                        }

                        if (documento.CnpjsSindicatosPatronais is not null)
                        {
                            var cnpjsPatronais = documento.CnpjsSindicatosPatronais.Split(", ");
                            var cnpjsPatronaisFormatados = (cnpjsPatronais.Select(u => CNPJ.Formatar(u.ToString()))).Distinct();
                            ws.Cell(linhaAtual, 10).Value = string.Join(", ", cnpjsPatronaisFormatados);
                        }

                        if (documento.DenominacoesSindicatosPatronais is not null)
                        {
                            ws.Cell(linhaAtual, 11).Value = documento.DenominacoesSindicatosPatronais;
                        }

                        if (documento.AbrangenciaDocumento is not null && documento.AbrangenciaDocumento.Any())
                        {
                            var abrangencia = (documento.AbrangenciaDocumento.Distinct().Select(a => $"{a.Uf}/{a.Municipio}")).Distinct();
                            ws.Cell(linhaAtual, 12).Value = string.Join(", ", abrangencia);
                        }

                        if (documento.DataLiberacaoClausulas is not null)
                        {
                            ws.Cell(linhaAtual, 13).Value = documento.DataLiberacaoClausulas.Value.ToShortDateString();
                        }

                        if (documento.DataAssinaturaDocumento is not null)
                        {
                            ws.Cell(linhaAtual, 14).Value = documento.DataAssinaturaDocumento.Value.ToShortDateString();
                        }

                        if (documento.NomeDocumento is not null)
                        {
                            ws.Cell(linhaAtual, 15).Value = documento.NomeDocumento;
                        }

                        if (documento.AtividadesEconomicasDocumento is not null)
                        {
                            var atividadesEconomicas = (documento.AtividadesEconomicasDocumento.DistinctBy(a => a.Id).Select(a => a.Subclasse)).Distinct();
                            ws.Cell(linhaAtual, 16).Value = string.Join(", ", atividadesEconomicas);
                        }

                        if (documento.DatabaseDocumento is not null)
                        {
                            ws.Cell(linhaAtual, 17).Value = documento.DatabaseDocumento;
                        }

                        if (documento.ValidadeInicialDocumento.HasValue)
                        {
                            ws.Cell(linhaAtual, 18).Value = documento.ValidadeInicialDocumento.Value.ToShortDateString();
                        }

                        if (documento.ValidadeFinalDocumento.HasValue)
                        {
                            ws.Cell(linhaAtual, 19).Value = documento.ValidadeFinalDocumento.Value.ToShortDateString();
                        }

                        var colunaAtual = 20;
                        if (documento.Clausulas is not null && documento.Clausulas.Any())
                        {
                            foreach (var clausula in documento.Clausulas)
                            {
                                var clausulasClienteExistentes = clausulasDocumentos.Where(c => clausulasCliente.Any(cl => cl.EstruturaClausulaId == clausula.EstruturaClausulaId)).ToList();

                                if (linhaAtual == 2)
                                {
                                    ws.Cell(1, colunaAtual).Value = $"{clausula.Nome}";
                                    ws.EstilizarHeader(1, colunaAtual);

                                    if (clausulasClienteExistentes is not null && clausulasClienteExistentes.Any())
                                    {
                                        var colunaEmpresa = colunaAtual + 1;
                                        ws.Cell(1, colunaEmpresa).Value = $"{clausula.Nome} resumo empresa";
                                        ws.EstilizarHeader(1, colunaEmpresa);
                                    }
                                }

                                ws.Cell(linhaAtual, colunaAtual).Value = clausula.Texto;

                                colunaAtual++;

                                if (clausulasClienteExistentes is not null && clausulasClienteExistentes.Any())
                                {
                                    var clausulaCliente = clausulasCliente.SingleOrDefault(cl => cl.DocumentoId == clausula.DocumentoId && cl.EstruturaClausulaId == clausula.EstruturaClausulaId);

                                    ws.Cell(linhaAtual, colunaAtual).Value = clausulaCliente?.Texto;

                                    colunaAtual++;
                                }
                            }
                        }

                        linhaAtual++;
                    }
                })
                .ToByteArray(wb);

            return bytes;
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
                "Abrangência do documento",
                "Data de processamento Ineditta",
                "Data-assinatura/registro",
                "Nome documento",
                "Atividade Econômica",
                "Data Base",
                "Validade inicial",
                "Validade final"
            };
        }
    
        private static void EstilizarHeader(this IXLWorksheet ws, int row, int column)
        {
            var cell = ws.Cell(row, column);
            cell.Style.Font.Bold = true;
            cell.Style.Fill.SetBackgroundColor(XLColor.FromArgb(4, 128, 190));
            cell.Style.Font.FontColor = XLColor.White;
        }

        private static IEnumerable<ClausulaRelatorioBuscaRapidaViewModel> ClausulasDocumentosConverter(IEnumerable<RelatorioBuscaRapidaExcelViewModel> documentos)
        {
            List<ClausulaRelatorioBuscaRapidaViewModel> clausulas = new();

            foreach (var documento in documentos)
            {
                foreach(var clausula in documento.Clausulas)
                {
                    clausulas.Add(clausula);
                }
            }

            return clausulas;
        }
    }
}
