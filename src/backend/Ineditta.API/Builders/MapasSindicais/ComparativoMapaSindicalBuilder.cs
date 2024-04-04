using System.Globalization;

using ClosedXML.Excel;

using CSharpFunctionalExtensions;

using Ineditta.API.Configurations;
using Ineditta.Application.Clausulas.Entities.InformacoesAdicionais;
using Ineditta.Application.TiposInformacoesAdicionais.Entities;
using Ineditta.Application.Usuarios.Entities;
using Ineditta.BuildingBlocks.Core.Extensions;
using Ineditta.Repository.Clausulas.Views;
using Ineditta.Repository.Clausulas.Views.ComparativoMapaSindical;

using Microsoft.Extensions.Options;

namespace Ineditta.API.Builders.MapasSindicais
{
    public sealed class ComparativoMapaSindicalBuilder
    {
        private const int NUMERO_COLUNA_INICIAL = 2;
        private const int NUMERO_LINHA_INFORMACOES_ADICIONAIS_INICIAL = 11;

        private static readonly Dictionary<TipoDado, Action<IXLWorksheet, int, int, ComparativoMapaSindicalItemVw>> _cellsActions;

        private readonly TemplateExcelConfiguration _templateExcelConfiguration;

        public ComparativoMapaSindicalBuilder(IOptions<TemplateExcelConfiguration> templateExcelConfiguration)
        {
            _templateExcelConfiguration = templateExcelConfiguration?.Value ?? throw new ArgumentNullException(nameof(templateExcelConfiguration));
        }

#pragma warning disable S3963 // "static" fields should be initialized inline
        static ComparativoMapaSindicalBuilder()
#pragma warning restore S3963 // "static" fields should be initialized inline
        {
            _cellsActions = new Dictionary<TipoDado, Action<IXLWorksheet, int, int, ComparativoMapaSindicalItemVw>>
            {
                {
                    TipoDado.Combo,
                    (ws, row, column, info) =>
                    {
                        ws.Cell(row, column).SetValue(info.ValorCombo);
                        ws.Column(column).Width = 15;
                        ws.Cell(row, column).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    }
                },
                {
                    TipoDado.Descricao,
                    (ws, row, column, info) =>
                    {
                        ws.Cell(row, column).SetValue(info.ValorDescricao);
                        ws.Cell(row, column).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Justify;

                        ws.Column(column).Width = 50;
                    }
                },
                {
                    TipoDado.Texto,
                    (ws, row, column, info) =>
                    {
                        var valor = info.ValorTexto ?? info.ValorDescricao;
                        ws.Cell(row, column).SetValue(valor);
                        ws.Cell(row, column).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Justify;

                        ws.Column(column).Width = 25;
                    }
                },
                {
                    TipoDado.Data,
                    (ws, row, column, info) =>
                {
                    if (!info.ValorData.HasValue || info.ValorData.Value.Year < 1950)
                    {
                        return;
                    }

                    ws.Cell(row, column).SetValue(info.ValorData.Value.ToDateTime(TimeOnly.MinValue));
                    ws.Cell(row, column).Style.DateFormat.Format = "dd/MM/yyyy";
                    ws.Column(column).Width = 15;
                    ws.Cell(row, column).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
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
                    ws.Column(column).Width = 15;
                    ws.Cell(row, column).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
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
                    ws.Cell(row, column).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
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
                        ws.Cell(row, column).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
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
                        ws.Cell(row, column).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    }
                }
            };
        }

