using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ineditta.Application.CalendarioSindicais.Eventos.Templates
{
    internal static class VencimentoDocumentoEmail
    {
        internal const string Html = @"<style>p {margin: 0px}</style>
                                        <p>Prezado(a) Cliente,</p>
                                        <p>Informamos que o(a) <strong>@Model.NomeDocumento</strong> descrito abaixo vence no dia <strong>@Model.DataVencimento</strong>:</p>
                                        <br>
                                        <p><strong>Abrangência:</strong> @Model.Abrangencia</p>
                                        <p><strong>Atividade econômica:</strong> @Model.AtividadesEconomicas</p>
                                        <p><strong>Sindicato(s) Laboral(is):</strong> @Model.SindicatoLaboral</p>
                                        <p><strong>Sindicato(s) Patronal(is):</strong> @Model.SindicatoPatronal</p>
                                        <br>
                                        <p>Atenciosamente,</p>
                                        <p><strong>Ineditta Consultoria Sindical</strong></p>
                                        ";
    }
}
