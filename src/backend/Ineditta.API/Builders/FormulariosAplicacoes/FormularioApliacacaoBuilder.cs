using ClosedXML.Excel;

using CSharpFunctionalExtensions;

using Ineditta.API.Configurations;
using Ineditta.API.Factories.Clausulas;
using Ineditta.Application.Clausulas.Entities.InformacoesAdicionais;
using Ineditta.BuildingBlocks.Core.Domain.Models;
using Ineditta.Repository.Contexts;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

using Ineditta.BuildingBlocks.Core.Extensions;
using System.Globalization;
using Ineditta.API.Builders.Worksheets;
using Ineditta.API.Factories.InformacoesAdicionais.Cliente;
using Ineditta.Application.Usuarios.Entities;
using Ineditta.API.ViewModels.DocumentosSindicais.ViewModels.Exel;
using Ineditta.API.Constants;
using Ineditta.BuildingBlocks.Core.Domain.ValueObjects;
using Ineditta.Application.Sindicatos.Base.ValueObjects;
using Ineditta.BuildingBlocks.Core.Files.Images;
using Ineditta.API.ViewModels.InformacoesAdicionaisClientes.ViewModels;
using Ineditta.API.ViewModels.ClausulasGerais.ViewModels;
using Ineditta.Application.Documentos.Sindicais.Dtos;
using Ineditta.Application.Documentos.Sindicais.Factories;
using Ineditta.Repository.Documentos.Sindicais.Views.DocumentosSindicatosEstabelecimentosVw;

namespace Ineditta.API.Builders.FormulariosAplicacoes
{
    public class FormularioApliacacaoBuilder
    {
        private const string COR_AZUL = "#366092";
        private const int BREAK_LINE = 16;
        private readonly InedittaDbContext _context;
        private readonly DocumentoSindicalClausulaFactory _documentoSindicalClausulaFactory;
        private readonly TemplateExcelConfiguration _templateExcelConfiguration;
        private readonly InformacoesAdicionaisClienteFactory _informacoesAdicionaisClienteFactory;
        private readonly GerarLinkArquivoFactory _gerarLinkArquivoFactory;

        public FormularioApliacacaoBuilder(InedittaDbContext inedittaDbContext, DocumentoSindicalClausulaFactory documentoSindicalClausulaFactory, IOptions<TemplateExcelConfiguration> templateExcelConfiguration, InformacoesAdicionaisClienteFactory informacoesAdicionaisClienteFactory, GerarLinkArquivoFactory gerarLinkArquivoFactory)
        {
            _context = inedittaDbContext;
            _documentoSindicalClausulaFactory = documentoSindicalClausulaFactory;
            _templateExcelConfiguration = templateExcelConfiguration?.Value ?? throw new ArgumentNullException(nameof(templateExcelConfiguration));
            _informacoesAdicionaisClienteFactory = informacoesAdicionaisClienteFactory;
            _gerarLinkArquivoFactory = gerarLinkArquivoFactory;
        }

        public async ValueTask<Result<byte[], Error>> HandleAsync(int documentoId, Usuario usuario, CancellationToken cancellationToken = default)
        {
            var documentoClausulas = await _documentoSindicalClausulaFactory.CriarAsync(documentoId);

            if (documentoClausulas is null)
            {
                return Result.Failure<byte[], Error>(Errors.Http.NotFound());
            }

            InformacaoAdicionalClienteViewModel? informacoesAdicionaisCliente = null;

            if (usuario.Nivel != Nivel.Ineditta)
            {
                informacoesAdicionaisCliente = await _informacoesAdicionaisClienteFactory.CriarAsync(documentoId, usuario);
            }

            List<DocumentoSindicatoEstabelecimentoVw>? documentoEstabelecimentos = null;

            if (usuario.Nivel == Nivel.Ineditta)
            {
                documentoEstabelecimentos = await _context.DocumentoSindicatoEstabelecimentoVw
                    .AsNoTracking()
                    .Where(p => p.DocumentoId == documentoId)
                    .ToListAsync(cancellationToken);
            }
            else
            {
                documentoEstabelecimentos = await _context.DocumentoSindicatoEstabelecimentoVw
                    .AsNoTracking()
                    .Where(p => p.DocumentoId == documentoId && p.GrupoEconomicoId == usuario.GrupoEconomicoId)
                    .ToListAsync(cancellationToken);
            }

            if (documentoEstabelecimentos is null)
            {
                return Result.Failure<byte[], Error>(Errors.Http.NotFound());
            }

            var documentoEstabelecimento = MergeDocumentosDataFactory(documentoEstabelecimentos);
            var clausulas = MergeInformacoesAdicionaisFactory(documentoClausulas, informacoesAdicionaisCliente);
            clausulas = clausulas.OrderBy(c => c.Numero);

            var tabela = FormatarDadosTabelaFactory(clausulas);

            var exel = Criar(documentoEstabelecimento, tabela, informacoesAdicionaisCliente);

            if (exel.IsFailure)
            {
                return exel;
            }

            return exel;
        }

