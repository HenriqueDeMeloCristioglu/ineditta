using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ineditta.Application.CalendarioSindicais.Eventos.Templates
{
    internal static class EventoClausulaEmail
    {
        internal const string Html = @"<style>p {margin: 0px}</style>
                                        <p>Prezado(a) Cliente,</p>
                                        <p>Informamos o evento <strong>@Model.NomeCampo</strong> da cláusula <strong>@Model.NomeInfoAdicional</strong> prevista no documento sindical <strong>@Model.NomeDocumento</strong> descrito abaixo vence/encerra no dia <strong>@Model.DataVencimento</strong>:</p>
                                        <br>
                                        <p><strong>Abrangência:</strong> @Model.Abrangencias</p>
                                        <p><strong>Atividade(s) Econômica(s):</strong> @Model.AtividadesEconomicas</p>
                                        <p><strong>Sindicato(s) Laboral(is):</strong> @Model.SindicatosLaborais</p>
                                        <p><strong>Sindicato(s) Patronal(is):</strong> @Model.SindicatosPatronais</p>
                                        <br>
                                        <p>Atenciosamente,</p>
                                        <p><strong>Ineditta Consultoria Sindical</strong></p>
                                        ";
    }
}
