namespace Ineditta.Application.CalendarioSindicais.Eventos.Templates
{
    internal static class ReuniaoSindicalEmail
    {
        internal const string Html = @"<style>p {margin: 0px}</style>
                                        <p>Prezado(a) Cliente,</p>
                                        <p>Informamos que a <strong>Reunião entre entidades sindicais</strong> descrita abaixo ocorrerá em <strong>@Model.DataHora</strong>:</p>
                                        <br>
                                        <p><strong>Sindicato(s) Laboral(is):</strong> @Model.SindicatosLaborais</p>
                                        <p><strong>Sindicato(s) Patronal(is):</strong> @Model.SindicatosPatronais</p>
                                        <p><strong>Atividades Econômicas:</strong> @Model.AtividadesEconomicas</p>
                                        <p><strong>Data-base da negociação:</strong> @Model.DataBase</p>
                                        <p><strong>Nome do documento:</strong> @Model.NomeDocumento</p>
                                        <p><strong>Fase da negociação:</strong> @Model.FaseNegociacao</p>
                                        <p><strong>Abrangência:</strong> @Model.Abrangencia</p>
                                        <p><strong>Tipo da reunião:</strong> @Model.TipoReuniaoSindical</p>
                                        <p><strong>Observações:</strong> @Model.ClientReport</p>
                                        <br>
                                        <p>Atenciosamente,</p>
                                        <p><strong>Ineditta Consultoria Sindical</strong></p>
                                        ";
    }
}