        private Result<byte[], Error> Criar(
            DocumentoSindicatoEstabelecimentoViewModel documento,
            IEnumerable<TabelaClausulasInformacoesAdicionaisViewModel> tabela,
            InformacaoAdicionalClienteViewModel? informacoesCliente
        )
        {
            XLColor blue = XLColor.FromHtml(COR_AZUL);
            var tipoDadoDicionario = ObterTipoCampoDado();
            var currentLine = 36;
            var initialColumn = 3;

            using var workbook = WorksheetBuilder.ReadFrom(_templateExcelConfiguration.FormularioAplicacao);

            var fileBytes = workbook
                .GetFirst()
                .Build(worksheet =>
                {
                    var logo = $"{StaticFile.Path}images/ineditta-logo.png";

                    worksheet.AddPicture(logo)
                        .MoveTo(worksheet.Cell(2, 3))
                        .WithSize(150, 60);

                    if (!string.IsNullOrEmpty(documento.GrupoEconomicoLogo))
                    {
                        using var url = GerarLogoFactory.ToMemoryUrl(documento.GrupoEconomicoLogo);

                        if (url != null)
                        {
                            using var image = ImageManager.Crop(url, 0.33m, (imageProperties) => (imageProperties.Width == imageProperties.Height) || (imageProperties.Width / imageProperties.Height < 2));

                            worksheet.AddPicture(image)
                                .MoveTo(worksheet.Cell(11, 3))
                                .WithSize(430, 180);
                            worksheet.Cell(11, 3).SetValue("");
                        }
                    }

                    worksheet.Cell(11, 8).SetValue(DateTime.Now);
                    worksheet.Cell(11, 8).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                    worksheet.Cell(13, 8).SetValue(string.Join(", ", documento.GrupoEconomicoNomes));
                    worksheet.Cell(15, 8).SetValue(string.Join(", ", documento.MatrizNomes));

                    if (documento?.EstabelecimentosNomes != null)
                    {
                        worksheet.Cell(17, 8).SetValue(string.Join(", ", documento.EstabelecimentosNomes));
                    }

                    if (documento?.EstabelecimentoCodigo != null)
                    {
                        worksheet.Cell(19, 8).SetValue(string.Join(", ", documento.EstabelecimentoCodigo));
                    }

                    if (documento?.EstabelecimentoCodigosSindicatosLaborais != null)
                    {
                        worksheet.Cell(21, 6).SetValue(string.Join(", ", documento.EstabelecimentoCodigosSindicatosLaborais));
                    }

                    var sindicatosLaborais = documento?.DocumentoSindicatosLaborais?.Select(s => s.Sigla + " - " + CNPJ.Formatar(s.Cnpj!));
                    if (sindicatosLaborais != null)
                    {
                        worksheet.Cell(21, 10).SetValue(string.Join(", ", sindicatosLaborais.Distinct()));
                    }

                    var sindicatosLaboraisCodigosDoc = documento?.DocumentoSindicatosLaborais?.Select(s => s.Codigo);
                    if (sindicatosLaboraisCodigosDoc != null)
                    {
                        worksheet.Cell(23, 6).SetValue(string.Join(", ", sindicatosLaboraisCodigosDoc.Select(c => CodigoSindical.Formatar(c!)).Distinct()));
                    }

                    if (documento?.EstabelecimentoCodigosSindicatosPatronais != null)
                    {
                        worksheet.Cell(25, 6).SetValue(string.Join(", ", documento.EstabelecimentoCodigosSindicatosPatronais));
                    }

                    var sindicatosPatronais = documento?.DocumentoSindicatosPatronais?.Select(s => s.Sigla + " - " + CNPJ.Formatar(s.Cnpj!));
                    if (sindicatosPatronais != null)
                    {
                        worksheet.Cell(25, 10).SetValue(string.Join(", ", sindicatosPatronais.Distinct()));
                    }

                    var sindicatosPatronaisCodigoDoc = documento?.DocumentoSindicatosPatronais?.Select(s => s.Codigo);
                    if (sindicatosPatronaisCodigoDoc != null)
                    {
                        worksheet.Cell(27, 6).SetValue(string.Join(", ", sindicatosPatronaisCodigoDoc.Select(c => CodigoSindical.Formatar(c!)).Distinct()));
                    }

                    worksheet.Cell(29, 6).SetValue(documento?.DocumentoNome);
                    worksheet.Cell(29, 12).SetValue(documento?.DocumentoVersao);

                    var dataAssinatura = documento?.DocumentoDataAssinatura;

                    if (dataAssinatura != null && (dataAssinatura.Value > DateOnly.Parse("1900-01-01", CultureInfo.InvariantCulture)))
                    {
                        worksheet.Cell(29, 16).SetValue(dataAssinatura.Value.ToShortDateString());
                    }
                    else
                    {
                        var dataResgitroMte = documento?.DocumentoDataRegistroMte;
                        worksheet.Cell(29, 16).SetValue(dataResgitroMte.HasValue ? dataResgitroMte.Value.ToShortDateString() : "");
                    }


                    worksheet.Cell(31, 6).SetValue(documento?.DocumentoDatabase);

                    var dataVigenciaInicial = documento?.DocumentoVigenciaInicial;
                    object vigenciaInicial = dataVigenciaInicial.HasValue ? dataVigenciaInicial.Value.ToShortDateString() : "";
                    var dataVigenciaFinal = documento?.DocumentoVigenciaFinal;
                    object vigenciaFinal = dataVigenciaFinal.HasValue ? dataVigenciaFinal.Value.ToShortDateString() : "";
                    worksheet.Cell(31, 9).SetValue(vigenciaInicial + " - " + vigenciaFinal);

                    var link = _gerarLinkArquivoFactory.Criar(documento!.DocumentoId);
                    worksheet.Cell(31, 14).SetValue(link);
                    var hyperLink = worksheet.Cell(31, 14).CreateHyperlink();
                    if (link != null)
                    {
                        hyperLink.ExternalAddress = new Uri(link);
                    }
                })
                .Build(worksheet =>
                {
                    worksheet.Cell(34, 1).Style.Fill.SetBackgroundColor(blue);
                    worksheet.Cell(35, 1).Style.Fill.SetBackgroundColor(blue);
                })
                .Build(worksheet =>
                {
                    foreach (var tabelaLinha in tabela)
                    {
                        // Titulo
                        worksheet.Range($"C{currentLine}:Q{currentLine}").Merge();
                        var cell = worksheet.Cell(currentLine, initialColumn);
                        cell.SetValue(tabelaLinha.Titulo);
                        cell.Style.Fill.SetBackgroundColor(blue);
                        cell.Style.Font.SetFontColor(XLColor.White);
                        cell.Style.Font.SetBold();
                        cell.Style.Font.FontSize = 14;
                        cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        currentLine++;

                        if (tabelaLinha?.QuebrasLinhas == null || !tabelaLinha.QuebrasLinhas.Any())
                        {
                            continue;
                        }

                        // Quebra de linhas
                        tabelaLinha.QuebrasLinhas.ToList().ForEach(quebraLinha =>
                        {
                            // Cabecalho
                            var currentColumnTitle = initialColumn;
                            foreach (string cabecalho in quebraLinha.Cabecalho)
                            {
                                worksheet.Cell(currentLine, currentColumnTitle).WorksheetColumn().Width = 30;
                                worksheet.Cell(currentLine, currentColumnTitle).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                                worksheet.Cell(currentLine, currentColumnTitle).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                                worksheet.Cell(currentLine, currentColumnTitle).Style.Alignment.WrapText = true;
                                worksheet.Cell(currentLine, currentColumnTitle).Style.Fill.SetBackgroundColor(blue);
                                worksheet.Cell(currentLine, currentColumnTitle).Style.Font.SetFontColor(XLColor.White);
                                worksheet.Cell(currentLine, currentColumnTitle).Style.Font.SetBold();
                                worksheet.Row(currentLine).Height = 30;

                                worksheet.Cell(currentLine, currentColumnTitle).SetValue(cabecalho);

                                currentColumnTitle++;
                            }

                            currentLine++;

                            // Linhas
                            foreach (var linha in quebraLinha.Linhas)
                            {
                                if (linha?.InformacoesAdicionais == null || !linha.InformacoesAdicionais.Any())
                                {
                                    continue;
                                }

                                var currentColumnDado = initialColumn;
                                foreach (var informacaoAdicional in linha.InformacoesAdicionais)
                                {
                                    worksheet.Cell(currentLine, currentColumnDado).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                                    worksheet.Cell(currentLine, currentColumnDado).Style.Border.OutsideBorderColor = XLColor.Black;
#pragma warning disable CA1854
                                    if (informacaoAdicional.Tipo.TryParseToEnum<TipoDado>(out var tipo) && tipo is not null && tipoDadoDicionario.ContainsKey(tipo.Value))
                                    {
                                        tipoDadoDicionario[tipo.Value].Invoke(worksheet, currentLine, currentColumnDado, informacaoAdicional);
                                    }

                                    currentColumnDado++;
                                }

                                currentLine++;
                            }


                            currentLine++;
                        });

                        if (informacoesCliente?.ObservacoesAdicionais != null && informacoesCliente.ObservacoesAdicionais.Any())
                        {
                            informacoesCliente?.ObservacoesAdicionais
                            .ToList().ForEach(o =>
                            {
                                if (o.ClausulaId == tabelaLinha.Id)
                                {
                                    string texto = "Regras parâmetros para empresa";
                                    worksheet.Cell(currentLine, initialColumn).SetValue(texto.ToUpper(CultureInfo.CurrentCulture));
                                    worksheet.Cell(currentLine, initialColumn).Style.Font.Bold = true;
                                    worksheet.Cell(currentLine, initialColumn).Style.Font.FontColor = blue;
                                    worksheet.Cell(currentLine, initialColumn).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                                    currentLine++;

                                    worksheet.Range($"C{currentLine}:Q{currentLine}").Merge();
                                    worksheet.Cell(currentLine, initialColumn).WorksheetRow().Height = 50;
                                    worksheet.Cell(currentLine, initialColumn).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Justify;
                                    worksheet.Cell(currentLine, initialColumn).Style.Alignment.Vertical = XLAlignmentVerticalValues.Justify;
                                    worksheet.Cell(currentLine, initialColumn).SetValue(o.Valor);
                                    worksheet.Cell(currentLine, initialColumn).Style.Font.FontColor = XLColor.Black;

                                    var mergedRange = worksheet.Range(currentLine, initialColumn, currentLine, initialColumn + 14);
                                    mergedRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                                    mergedRange.Style.Border.OutsideBorderColor = XLColor.Black;

                                    currentLine += 2;
                                }
                            });
                        }

                        currentLine++;
                    }
                })
                .Build(worksheet =>
                {
                    if (informacoesCliente?.OutrasInformacoes is not null && informacoesCliente.OutrasInformacoes.Length > 0)
                    {
                        var value = "Outras Informações Referentes ao documento";
                        worksheet.Cell(currentLine, initialColumn).SetValue(value.ToUpper(CultureInfo.CurrentCulture));
                        worksheet.Cell(currentLine, initialColumn).Style.Font.FontColor = XLColor.Blue;
                        worksheet.Cell(currentLine, initialColumn).Style.Font.Bold = true;
                        worksheet.Cell(currentLine, initialColumn).Style.Font.Underline = XLFontUnderlineValues.Single;
                        worksheet.Cell(currentLine, initialColumn).Style.Font.FontSize = 14;

                        currentLine += 2;

                        worksheet.Range($"C{currentLine}:Q{currentLine}").Merge();
                        worksheet.Cell(currentLine, initialColumn).WorksheetRow().Height = 100;
                        worksheet.Cell(currentLine, initialColumn).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        worksheet.Cell(currentLine, initialColumn).Style.Alignment.Vertical = XLAlignmentVerticalValues.Justify;
                        worksheet.Cell(currentLine, initialColumn).SetValue(informacoesCliente.OutrasInformacoes);

                        var mergedRange = worksheet.Range(currentLine, initialColumn, currentLine, initialColumn + 14);
                        mergedRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        mergedRange.Style.Border.OutsideBorderColor = XLColor.Black;

                        currentLine += 2;
                    }
                })
                .Build(worksheet =>
                {
                    var mergedBottom = worksheet.Range(currentLine, 1, currentLine, 17);
                    mergedBottom.Style.Fill.SetBackgroundColor(blue);

                    var mergedRight = worksheet.Range(1, 18, currentLine, 18);
                    mergedRight.Style.Fill.SetBackgroundColor(blue);

                    var mergedLeft = worksheet.Range(1, 1, currentLine, 1);
                    mergedLeft.Style.Fill.SetBackgroundColor(blue);
                })
                .Build(worksheet =>
                {
                    foreach (var row in worksheet.RowsUsed())
                    {
                        foreach (var cell in row.CellsUsed())
                        {
                            if (cell.HasComment)
                            {
                                var comment = cell.GetComment();
                                comment?.Delete();
                            }
                        }
                    }
                })
                .ToByteArray(workbook);

            return Result.Success<byte[], Error>(fileBytes);
        }

