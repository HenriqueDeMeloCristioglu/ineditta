namespace Ineditta.Application.Documentos.Sindicais.Templates
{
    public static class DocumentoSindicalNotificarUpsertEmail
    {
        public const string Html = @"@using System
                                        <style>p {margin: 0px}</style>   
                                        <p>Prezado(a)</p>
                                        <br>
                                        <p>Informamos que o documento <strong>@Model.NomeDocumento</strong> descrito abaixo, foi inserido no Sistema Ineditta:</p>
                                        <br>
                                        @if (!string.IsNullOrEmpty(@Model.OrigemDocumento)) { 
                                            <p><strong>Origem do Documento:</strong> @Model.OrigemDocumento</p>
                                        }
                                        @if (!string.IsNullOrEmpty(@Model.NomeDocumento)) { 
                                            <p><strong>Nome do Documento:</strong> @Model.NomeDocumento</p>
                                        }
                                        @if (!string.IsNullOrEmpty(@Model.Assunto)) { 
                                            <p><strong>Assunto:</strong> @Model.Assunto</p>
                                        }
                                        @if (!string.IsNullOrEmpty(@Model.NumeroLegislacao)) { 
                                            <p><strong>Número da Legislação:</strong> @Model.NumeroLegislacao</p>
                                        }
                                        @if (!string.IsNullOrEmpty(@Model.FonteLegislacaoSite)) { 
                                            <p><strong>Fonte Legislação Site:</strong> @Model.FonteLegislacaoSite</p>
                                        }
                                        @if (!string.IsNullOrEmpty(@Model.Restrito)) { 
                                            <p><strong>Restrito:</strong> @Model.Restrito</p>
                                        }
                                        @if (!string.IsNullOrEmpty(@Model.SindicatosLaborais)) { 
                                            <p><strong>Sindicato(s) Laboral(is):</strong> @Model.SindicatosLaborais</p>
                                        }
                                        @if (!string.IsNullOrEmpty(@Model.SindicatosPatronais)) { 
                                            <p><strong>Sindicato(s) Patronal(is):</strong> @Model.SindicatosPatronais</p>
                                        }
                                        @if (@Model.ValidadeInicial is not null && @Model.ValidadeInicial > DateOnly.MinValue) { 
                                            <p><strong>Validade Inicial:</strong> @Model.ValidadeInicial</p>
                                        }
                                        @if (@Model.ValidadeFinal is not null && @Model.ValidadeFinal > DateOnly.MinValue) { 
                                            <p><strong>Validade Final:</strong> @Model.ValidadeFinal</p>
                                        }
                                        @if (!string.IsNullOrEmpty(@Model.Comentarios)) { 
                                            <p><strong>Comentarios:</strong> @Model.Comentarios</p>
                                        }
                                        @if (@Model.Estabelecimentos is not null && @Model.Estabelecimentos.Count > 0) {
                                            <p><strong>Estabelecimentos: </strong></p><ul>
                                            @foreach (var estabelecimento in @Model.Estabelecimentos) {
                                                <li><strong>Nome: </strong> @estabelecimento.Nome <strong>&#124;</strong>
                                                    <strong>CNPJ: </strong> @estabelecimento.Cnpj.Value
                                                    @if (!string.IsNullOrEmpty(@estabelecimento.CodigoSindicatoCliente)) {
                                                        <strong>&#124;</strong> <strong>Código Sindicato Cliente: </strong> @estabelecimento.CodigoSindicatoCliente
                                                    }
                                                </li>
                                            }
                                            </ul>
                                        }       
                                        @if (!string.IsNullOrEmpty(@Model.Abrangencia)) { 
                                            <p><strong>Abrangência:</strong> @Model.Abrangencia</p>
                                        }
                                        @if (!string.IsNullOrEmpty(@Model.AtividadesEconomicas)) { 
                                            <p><strong>Atividades Econômicas:</strong> @Model.AtividadesEconomicas</p>
                                        }
                                        <br>
                                        <p><a href='@Model.DocumentoLink'><strong>Clique aqui </strong></a>para visualizar o documento.</p>
                                        ";


    }
}
