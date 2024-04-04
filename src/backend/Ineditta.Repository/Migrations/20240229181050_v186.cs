using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ineditta.Repository.Migrations
{
    /// <inheritdoc />
    public partial class v186 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                INSERT INTO template_email_tb
                (tipo_template, nivel, referencia_id, html)
                SELECT 1, 2, jt2.id, '@using System
                                        @using Ineditta.BuildingBlocks.Core.Domain.ValueObjects
                                        <style>p {margin: 0px}</style>
                                        <p>Prezado Gerente,</p>
                                        <br>
                                        <p>
                                            Informamos que este mês foi assinado um novo documento pelo sindicato: &#34;@Model.NomeDocumento&#34; 
                                            (@if (@Model.VigenciaInicial is not null && @Model.VigenciaInicial > DateOnly.MinValue) { 
                                                <span>@Model.VigenciaInicial</span>
                                            }
                                            @if (@Model.VigenciaFinal is not null && @Model.VigenciaFinal > DateOnly.MinValue) { 
                                                <span> - @Model.VigenciaFinal</span>
                                            })
                                        </p>
                                        <br>
                                        <p>Sempre consulte o documento atualizado clicando <a href=''@Model.Url''><strong>aqui</strong></a>, atentando-se aos principais itens abaixo, entre outros:</p>
                                        <br>
                                        <ul>
                                            <li>Jornada e Horário de Trabalho;</li>
                                            <li>Permissões e Condições para o trabalho aos domingos</li>
                                            <li>Permissões e Condições para o trabalho aos feriados</li>
                                            <li>Permissões e Condições de jornadas em datas especiais (dezembro)</li>
                                        </ul>
                                        <br>
                                        <p>Caso tenha alguma dúvida, a equipe de Relações Sindicais do MAGALU estará à disposição para orientá-los.</p>
                                        <br>
                                        <p style=''color: blue;''><strong>Campos adicionais para consulta CSC Magalu:</strong></p>
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
                                        <p>Atenciosamente,</p>
                                        <p><strong>Ineditta Consultoria Sindical</strong></p>'
                FROM
                    (SELECT jt.* FROM JSON_TABLE(
		                JSON_ARRAY(290, 342, 343, 344, 345, 346, 347, 348, 349, 350, 351, 352, 353, 354, 355, 356, 357, 358, 359, 360, 361, 362, 363, 364, 365, 366)
		                , '$[*]' COLUMNS(id INT PATH '$[0]')
	                ) as jt) jt2;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DELETE FROM template_email_tb tet
                WHERE tet.referencia_id IN (290, 342, 343, 344, 345, 346, 347, 348, 349, 350, 351, 352, 353, 354, 355, 356, 357, 358, 359, 360, 361, 362, 363, 364, 365, 366)
                      AND tet.tipo_template = 1
                      AND tet.nivel = 2;
            ");
        }
    }
}
