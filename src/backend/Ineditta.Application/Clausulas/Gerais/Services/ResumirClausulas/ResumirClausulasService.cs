using CSharpFunctionalExtensions;

using Ineditta.Application.AIs.Core.Dtos;
using Ineditta.Application.AIs.Core.Services;
using Ineditta.Application.Clausulas.Gerais.Dtos.ResumirClausulas;

namespace Ineditta.Application.Clausulas.Gerais.Services.ResumirClausulas
{
    public class ResumirClausulaService : IResumirClausulas
    {
        private readonly IAIService _aIService;

        public ResumirClausulaService(IAIService aIService)
        {
            _aIService = aIService;
        }

        public async ValueTask<Result<ResumirClausulasServiceResponse>> Criar(ResumirClausulasServiceRequest request)
        {
            var texto = string.Join("\n \n ", request.Textos);
            var pergunta = request.InstrucoesIA.Replace("texto_clausula", $"\n \n {texto} \n \n") + $"\n \n Gere no máximo {request.MaximoPalavrasIA} palavras.";

            var descricaoDoSistema = "Você é uma assistente que lê textos de cláusulas que vem de documentos sindicais e faz um resumo de todas elas. só retorna pra mim o resumo em s, sem nenhum tipo de anotação";

            var sendMessageDto = new SendMessageDto(pergunta, descricaoDoSistema);

            var result = await _aIService.RealizarPergunta(sendMessageDto);

            if (result.IsFailure)
            {
                return Result.Failure<ResumirClausulasServiceResponse>("Erro ao resumir cláusulas");
            }

            var resumirClausulasServiceResponse = new ResumirClausulasServiceResponse
            {
                Texto = result.Value
            };

            return Result.Success(resumirClausulasServiceResponse);
        }
    }
}