        private static DocumentoSindicatoEstabelecimentoViewModel MergeDocumentosDataFactory(List<DocumentoSindicatoEstabelecimentoVw> documentos)
        {
            var gruposEconomicosIds = new List<int>();
            var gruposEconomicosNomes = new List<string>();
            var matrizIds = new List<int>();
            var matrizNomes = new List<string>();
            var estabelecimentosIds = new List<int>();
            var estabelecimentosNomes = new List<string>();
            var estabelecimentoCodigosSindicatosLaborais = new List<string>();
            var estabelecimentoCodigosSindicatosPatronais = new List<string>();
            var estabelecimentoCodigo = new List<string>();
            var documentoSindicatosLaborais = new List<SindicatoLaboral>();
            var documentoSindicatosPatronais = new List<SindicatoPatronal>();
            var estabelecimentoSindicatosLaborais = new List<EstabelecimentoSindicatoViewModel>();
            var estabelecimentoSindicatosPatronais = new List<EstabelecimentoSindicatoViewModel>();

            foreach (var documento in documentos)
            {
                gruposEconomicosIds.Add(documento.GrupoEconomicoId);
                gruposEconomicosNomes.Add(documento.GrupoEconomicoNome);
                matrizIds.Add(documento.MatrizId);
                matrizNomes.Add(documento.MatrizNome);
                estabelecimentosIds.Add(documento.EstabelecimentoId);

                if (documento.EstabelecimentoNome is not null)
                {
                    estabelecimentosNomes.Add(documento.EstabelecimentoNome);
                }

                if (documento.EstabelecimentoCodigoSindicatoLaboral is not null)
                {
                    estabelecimentoCodigosSindicatosLaborais.Add(documento.EstabelecimentoCodigoSindicatoLaboral);
                }

                if (documento.EstabelecimentoCodigoSindicatoPatronal is not null)
                {
                    estabelecimentoCodigosSindicatosPatronais.Add(documento.EstabelecimentoCodigoSindicatoPatronal);
                }

                if (documento.EstabelecimentoCodigo is not null)
                {
                    estabelecimentoCodigo.Add(documento.EstabelecimentoCodigo);
                }

                if (documento.DocumentoSindicatosLaborais is not null)
                {
                    documentoSindicatosLaborais.AddRange(documento.DocumentoSindicatosLaborais);
                }

                if (documento.DocumentoSindicatosPatronais is not null)
                {
                    documentoSindicatosPatronais.AddRange(documento.DocumentoSindicatosPatronais);
                }

                if (documento.EstabelecimentoSindicatosLaborais is not null)
                {
                    estabelecimentoSindicatosLaborais.AddRange(documento.EstabelecimentoSindicatosLaborais);
                }

                if (documento.EstabelecimentoSindicatosPatronais is not null)
                {
                    estabelecimentoSindicatosPatronais.AddRange(documento.EstabelecimentoSindicatosPatronais);
                }
            }

            return new DocumentoSindicatoEstabelecimentoViewModel
            {
                DocumentoId = documentos[0].DocumentoId,
                GrupoEconomicoIds = gruposEconomicosIds.Distinct(),
                GrupoEconomicoNomes = gruposEconomicosNomes.Distinct(),
                MatrizIds = matrizIds.Distinct(),
                MatrizNomes = matrizNomes.Distinct(),
                EstabelecimentosIds = estabelecimentosIds.Distinct(),
                EstabelecimentosNomes = estabelecimentosNomes.Distinct(),
                EstabelecimentoCodigosSindicatosLaborais = estabelecimentoCodigosSindicatosLaborais.Distinct(),
                EstabelecimentoCodigosSindicatosPatronais = estabelecimentoCodigosSindicatosPatronais.Distinct(),
                DocumentoTitulo = documentos[0].DocumentoTitulo,
                DocumentoNome = documentos[0].DocumentoNome,
                DocumentoVersao = documentos[0].DocumentoVersao,
                DocumentoDatabase = documentos[0].DocumentoDatabase,
                DocumentoVigenciaInicial = documentos[0].DocumentoVigenciaInicial,
                DocumentoVigenciaFinal = documentos[0].DocumentoVigenciaFinal,
                GrupoEconomicoLogo = documentos[0].GrupoEconomicoLogo,
                EstabelecimentoCodigo = estabelecimentoCodigo.Distinct(),
                DocumentoDataAssinatura = documentos[0].DocumentoDataAssinatura,
                DocumentoCaminhoArquivo = documentos[0].DocumentoCaminhoArquivo,
                DocumentoDataRegistroMte = documentos[0].DocumentoDataRegistroMte,
                DocumentoSindicatosLaborais = documentoSindicatosLaborais.Distinct(),
                DocumentoSindicatosPatronais = documentoSindicatosPatronais.Distinct(),
                EstabelecimentoSindicatosLaborais = estabelecimentoSindicatosLaborais.Distinct(),
                EstabelecimentoSindicatosPatronais = estabelecimentoSindicatosPatronais.Distinct()
            };
        }

