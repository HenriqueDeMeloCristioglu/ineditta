using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ineditta.Application.CalendarioSindicais.Eventos.Templates
{
    internal static class AgendaEventosEmail
    {
        internal const string Html = @"<style>p {margin: 0px}</style>
                                        <p>Prezado(a) Cliente,</p>
                                        <p>Informamos o evento <strong>@Model.Titulo</strong> descrito abaixo vence/encerra no dia <strong>@Model.DataVencimento</strong>:</p>
                                        <br>
                                        <p><strong>Local:</strong> @Model.Local</p>
                                        <p><strong>Data e Hora:</strong> @Model.DataVencimento</p>
                                        <p><strong>Recorrência:</strong> @Model.Recorrencia</p>
                                        <p><strong>Validade da Recorrência:</strong> @Model.ValidadeRecorrencia</p>
                                        <p><strong>Comentários:</strong> @Model.Comentarios</p>
                                        <p><strong>Permite visualização aos demais usuários:</strong> @Model.Visivel</p>
                                        ";
    }
}
