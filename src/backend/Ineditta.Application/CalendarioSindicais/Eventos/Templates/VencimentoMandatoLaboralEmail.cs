namespace Ineditta.Application.CalendarioSindicais.Eventos.Templates
{
    internal static class VencimentoMandatoLaboralEmail
    {
        internal const string Html = @"<style>p {margin: 0px}</style>
                                        <p>Prezado(a) Cliente,</p>
                                        <p>Informamos que o <strong>mandato sindical laboral</strong> descrito abaixo vence no dia @Model.DataVencimento:</p>
                                        <br>
                                        <p><strong>Sindicato Laboral:</strong> @Model.SindicatoLaboral</p>
                                        <p><strong>Abrangência:</strong> @Model.Abrangencia</p>
                                        <p><strong>Dirigentes:</strong></p>
                                        <ul>
                                        @foreach(var dirigente in Model.Dirigente)
                                        {
                                            <li><strong>Nome: </strong>@dirigente.Nome - <strong>Função: </strong>@dirigente.Funcao</li>
                                        }
                                        </ul>
                                        <br>
                                        <p>Atenciosamente,</p>
                                        <p><strong>Ineditta Consultoria Sindical</strong></p>
                                        ";
    }
}