        private static IEnumerable<DocumentoSindicalClausulaVw> MergeInformacoesAdicionaisFactory(IEnumerable<DocumentoSindicalClausulaVw> clausulas, InformacaoAdicionalClienteViewModel? informacoesAdicionaisCliente)
        {
            clausulas = clausulas.Select(clausula =>
            {
                if (clausula.Grupos == null || !clausula.Grupos.Any()) return clausula;

                clausula.Grupos = clausula.Grupos.Select(linha =>
                {
                    if (linha.InformacoesAdicionais == null || !linha.InformacoesAdicionais.Any()) return linha;

                    linha.InformacoesAdicionais = linha.InformacoesAdicionais.Select(item =>
                    {
                        if (informacoesAdicionaisCliente is not null)
                        {
                            informacoesAdicionaisCliente.InformacoesAdicionais?.ToList().ForEach(itemCliente =>
                            {
                                if (item.Id == itemCliente.ClausulaGeralEstruturaId)
                                {
                                    item.Dado.Hora = itemCliente.Valor;
                                    item.Dado.Descricao = itemCliente.Valor;
                                    item.Dado.Data = itemCliente.Valor;
                                    item.Dado.Nome = itemCliente.Valor;
                                    if (decimal.TryParse(itemCliente.Valor, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal numerico))
                                    {
                                        item.Dado.Numerico = numerico;
                                        item.Dado.Percentual = numerico;
                                    }
                                    item.Dado.Texto = itemCliente.Valor;

                                    List<string> valor = new();

                                    if (itemCliente.Valor is not null && itemCliente.Valor.Length > 2)
                                    {
                                        valor = itemCliente.Valor.Split(", ").ToList();
                                    }

                                    item.Dado.Combo = new Combo
                                    {
                                        Opcoes = item?.Dado?.Combo?.Opcoes,
                                        Valor = valor
                                    };
                                }
                            });
                        }

                        return item;
                    });

                    return linha;
                });

                return clausula;
            });

            return clausulas;
        }

