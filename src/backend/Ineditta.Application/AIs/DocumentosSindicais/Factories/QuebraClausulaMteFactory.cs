using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using CSharpFunctionalExtensions;
using HtmlAgilityPack;
using Ineditta.Application.AIs.DocumentosSindicais.Dtos;
using Ineditta.BuildingBlocks.Core.Domain.Models;

namespace Ineditta.Application.AIs.DocumentosSindicais.Factories
{
    public static class QuebraClausulaMteFactory
    {
        public static Result<QuebraClausulaDto, Error> RealizarQuebra(string textoContrato)
        {
            var listaClausulasMTE = new List<ClausulaDto>();

            HtmlDocument htmlDocumento = new();
            htmlDocumento.LoadHtml(textoContrato);

            try
            {
                List<HtmlNode> estruturasClausulasNodes = htmlDocumento.DocumentNode.Descendants("label").ToList();

                var textogrupo = "";
                var textosubgrupo = "";
                var tituloClausula = "";
                var descricaoClausula = "";

                foreach (HtmlNode estruturaClausulaNode in estruturasClausulasNodes)
                {
                    string tipoDadoClausula = estruturaClausulaNode.GetAttributeValue("class", "");

                    if (tipoDadoClausula == "textogrupo")
                    {
                        textogrupo = TratarConteudoHtml(estruturaClausulaNode.InnerText);
                    }
                    else if (tipoDadoClausula == "textosubgrupo")
                    {
                        textosubgrupo = TratarConteudoHtml(estruturaClausulaNode.InnerText);
                    }
                    else if (tipoDadoClausula == "tituloClausula")
                    {
                        tituloClausula = string.IsNullOrEmpty(tituloClausula) ? TratarConteudoHtml(estruturaClausulaNode.InnerText) : $"{tituloClausula} {TratarConteudoHtml(estruturaClausulaNode.InnerText)}";
                    }
                    else if (tipoDadoClausula == "descricaoClausula")
                    {
                        descricaoClausula = string.IsNullOrEmpty(descricaoClausula) ? ConverterParaTexto(TratarConteudoHtml(estruturaClausulaNode.InnerHtml)) : $"{descricaoClausula} {ConverterParaTexto(TratarConteudoHtml(estruturaClausulaNode.InnerHtml))}";

                        // Final do webscraping
                        if (descricaoClausula.Contains("Anexo (PDF)"))
                        {
                            break;
                        }

                        string textoCompleto = tituloClausula + Environment.NewLine + Environment.NewLine + descricaoClausula;
                        textoCompleto = QuebraClausulaCommonFactory.FormatarTexto(textoCompleto);

                        listaClausulasMTE.Add(new ClausulaDto(textogrupo, textosubgrupo, QuebraClausulaCommonFactory.ObterNumeroClausula(tituloClausula), QuebraClausulaCommonFactory.ObterNomeClausula(tituloClausula), textoCompleto));

                        tituloClausula = "";
                        descricaoClausula = "";
                    }
                }

                return Result.Success<QuebraClausulaDto, Error>(new QuebraClausulaDto(listaClausulasMTE.Count, listaClausulasMTE, false));
            }
            catch (Exception ex)
            {
                return Result.Failure<QuebraClausulaDto, Error>(Errors.General.Business($"Erro ao quebras as cláusulas do HTML: {ex.Message}"));
            }
        }

        private static string TratarConteudoHtml(string conteudoHtml)
        {
            conteudoHtml = QuebraClausulaCommonFactory.FormatarTexto(ReplaceSpecialCharacters(conteudoHtml));

            return conteudoHtml;
        }

        private static string ReplaceSpecialCharacters(string conteudoHtml)
        {
            if (string.IsNullOrEmpty(conteudoHtml))
                return conteudoHtml;

            return HttpUtility.HtmlDecode(conteudoHtml);
        }

        private static string ConverterParaTexto(string html)
        {
            var resultado = new StringBuilder();

            var doc = new HtmlDocument();
            doc.LoadHtml(ConverterTagsQuebraLinha(html));

            var nodes = doc.DocumentNode.ChildNodes;

            foreach (var node in nodes)
            {
                resultado.AppendLine(ExtrairTextoDoNodeRecursivo(node).Trim());
            }

            // Remover três ou mais quebras de linha consecutivas
            var regex = new Regex(@"(\r\n){3,}");
            resultado = new StringBuilder(regex.Replace(resultado.ToString(), "\r\n\r\n"));

            return resultado.ToString().Trim();
        }