        internal Result<byte[]> Converter(Usuario usuarioLogado, IReadOnlyList<ComparativoMapaSindicalPrincipalVw> documentos, IReadOnlyList<ComparativoMapaSindicalItemVw> clausulas)
        {
            if (documentos == null || !documentos.Any() || clausulas == null || !clausulas.Any())
            {
                return Result.Failure<byte[]>("Não há dados processados para a seleção efetuada");
            }

            using var templateFile = new FileStream(_templateExcelConfiguration.ComparativoMapaSindical, FileMode.Open, FileAccess.Read, FileShare.Read);

            using var wbTemplate = new XLWorkbook(templateFile);

            var primeiraAbaTemplateWs = wbTemplate.Worksheets.FirstOrDefault();

            if (primeiraAbaTemplateWs == null)
            {
                return Result.Failure<byte[]>("Primeira aba da planilha de template não encontrada");
            }

            for (int i = 0; i < documentos.Count; i++)
            {
                var documento = documentos[i];
                var column = NUMERO_COLUNA_INICIAL + i;

                primeiraAbaTemplateWs.Cell(1, column).SetValue($"Negociação {column - 1}");
                primeiraAbaTemplateWs.Cell(1, column).Style.Fill.SetBackgroundColor(XLColor.DarkBlue);
                primeiraAbaTemplateWs.Cell(1, column).Style.Font.SetFontColor(XLColor.White);
                primeiraAbaTemplateWs.Cell(1, column).Style.Font.SetBold();
                primeiraAbaTemplateWs.Cell(1, column).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                if (documento.Cnaes != null)
                {
                    primeiraAbaTemplateWs.Cell(2, column).SetValue(string.Join(", ", documento.Cnaes.Select(cnae => cnae.Subclasse)));
                }

                if (documento.Abrangencia != null)
                {
                    primeiraAbaTemplateWs.Cell(3, column).SetValue(string.Join(", ", documento.Abrangencia.Select(abrangencia => abrangencia.Uf).Distinct()));
                }

                if (documento.SindicatosLaborais != null)
                {
                    primeiraAbaTemplateWs.Cell(4, column).SetValue(string.Join(", ", documento.SindicatosLaborais.Select(sindicato => sindicato.Sigla).Distinct()));
                }

                if (documento.SindicatosPatronais != null)
                {
                    primeiraAbaTemplateWs.Cell(5, column).SetValue(string.Join(", ", documento.SindicatosPatronais.Select(sindicato => sindicato.Sigla).Distinct()));
                }

                if (documento.Estabelecimentos != null)
                {
                    var estabelecimentosUsuario = documento.Estabelecimentos;

                    if (usuarioLogado.Nivel == Nivel.GrupoEconomico)
                    {
                        estabelecimentosUsuario = documento.Estabelecimentos.Where(estabelecimento => usuarioLogado.GrupoEconomicoId.HasValue && estabelecimento.G == usuarioLogado.GrupoEconomicoId.Value)?.ToArray();
                    }

                    if (usuarioLogado.Nivel == Nivel.Matriz || usuarioLogado.Nivel == Nivel.Unidade)
                    {
                        estabelecimentosUsuario = documento.Estabelecimentos.Where(estabelecimento => usuarioLogado.EstabelecimentosIds is not null && estabelecimento.U is not null && usuarioLogado.EstabelecimentosIds.Contains(estabelecimento.U.Value))?.ToArray();
                    }

                    primeiraAbaTemplateWs.Cell(6, column).SetValue(estabelecimentosUsuario?.Length ?? 0);
                }
                else
                {
                    primeiraAbaTemplateWs.Cell(6, column).SetValue(0);
                }

                if (!string.IsNullOrEmpty(documento.Database))
                {
                    primeiraAbaTemplateWs.Cell(7, column).SetValue(documento.Database);
                }

                if (documento.ValidadeInicial.HasValue && documento.ValidadeFinal.HasValue)
                {
                    primeiraAbaTemplateWs.Cell(8, column).SetValue($"{documento.ValidadeInicial.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)} até {documento.ValidadeFinal.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)}");
                }

                if (documento.IndiceProjetado > 0)
                {
                    primeiraAbaTemplateWs.Cell(9, column).SetValue(documento.IndiceProjetado);
                    primeiraAbaTemplateWs.Cell(9, column).Style.NumberFormat.SetFormat("0.00\\%");
                }

                if (!string.IsNullOrEmpty(documento.DocumentoNome))
                {
                    primeiraAbaTemplateWs.Cell(10, column).SetValue(documento.DocumentoNome);
                }

                primeiraAbaTemplateWs.Column(column).Cells(1, NUMERO_LINHA_INFORMACOES_ADICIONAIS_INICIAL - 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            }

            var documentosEstuturas = MontarEstruturasPorDocumento(documentos, clausulas);

            if (!(documentosEstuturas?.Any() ?? false))
            {
                return Result.Failure<byte[]>("Não há dados processados para a seleção efetuada");
            }

            var linhasEstruturaInformacaoAdicional = MontarLinhaEstruturaInformacacaoAdicional(documentosEstuturas);

            if (!(linhasEstruturaInformacaoAdicional?.Any() ?? false))
            {
                return Result.Failure<byte[]>("Não foi possível mapear as linhas por informação adicional");
            }

            foreach (var documentoEstrutura in documentosEstuturas)
            {
                if (!(documentoEstrutura.Value?.Any() ?? false))
                {
                    continue;
                }

                foreach (var estrutura in documentoEstrutura.Value)
                {
                    var linhaNomeEstrutura = linhasEstruturaInformacaoAdicional.Single(ial => ial.EstruturaId == estrutura.Id && ial.InformacaoAdicionalId is null);

                    primeiraAbaTemplateWs.Cell(linhaNomeEstrutura.Linha, 1).SetValue(estrutura.Nome);
                    primeiraAbaTemplateWs.Cell(linhaNomeEstrutura.Linha, 1).Style.Font.SetFontColor(XLColor.White);
                    primeiraAbaTemplateWs.Cell(linhaNomeEstrutura.Linha, 1).Style.Font.SetBold();

                    primeiraAbaTemplateWs.Row(linhaNomeEstrutura.Linha).Cells(1, documentos.Count + 1).Style.Fill.SetBackgroundColor(XLColor.FromArgb(47, 84, 150));


                    foreach (var informacaoAdicional in estrutura.InformacaoAdicionais!)
                    {
                        if (informacaoAdicional.InformacaoAdicionalId == TipoInformacaoAdicional.NomeClausula.Id)
                        {
                            continue;
                        }

                        var linhaInfoAdicional = linhasEstruturaInformacaoAdicional.Single(ial => ial.EstruturaId == estrutura.Id &&
                                                                                  ial.InformacaoAdicionalId == informacaoAdicional.InformacaoAdicionalId);

                        if (_cellsActions.TryGetValue(informacaoAdicional.InformacaoAdicionalTipoDado, out _))
                        {
                            _cellsActions[informacaoAdicional.InformacaoAdicionalTipoDado].Invoke(primeiraAbaTemplateWs, linhaInfoAdicional.Linha, documentoEstrutura.Key, informacaoAdicional);
                        }

                        primeiraAbaTemplateWs.Cell(linhaInfoAdicional.Linha, 1).SetValue(informacaoAdicional.InformacaoAdicionalNome);
                        primeiraAbaTemplateWs.Cell(linhaInfoAdicional.Linha, 1).Style.Fill.SetBackgroundColor(XLColor.FromArgb(189, 214, 238));
                    }
                }
            }

            var totalLinhas = linhasEstruturaInformacaoAdicional.Max(p => p.Linha);

            for (int i = 1; i <= primeiraAbaTemplateWs.ColumnsUsed().Count(); i++)
            {
                primeiraAbaTemplateWs.Column(i).Cells(1, totalLinhas).Style.Border.SetOutsideBorder(XLBorderStyleValues.Dotted);
                primeiraAbaTemplateWs.Column(i).Cells(1, totalLinhas).Style.Border.SetInsideBorder(XLBorderStyleValues.Dotted);
            }

            using var stream = new MemoryStream();

            wbTemplate.SaveAs(stream);

            return Result.Success(stream.ToArray());
        }

        private static IEnumerable<InformacaoAdicionalLinha> MontarLinhaEstruturaInformacacaoAdicional(Dictionary<int, List<EstruturaClausula>> documentosEstruturasClausulas)
        {
            var gruposEstruturas = from iaft in documentosEstruturasClausulas.SelectMany(ect => ect.Value)
                                   orderby iaft.Nome
                                   group iaft by iaft.Id into _iaft
                                   select new
                                   {
                                       Id = _iaft.Key,
                                       _iaft.First().Nome,
                                       InformacoesAdicionaisIds = _iaft.SelectMany(iaft => iaft.InformacaoAdicionais!)
                                                                    .OrderBy(iaft => iaft.Sequencia)
                                                                    .Select(iaft => iaft.InformacaoAdicionalId)
                                                                    .Distinct()
                                   };

            var numeroLinha = NUMERO_LINHA_INFORMACOES_ADICIONAIS_INICIAL;

            foreach (var grupoEstrutura in gruposEstruturas)
            {
                yield return new InformacaoAdicionalLinha { EstruturaId = grupoEstrutura.Id, Linha = numeroLinha };
                numeroLinha++;

                foreach (var infoAdicional in grupoEstrutura.InformacoesAdicionaisIds)
                {
                    if (infoAdicional == TipoInformacaoAdicional.NomeClausula.Id)
                    {
                        continue;
                    }

                    yield return new InformacaoAdicionalLinha { EstruturaId = grupoEstrutura.Id, InformacaoAdicionalId = infoAdicional, Linha = numeroLinha };
                    numeroLinha++;
                }
            }
        }

        private static Dictionary<int, List<EstruturaClausula>> MontarEstruturasPorDocumento(IReadOnlyList<ComparativoMapaSindicalPrincipalVw> documentos, IReadOnlyList<ComparativoMapaSindicalItemVw> clausulas)
        {
            var colunaEstruturaClausula = new Dictionary<int, List<EstruturaClausula>>();
           

            for (int i = 0; i < documentos.Count; i++)
            {
                var documento = documentos[i];
                var column = NUMERO_COLUNA_INICIAL + i;

                var clausulasDuplicadas = from cls in clausulas
                                          where cls.InformacaoAdicionalId == TipoInformacaoAdicional.NomeClausula.Id
                                          && cls.DocumentoSindicalId == documento.DocumentoId
                                          group cls by cls.ClausulaInformacaoNumero into _cls
                                          select new
                                          {
                                              _cls.Key,
                                              Count = _cls.DistinctBy(cls => cls.GrupoDados).Count()
                                          };
                

                IEnumerable<int> clausulasEstruturasNumero = clausulas.Where(clausula => clausula.DocumentoSindicalId == documento.DocumentoId
                                                                  && clausula.ExibeComparativoMapaSindical
                                                                  && clausula.InformacaoAdicionalId == TipoInformacaoAdicional.NomeClausula.Id)
                                                  .OrderBy(clausula => clausula.ClausulaInformacaoNumero)
                                                  .ThenBy(clausula => clausula.GrupoDados)
                                                  .Select(clausula => clausula.ClausulaInformacaoNumero)
                                                  .Distinct();

                foreach (var clausulaEstruturaNumero in clausulasEstruturasNumero)
                {
                    var clausulasEstrutura = clausulas.Where(clausula => clausula.DocumentoSindicalId == documento.DocumentoId
                                                                  && clausula.ExibeComparativoMapaSindical
                                                                  && clausula.ClausulaInformacaoNumero == clausulaEstruturaNumero)
                                                  .OrderBy(clausula => clausula.ClausulaInformacaoNumero)
                                                  .ThenBy(clausula => clausula.GrupoDados)
                                                  .ToList();

                    var estrutura = clausulasEstrutura.First(clausula => clausula.InformacaoAdicionalId == TipoInformacaoAdicional.NomeClausula.Id);

#pragma warning disable S6605 // Collection-specific "Exists" method should be used instead of the "Any" extension
                    if (!clausulasEstrutura.Any(cet => cet.InformacaoAdicionalId == TipoInformacaoAdicional.Comparativo.Id && cet.ValorCombo is not null && cet.ValorCombo.ToLowerInvariant() == "sim"))
                    {
                        continue;
                    }
#pragma warning restore S6605 // Collection-specific "Exists" method should be used instead of the "Any" extension

                    if (clausulasDuplicadas.Any(cdt => cdt.Key == clausulaEstruturaNumero && cdt.Count > 1))
                    {
                        var clausulaComparativa = clausulasEstrutura
                            .Where(cet => cet.InformacaoAdicionalId == TipoInformacaoAdicional.Comparativo.Id && cet.ValorCombo is not null && cet.ValorCombo.ToLowerInvariant() == "sim")
                            .Select(cet => new
                            {
                                cet.ClausulaId,
                                GrupoDados = cet.GrupoDados
                            })
                            .FirstOrDefault();

                        if (clausulaComparativa is null)
                        {
                            continue;
                        }


                        clausulasEstrutura = clausulasEstrutura.Where(cet => cet.ClausulaId == clausulaComparativa.ClausulaId && cet.GrupoDados == clausulaComparativa.GrupoDados)
                                                .ToList();
                    }

                    var estruturaClausulaDto = new EstruturaClausula
                    {
                        Id = estrutura.ClausulaInformacaoNumero,
                        Nome = estrutura.ValorCombo ?? "",
                        InformacaoAdicionais = clausulasEstrutura.Where(clausula => clausula.InformacaoAdicionalId != TipoInformacaoAdicional.NomeClausula.Id &&
                                                                           clausula.InformacaoAdicionalId != TipoInformacaoAdicional.Comparativo.Id)
                                                       .OrderBy(clausula => clausula.Sequencia)
                    };
#pragma warning disable CA1854

                    if (colunaEstruturaClausula.ContainsKey(column))
                    {
                        colunaEstruturaClausula[column].Add(estruturaClausulaDto);
                        continue;
                    }

                    colunaEstruturaClausula.Add(column, new List<EstruturaClausula> { estruturaClausulaDto });

#pragma warning disable CA1854

                }
            }

            return colunaEstruturaClausula;
        }

        public record EstruturaClausula
        {
            public int Id { get; init; }
            public string Nome { get; init; } = null!;
            public IEnumerable<ComparativoMapaSindicalItemVw>? InformacaoAdicionais { get; set; }
        }

        public record InformacaoAdicionalLinha
        {
            public int Linha { get; init; }
            public int EstruturaId { get; init; }
            public int? InformacaoAdicionalId { get; init; }
        }
    }
}