        private static IEnumerable<TabelaClausulasInformacoesAdicionaisViewModel> FormatarDadosTabelaFactory(IEnumerable<DocumentoSindicalClausulaVw> clausulas)
        {
            var tabela = new List<TabelaClausulasInformacoesAdicionaisViewModel>();

            clausulas
                .ToList()
                .ForEach(clausula =>
                {
                    if (clausula.Grupos == null || !clausula.Grupos.Any())
                    {
                        return;
                    }

                    var quebrasLinhas = new List<QuebraLinha>();

                    var titulo = "Cláusula: " + clausula.Numero + " - " + clausula.NomeEstruraClausula + " | Grupo: " + clausula.NomeGrupoClausula;

                    var numeroQuebraLinhas = 0;

                    numeroQuebraLinhas = CalcularQuebraLinha(clausula.Grupos);

                    for (var i = 0; i < numeroQuebraLinhas; i++)
                    {
                        var grupos = clausula.Grupos;
                        var informacoesAdicionais = grupos.ToList()[0].InformacoesAdicionais;

                        if (informacoesAdicionais == null)
                        {
                            continue;
                        }

                        IEnumerable<string> cabecalho = CriarCabecalhoQuebraLinha(informacoesAdicionais.ToList(), i);
                        IEnumerable<Linha> linhas = CriarLinhas(grupos, i);

                        var novaQuebraLinha = new QuebraLinha
                        {
                            Cabecalho = cabecalho,
                            Linhas = linhas
                        };

                        quebrasLinhas.Add(novaQuebraLinha);
                    }

                    var novaTabelaLinha = new TabelaClausulasInformacoesAdicionaisViewModel
                    {
                        Id = clausula?.Id ?? 0,
                        Titulo = titulo,
                        QuebrasLinhas = quebrasLinhas
                    };

                    tabela.Add(novaTabelaLinha);
                });

            return tabela;

            static IEnumerable<Linha> CriarLinhas(IEnumerable<Grupos> grupos, int quebraAtual)
            {
                List<Linha> linhas = new();

                foreach (var grupo in grupos)
                {
                    var novaLinha = CriarLinha(grupo, quebraAtual);

                    if (novaLinha != null)
                    {
                        linhas.Add(novaLinha);
                    }
                }

                static Linha? CriarLinha(Grupos grupo, int quebraAtual)
                {
                    var primeiraPosicao = quebraAtual * BREAK_LINE; // 0 // 12 // 24
                    var ultimaPosicao = ((quebraAtual + 1) * BREAK_LINE) - 1; // 11 // 23 // 35

                    if (grupo.InformacoesAdicionais == null || !grupo.InformacoesAdicionais.Any())
                    {
                        return null;
                    }

                    var informacoesAdicionais = grupo.InformacoesAdicionais.ToList();
                    List<InformacaoAdicionalSisap> novasInformacoesAdicionais = new();

                    if (ultimaPosicao > informacoesAdicionais.Count)
                    {
                        ultimaPosicao = informacoesAdicionais.Count;
                    }

                    for (var i = primeiraPosicao; i < ultimaPosicao; i++)
                    {
                        novasInformacoesAdicionais.Add(informacoesAdicionais[i]);
                    }

                    var linha = new Linha
                    {
                        InformacoesAdicionais = novasInformacoesAdicionais
                    };

                    return linha;
                }

                return linhas;
            }

            static IEnumerable<string> CriarCabecalhoQuebraLinha(List<InformacaoAdicionalSisap> informacoesAdicionais, int quebraAtual)
            {
                List<string> cabecalho = new();

                // Gerar cabecalhos de acordo com as quebras de linhas - 12 cabecalhos por linha
                // Posições de acordo com a quebra atual
                var primeiraPosicao = quebraAtual * BREAK_LINE; // 0 // 12 // 24
                var ultimaPosicao = ((quebraAtual + 1) * BREAK_LINE) - 1; // 11 // 23 // 35

                if (ultimaPosicao > informacoesAdicionais.Count)
                {
                    ultimaPosicao = informacoesAdicionais.Count;
                }

                for (var i = primeiraPosicao; i < ultimaPosicao; i++)
                {
                    cabecalho.Add(informacoesAdicionais[i]?.Descricao ?? "");
                }

                return cabecalho;
            }

            static int CalcularQuebraLinha(IEnumerable<Grupos> grupos)
            {
                var quebras = 0;
                List<Grupos> grupoList = grupos.ToList();

                var grupo = grupoList[0];

                if (grupo.InformacoesAdicionais == null || !grupo.InformacoesAdicionais.Any())
                {
                    return quebras;
                }

                var informacoesAdicionaisCount = grupo.InformacoesAdicionais.Count();

                quebras = (int)Math.Ceiling((double)informacoesAdicionaisCount / BREAK_LINE);

                return quebras;
            }
        }

