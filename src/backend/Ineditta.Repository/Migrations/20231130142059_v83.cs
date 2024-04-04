using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ineditta.Repository.Migrations
{
    /// <inheritdoc />
    public partial class v83 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                UPDATE jfase SET fase = '{""fase"": ""Fechada-formalização documento"",""perguntas"": [{""tipo"": ""R"",""texto"": ""Contato realizado?"",""opcoes"": [],""adicionais"": [{""tipo"": ""S"",""texto"": ""Sindicato contatado"",""opcoes"": [""Laboral"",""Patronal""]},{""tipo"": ""S"",""texto"": ""Contato feito por"",""opcoes"": [""E-mail"",""Telefone"",""WhatsApp"",""Facebook"",""Linkedin"",""Twitter"",""Site"",""Fórum"",""Outros""]},{""tipo"": ""R"",""texto"": ""Confirmou o percentual de Reajuste Salarial junto aos dois sindicatos?"",""opcoes"": []},{""tipo"": ""R"",""texto"": ""Verificou se existe circular assinada entre as partes?"",""opcoes"": []},{""tipo"": ""S"",""texto"": ""Qual o status da disponibilização do documento?"",""opcoes"": [""Aguardando liberação."",""Aguardando documento assinado."",""Aguardando registro no Mediador."",""Os sindicatos não compartilham uma cópia do documento assinado, somente para associados ou contribuintes."",""Outros""]},{""tipo"": ""T"",""texto"": ""Caso estejam aguardando o mediador informe o Nº da solicitação (MR)"",""opcoes"": []},{""tipo"": ""T"",""texto"": ""Estimativa do reajuste patronal?"",""opcoes"": []},{""tipo"": ""TA"",""texto"": ""Observações (Para uso interno da Ineditta)"",""opcoes"": []}]}]}'
                WHERE id_jfase = 10;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.Sql(@"
                UPDATE jfase SET fase = '{""fase"": ""Fechada-formalização documento"",""perguntas"": [{""tipo"": ""R"",""texto"": ""Contato realizado?"",""opcoes"": [],""adiconais"": [{""tipo"": ""S"",""texto"": ""Sindicato contatado"",""opcoes"": [""Laboral"",""Patronal""]},{""tipo"": ""S"",""texto"": ""Contato feito por"",""opcoes"": [""E-mail"",""Telefone"",""WhatsApp"",""Facebook"",""Linkedin"",""Twitter"",""Site"",""Fórum"",""Outros""]},{""tipo"": ""R"",""texto"": ""Confirmou o percentual de Reajuste Salarial junto aos dois sindicatos?"",""opcoes"": []},{""tipo"": ""R"",""texto"": ""Verificou se existe circular assinada entre as partes?"",""opcoes"": []},{""tipo"": ""S"",""texto"": ""Qual o status da disponibilização do documento?"",""opcoes"": [""Aguardando liberação."",""Aguardando documento assinado."",""Aguardando registro no Mediador."",""Os sindicatos não compartilham uma cópia do documento assinado, somente para associados ou contribuintes."",""Outros""]},{""tipo"": ""T"",""texto"": ""Caso estejam aguardando o mediador informe o Nº da solicitação (MR)"",""opcoes"": []},{""tipo"": ""T"",""texto"": ""Estimativa do reajuste patronal?"",""opcoes"": []},{""tipo"": ""TA"",""texto"": ""Observações (Para uso interno da Ineditta)"",""opcoes"": []}]}]}'
                WHERE id_jfase = 10;
            ");
        }
    }
}