        private static string ExtrairTextoDoNodeRecursivo(HtmlNode node)
        {
            var textoNode = new StringBuilder();

            if (node.NodeType == HtmlNodeType.Text)
            {
                textoNode.Append(node.InnerText.Trim());
            }
            else if (node.NodeType == HtmlNodeType.Element)
            {
                if (node.Name == "table")
                {
                    textoNode.AppendLine(ExtrairTextoDaTabela(node));
                }
                else
                {
                    foreach (var childNode in node.ChildNodes)
                    {
                        textoNode.AppendLine(ExtrairTextoDoNodeRecursivo(childNode));
                    }
                }
            }

            return textoNode.ToString().Trim();
        }

        private static string ExtrairTextoDaTabela(HtmlNode tableNode)
        {
            var textoTabela = new StringBuilder();

            foreach (var row in tableNode.SelectNodes(".//tr"))
            {
                foreach (var cell in row.SelectNodes("th|td"))
                {
                    textoTabela.Append(cell.InnerText.Trim() + " || ");
                }
                textoTabela.AppendLine();  // Adiciona uma linha em branco entre diferentes linhas da tabela
            }

            return textoTabela.ToString().Trim();
        }

        private static string ConverterTagsQuebraLinha(string html)
        {
            html = html.Replace("<b>", "");
            html = html.Replace("</b>", "");

            html = html.Replace("<i>", "");
            html = html.Replace("</i>", "");

            html = html.Replace("<u>", "");
            html = html.Replace("</u>", "");

            html = html.Replace("<strong>", "");
            html = html.Replace("</strong>", "");

            html = html.Replace("<em>", "");
            html = html.Replace("</em>", "");

            html = html.Replace("<s>", "");
            html = html.Replace("</s>", "");

            html = html.Replace("<sup>", "");
            html = html.Replace("</sup>", "");

            html = html.Replace("<sub>", "");
            html = html.Replace("</sub>", "");

            html = html.Replace("<small>", "");
            html = html.Replace("</small>", "");

            html = html.Replace("<big>", "");
            html = html.Replace("</big>", "");

            html = html.Replace("<font>", "");
            html = html.Replace("</font>", "");

            html = html.Replace("<strike>", "");
            html = html.Replace("</strike>", "");

            html = html.Replace("<tt>", "");
            html = html.Replace("</tt>", "");

            html = html.Replace("<cite>", "");
            html = html.Replace("</cite>", "");

            html = html.Replace("<dfn>", "");
            html = html.Replace("</dfn>", "");

            html = html.Replace("<kbd>", "");
            html = html.Replace("</kbd>", "");

            html = html.Replace("<samp>", "");
            html = html.Replace("</samp>", "");

            html = html.Replace("<var>", "");
            html = html.Replace("</var>", "");

            html = html.Replace("<abbr>", "");
            html = html.Replace("</abbr>", "");

            html = html.Replace("<q>", "");
            html = html.Replace("</q>", "");

            html = html.Replace("<blockquote>", "");
            html = html.Replace("</blockquote>", "");

            html = html.Replace("<code>", "");
            html = html.Replace("</code>", "");

            html = html.Replace("<pre>", "");
            html = html.Replace("</pre>", "");

            html = html.Replace("<ins>", "");
            html = html.Replace("</ins>", "");

            html = html.Replace("<del>", "");
            html = html.Replace("</del>", "");

            // Com conteúdo
            string patternSpan = @"<span[^>]*>";
            html = Regex.Replace(html, patternSpan, "");

            html = html.Replace("</span>", "");

            string patternLink = @"<a[^>]*>";
            html = Regex.Replace(html, patternLink, "");

            html = html.Replace("</a>", "");

            string patternFont = @"<font[^>]*>";
            html = Regex.Replace(html, patternFont, "");

            html = html.Replace("</font>", "");

            string patternMark = @"<mark[^>]*>";
            html = Regex.Replace(html, patternMark, "");

            html = html.Replace("</mark>", "");

            string patternAbbr = @"<abbr[^>]*>";
            html = Regex.Replace(html, patternAbbr, "");

            html = html.Replace("</abbr>", "");

            string patternParamQ = @"<q[^>]*>";
            html = Regex.Replace(html, patternParamQ, "");

            html = html.Replace("</q>", "");

            string patternCode = @"<code[^>]*>";
            html = Regex.Replace(html, patternCode, "");

            html = html.Replace("</code>", "");

            string patternPre = @"<pre[^>]*>";
            html = Regex.Replace(html, patternPre, "");

            html = html.Replace("</pre>", "");

            string patternIns = @"<ins[^>]*>";
            html = Regex.Replace(html, patternIns, "");

            html = html.Replace("</ins>", "");

            string patternDel = @"<del[^>]*>";
            html = Regex.Replace(html, patternDel, "");

            html = html.Replace("</del>", "");

            return html;
        }
    }
}