        private static Dictionary<TipoDado, Action<IXLWorksheet, int, int, InformacaoAdicionalSisap>> ObterTipoCampoDado()
#pragma warning restore S3963 // "static" fields should be initialized inline
        {
            return new Dictionary<TipoDado, Action<IXLWorksheet, int, int, InformacaoAdicionalSisap>>
            {
                {
                    TipoDado.Combo,
                    (ws, row, column, info) =>
                    {
                        ws.Cell(row, column).SetValue(string.Join(", ", info?.Dado?.Combo?.Valor ?? Enumerable.Empty<string>()));
                        ws.Cell(row, column).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Justify;
                        ws.Cell(row, column).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        ws.Cell(row, column).Style.Alignment.WrapText = true;
                        ws.Column(column).Width = 30;
                    }
                },
                {
                    TipoDado.ComboMultipla,
                    (ws, row, column, info) =>
                    {
                        ws.Cell(row, column).SetValue(string.Join(", ", info?.Dado?.Combo?.Valor ?? Enumerable.Empty<string>()));
                        ws.Cell(row, column).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Justify;
                        ws.Cell(row, column).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        ws.Cell(row, column).Style.Alignment.WrapText = true;
                        ws.Column(column).Width = 30;
                    }
                },
                {
                    TipoDado.Descricao,
                    (ws, row, column, info) =>
                    {
                        ws.Cell(row, column).SetValue(info.Dado.Descricao);
                        ws.Cell(row, column).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Justify;
                        ws.Cell(row, column).Style.Alignment.Vertical = XLAlignmentVerticalValues.Justify;
                        ws.Cell(row, column).Style.Alignment.WrapText = true;
                        ws.Column(column).Width = 30;
                        ws.Row(row).Height = 50;
                    }
                },
                {
                    TipoDado.Texto,
                    (ws, row, column, info) =>
                    {
                        ws.Cell(row, column).SetValue(info.Dado.Texto);
                        ws.Cell(row, column).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Justify;
                        ws.Cell(row, column).Style.Alignment.Vertical = XLAlignmentVerticalValues.Justify;
                        ws.Cell(row, column).Style.Alignment.WrapText = true;
                        ws.Column(column).Width = 30;
                    }
                },
                {
                    TipoDado.Data,
                    (ws, row, column, info) =>
                    {
                        if (info?.Dado?.Data is not null && DateOnly.TryParse(info.Dado.Data, CultureInfo.CurrentCulture, out DateOnly data))
                        {
                            ws.Cell(row, column).SetValue(data.ToShortDateString());
                            ws.Cell(row, column).Style.DateFormat.Format = "dd/MM/yyyy";
                            ws.Cell(row, column).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Justify;
                            ws.Cell(row, column).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                            ws.Cell(row, column).Style.Alignment.WrapText = true;
                            ws.Column(column).Width = 30;
                        }
                    }
                },
                {
                    TipoDado.Percentual,
                    (ws, row, column, info) =>
                    {
                        ws.Cell(row, column).SetValue(info.Dado.Percentual);
                        ws.Cell(row, column).Style.NumberFormat.SetFormat("0.00\\%");
                        ws.Cell(row, column).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Justify;
                        ws.Cell(row, column).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        ws.Cell(row, column).Style.Alignment.WrapText = true;
                        ws.Column(column).Width = 30;
                    }
                },
                {
                    TipoDado.Monetario,
                    (ws, row, column, info) =>
                    {
                        var numero = info.Dado.Numerico;

                        if (numero != null)
                        {
                            ws.Cell(row, column).SetValue(numero.ToMoneyValue());
                            ws.Cell(row, column).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Justify;
                            ws.Cell(row, column).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                            ws.Cell(row, column).Style.Alignment.WrapText = true;
                            ws.Column(column).Width = 30;
                        }
                    }
                },
                {
                    TipoDado.Inteiro,
                    (ws, row, column, info) =>
                    {
                        ws.Cell(row, column).SetValue(info.Dado.Numerico);
                        ws.Cell(row, column).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Justify;
                        ws.Cell(row, column).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        ws.Cell(row, column).Style.Alignment.WrapText = true;
                        ws.Column(column).Width = 30;
                    }
                },
                {
                    TipoDado.Hora,
                    (ws, row, column, info) =>
                    {
                        ws.Cell(row, column).SetValue(info.Dado.Hora);
                        ws.Cell(row, column).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Justify;
                        ws.Cell(row, column).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        ws.Cell(row, column).Style.Alignment.WrapText = true;
                        ws.Column(column).Width = 30;
                    }
                }
            };
        }
    }
}