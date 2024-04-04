using System.Text.RegularExpressions;

using CSharpFunctionalExtensions;

using Ineditta.Application.AIs.Core.Dtos;
using Ineditta.Application.AIs.DocumentosSindicais.Dtos;
using Ineditta.BuildingBlocks.Core.Domain.Models;

using Newtonsoft.Json;

namespace Ineditta.Application.AIs.DocumentosSindicais.Factories
{
    public static class ClassificacaoClausulaFactory
    {
        public static Result<SendMessageDto, Error> CriarPerguntaIA(ClausulaDto clausula, string classificacoesPossiveis)
        {
            if (string.IsNullOrEmpty(classificacoesPossiveis))
            {
                return Result.Failure<SendMessageDto, Error>(Errors.General.Business("Nenhuma possível classificação foi informada."));
            }

            var system = "Você é um assistente prestativo, que responde em português do Brasil.";

            var pergunta = $"Com base na clásula de um contrato que estou enviando abaixo, selecione uma das possíveis classificações e me responda apenas o objeto em formato JSON da classificação que mais fizer sentido. A resposta deve ser apenas o JSON, nada a mais. {Environment.NewLine}";

            if (!string.IsNullOrEmpty(clausula.Grupo))
            {
                pergunta += $"Grupo: {clausula.Grupo}. {Environment.NewLine}";
            }

            if (!string.IsNullOrEmpty(clausula.SubGrupo))
            {
                pergunta += $"SubGrupo: {clausula.SubGrupo}. {Environment.NewLine}";
            }

            pergunta += $"Título cláusula: {clausula.Nome}. {Environment.NewLine}";

            pergunta += $"Cláusula: {clausula.Descricao}. {Environment.NewLine}";

            pergunta += $"Opções de classificação: {classificacoesPossiveis}. {Environment.NewLine}";

            var mensagem = new SendMessageDto(system, pergunta);

            return Result.Success<SendMessageDto, Error>(mensagem);
        }

        public static Result<ClassificacaoClausulaDto, Error> ClassificarClausula(ClausulaDto clausula, string retornoIA)
        {
            if (string.IsNullOrEmpty(retornoIA))
            {
                return Result.Failure<ClassificacaoClausulaDto, Error>(Errors.General.Business("A resposta da IA está vazia."));
            }

            string jsonPattern = @"\{(?:[^{}]|(?<Open>{)|(?<-Open>}))*\}(?(Open)(?!))";
            Match match = Regex.Match(retornoIA, jsonPattern);

            if (!match.Success)
            {
                return Result.Failure<ClassificacaoClausulaDto, Error>(Errors.General.Business("Nenhum JSON encontrado na string de resposta da IA."));
            }

            string jsonResult = match.Value;

            try
            {
                var classificacaoClausulaResponse = JsonConvert.DeserializeObject<ClassificacaoClausulaResponseDto>(jsonResult);

                var clasulaClassificada = new ClassificacaoClausulaDto(classificacaoClausulaResponse?.Id ?? 0, retornoIA, clausula);

                return Result.Success<ClassificacaoClausulaDto, Error>(clasulaClassificada);
            }
            catch (JsonException ex)
            {
                return Result.Failure<ClassificacaoClausulaDto, Error>(Errors.General.Business($"Erro durante a desserialização do JSON: {ex.Message}"));
            }
        }
    }
}
