using System.Globalization;
using System.Text;

using CSharpFunctionalExtensions;

using DinkToPdf;
using DinkToPdf.Contracts;

using Ineditta.Repository.Clausulas.Geral.Views.Clausula;

namespace Ineditta.API.Builders.Clausulas
{
    public class RelatorioBuscaRapidaPdfBuilder
    {
        private readonly IConverter _converter;

        public RelatorioBuscaRapidaPdfBuilder(IConverter converter)
        {
            _converter = converter;
        }

        public Result<byte[]> Criar(List<ClausulaVw> clausulas)
        {
            var html = new StringBuilder(@"
                <html>
                <head>
                    <style>
                        mark {
                            padding: 0;
                        }
            
                        .container {
                            display: flex;
                            flex-direction: column;
                        }

                        .clausula-title {
                            font-size: 1.6rem;
                        }

                        .clausula-location {
                            font-size: 1.25rem;
                            text-decoration: underline;
                        }

                        .header-flex {
                            display: flex;
                            flex-direction: row;
                            margin-bottom: 10px;
                            justify-content: space-between;
                            width: 100%;
                        }

                        .header-flex div {
                            margin-bottom: 10px;
                        }

                        body {
                            font-family: 'Montserrat', sans-serif;
                        }

                        .ineditta-top {
                            text-align: right;
                        }

                        hr {
                            border: 2px solid #000
                        }

                        .table-striped>tbody>tr:nth-of-type(odd) {
                            background-color: #f9f9f9
                        }

                        .table {
                            width: 100%;
                            max-width: 100%;
                        }

                        td {
                            line-height: 1.5;
                        }
                    </style>
                </head>
                <body>
                    <p class='ineditta-top'><strong>Ineditta Consultoria Sindical e Trabalhista Ltda</strong></p>");


            foreach (var clausula in clausulas)
            {
                DateTime dataInicial = clausula.DocumentoValidadeInicial!.Value.ToDateTime(TimeOnly.MinValue);
                DateTime dataFinal = clausula.DocumentoValidadeFinal!.Value.ToDateTime(TimeOnly.MinValue);

#pragma warning disable S6608 // Prefer indexing instead of "Enumerable" methods on types implementing "IList"
                html.Append(CultureInfo.InvariantCulture, $@"
                <div class='container'>");

                if (clausula.DocumentoSindicatoLaboral is not null && clausula.DocumentoSindicatoLaboral.Any())
                {
                    var sindicatoLaboral = clausula.DocumentoSindicatoLaboral.First();

                    html.Append(CultureInfo.InvariantCulture, $@"
                        <p class='clausula-location'><strong>
                            {sindicatoLaboral.Uf} / {sindicatoLaboral.Municipio}
                        </strong></p>
                    ");
                }

                html.Append(CultureInfo.InvariantCulture, $@"
                    <table class='table'>
                        <tbody>
                            <tr>
                                <td><strong>Cláusula:</strong> {clausula.EstruturaClausulaNome}</td>
                                <td><strong>Grupo:</strong> {clausula.GrupoClausulaNome}</td>
                            </tr>
                            <tr>
                                <td><strong>Documento:</strong> {clausula.DocumentoNome}</td>
                                <td><strong>Data Base:</strong> {clausula.DocumentoDatabase}</td>
                            </tr>");

                if (clausula.DocumentoCnae is not null && clausula.DocumentoCnae.Any())
                {
                    var cnaes = clausula.DocumentoCnae;

                    html.Append(CultureInfo.InvariantCulture, $@"
                            <tr>
                                <td><strong>Atividade Econômica:</strong> {string.Join('/', cnaes.Select(cnae => cnae.Subclasse))}
                                </td>
                            </tr>");
                }

                if (clausula.DocumentoSindicatoLaboral is not null && clausula.DocumentoSindicatoLaboral.Any())
                {
                    var sindicatosLaborais = clausula.DocumentoSindicatoLaboral;

                    html.Append(CultureInfo.InvariantCulture, $@"
                            <tr>
                                <td><strong>Sindicato Laboral:</strong> {string.Join("; ", sindicatosLaborais.Select(sindicatoLaboral => sindicatoLaboral.Sigla + " / " + sindicatoLaboral.Denominacao))}</td>
                            </tr>");
                }

                if (clausula.DocumentoSindicatoPatronal is not null && clausula.DocumentoSindicatoPatronal.Any())
                {
                    var sindicatosPatronais = clausula.DocumentoSindicatoPatronal;

                    html.Append(CultureInfo.InvariantCulture, $@"
                            <tr>
                                <td><strong>Sindicato Patronal:</strong> {string.Join("; ", sindicatosPatronais.Select(sindicatoPatronal => sindicatoPatronal.Sigla + " / " + sindicatoPatronal.Denominacao))}</td>
                            </tr>");
                }

                html.Append(CultureInfo.InvariantCulture, $@"
                        </tbody>
                    </table>

                    <table class='table' style='margin-bottom: 20px;'>
                        <tbody>");

                if (clausula.DocumentoSindicatoLaboral is not null && clausula.DocumentoSindicatoLaboral.Any())
                {
                    var sindicatosLaborais = clausula.DocumentoSindicatoLaboral;

                    html.Append(CultureInfo.InvariantCulture, $@"
                    <tr>
                        <td><strong>Estado:</strong> {string.Join('/', sindicatosLaborais.Select(obj => obj.Uf))}</td>
                                <td><strong>Cidade:</strong> {string.Join('/', sindicatosLaborais.Select(obj => obj.Municipio))}</td>
                    ");
                }

                html.Append(CultureInfo.InvariantCulture, $@"
                            <td><strong>Período:</strong> {dataInicial.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)} - {dataFinal.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)}</td>
                        </tr>
                    </tbody>
                </table>

                <div>
                    <p><strong>Texto Resumo</strong>

                    <p style='text-align: justify; white-space: pre-line'>{clausula.TextoResumido}</p>");
#pragma warning restore S6608 // Prefer indexing instead of "Enumerable" methods on types implementing "IList"

                if (clausula?.TextoResumido?.Length > 0)
                {
                    html.Append(CultureInfo.InvariantCulture, $@"
                            <p><strong>Texto Resumo Empresa</strong>

                            <p style='text-align: justify; white-space: pre-line'>{clausula.TextoResumido}</p>
                    ");
                }

                html.Append(@"
                    </div>
                </div>");

                html.Append("<hr>");
                html.Append("<br />");
            }

            html.Append(@"</body>");

            var file = Encoding.UTF8.GetBytes(html.ToString());

            var doc = new HtmlToPdfDocument()
            {
                GlobalSettings =
                {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Portrait,
                    PaperSize = PaperKind.A4,
                    Margins = new MarginSettings() { Top = 10, Left = 20, Right = 20 }
                },
                Objects =
                {
                    new ObjectSettings()
                    {
                        HtmlContent = Encoding.UTF8.GetString(file),
                        WebSettings = { DefaultEncoding = "utf-8" }
                    }
                }
            };

            return _converter.Convert(doc);
        }
    }
}
