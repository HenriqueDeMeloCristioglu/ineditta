namespace Ineditta.Application.Clausulas.Gerais.Templates
{
    public static class DocumentoClausulasLiberadasEmail
    {
        public const string Html = @"   @using System
                                        @using Ineditta.BuildingBlocks.Core.Domain.ValueObjects
                                        <style>p {margin: 0px}</style>
                                        <p>Prezado(a) Cliente <strong>Ineditta</strong></p>
                                        <br>
                                        <p>
                                            As informações dos módulos: <strong>Cláusulas, Comparativo de cláusulas, Calendário Sindical, Mapa Sindical</strong> do documento <strong>@Model.NomeDocumento</strong> descrito abaixo, estão disponíveis para consulta.
                                        </p>
                                        <br>
                                        @if (!string.IsNullOrEmpty(@Model.Abrangencia)) { 
                                            <p><strong>Abrangência:</strong> @Model.Abrangencia</p>
                                        }
                                        @if (!string.IsNullOrEmpty(@Model.AtividadesEconomicas)) { 
                                            <p><strong>Atividade econômica:</strong> @Model.AtividadesEconomicas</p>
                                        }
                                        @if (!string.IsNullOrEmpty(@Model.SindicatosLaborais)) { 
                                            <p><strong>Sindicato(s) Laboral(is):</strong> @Model.SindicatosLaborais</p>
                                        }
                                        @if (!string.IsNullOrEmpty(@Model.SindicatosPatronais)) { 
                                            <p><strong>Sindicato(s) Patronal(is):</strong> @Model.SindicatosPatronais</p>
                                        }
                                        @if (@Model.VigenciaInicial is not null && @Model.VigenciaInicial > DateOnly.MinValue) { 
                                            <p><strong>Vigência Inicial:</strong> @Model.VigenciaInicial</p>
                                        }
                                        @if (@Model.VigenciaFinal is not null && @Model.VigenciaFinal > DateOnly.MinValue) { 
                                            <p><strong>Vigência Final:</strong> @Model.VigenciaFinal</p>
                                        }
                                        <br>
                                        <p><strong>Campos adicionais para consulta:</strong></p>
                                        <br>
                                        @if (!string.IsNullOrEmpty(@Model.CodigosSindicatosCliente)) { 
                                            <p><strong>Código sindicato:</strong> @Model.CodigosSindicatosCliente</p>
                                        }
                                        <br>
                                        @if (@Model.Estabelecimentos is not null && @Model.Estabelecimentos.Count > 0) {
                                            <p><strong>Estabelecimentos: </strong></p><ul>
                                            @foreach (var estabelecimento in @Model.Estabelecimentos) {
                                                <li>
                                                    @if (!string.IsNullOrEmpty(@estabelecimento.Codigo)) {
                                                        <strong>Código: </strong> @estabelecimento.Codigo <strong>&#124;</strong>
                                                    }
                                                    <strong>Nome: </strong> @estabelecimento.Nome <strong>&#124;</strong>
                                                    <strong>CNPJ: </strong> @CNPJ.Formatar(@estabelecimento.Cnpj.Value) <strong>&#124;</strong>
                                                    <strong>Localidade: </strong> @Model.LocalizacoesUnidades[@estabelecimento.Id]
                                                </li>
                                            }
                                            </ul>
                                        }  
                                        <br>
                                        <p>Documento arquivo <a href='@Model.Url'><strong>PDF</strong></a></p>
                                        <p>Em caso de dúvidas, entre em contato conosco clicando no link: <a href='@Model.GestaoDeChamadosUrl'><strong>Cadastrar chamados.</strong></a></p>
                                        <br>
                                        <p>Atenciosamente,</p>
                                        <p><strong>Ineditta Consultoria Sindical</strong></p>
                                        ";
    }
}
