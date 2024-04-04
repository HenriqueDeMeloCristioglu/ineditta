using CSharpFunctionalExtensions;

using Ineditta.Application.AIs.Core.Services;
using Ineditta.Application.AIs.DocumentosSindicais.Dtos;
using Ineditta.Application.AIs.DocumentosSindicais.Factories;
using Ineditta.Application.EstruturasClausulas.Gerais.Repositories;
using Ineditta.BuildingBlocks.Core.Domain.Models;

namespace Ineditta.Application.AIs.DocumentosSindicais.Services.ClassificacaoClausula
{
    public class ClassificacaoClausulaService : IClassificacaoClausulaService
    {

        private readonly IEstruturaClausulaRepository _estruturaClausulaRepository;
        private readonly IAIService _aIService;

        public ClassificacaoClausulaService(IEstruturaClausulaRepository estruturaClausulaRepository,
                                            IAIService aIService)
        {
            _estruturaClausulaRepository = estruturaClausulaRepository;
            _aIService = aIService;
        }

        public async ValueTask<Result<IEnumerable<ClassificacaoClausulaDto>, Error>> ClassificarClausulas(IEnumerable<ClausulaDto> clausulas, CancellationToken cancellationToken = default)
        {
            var listaClausulasClassificadas = new List<ClassificacaoClausulaDto>();

            if (!clausulas.Any())
            {
                return Result.Failure<IEnumerable<ClassificacaoClausulaDto>, Error>(Errors.General.Business("Nenhuma cláusula for informada para ser classificada."));
            }

            var possiveisClassificacoes = await _estruturaClausulaRepository.ObterRelacaoClassificacaoAsync();

            if (possiveisClassificacoes is null)
            {
                return Result.Failure<IEnumerable<ClassificacaoClausulaDto>, Error>(Errors.General.Business("Nenhuma possível classificação foi encontrada."));
            }

            var possiveisClassificacoesJsonString = string.Join(", ", possiveisClassificacoes);
            possiveisClassificacoesJsonString = $"[{possiveisClassificacoesJsonString}]";

            foreach (var clausula in clausulas)
            {
                var pergunta = ClassificacaoClausulaFactory.CriarPerguntaIA(clausula, possiveisClassificacoesJsonString);

                if (pergunta.IsFailure)
                {
                    return Result.Failure<IEnumerable<ClassificacaoClausulaDto>, Error>(pergunta.Error);
                }

                var respostaIA = await _aIService.RealizarPergunta(pergunta.Value, cancellationToken);

                if (respostaIA.IsFailure)
                {
                    return Result.Failure<IEnumerable<ClassificacaoClausulaDto>, Error>(respostaIA.Error);
                }

                var classificacaoIA = ClassificacaoClausulaFactory.ClassificarClausula(clausula, respostaIA.Value);

                if (classificacaoIA.IsFailure)
                {
                    return Result.Failure<IEnumerable<ClassificacaoClausulaDto>, Error>(classificacaoIA.Error);
                }

                listaClausulasClassificadas.Add(classificacaoIA.Value);
            }

            return Result.Success<IEnumerable<ClassificacaoClausulaDto>, Error>(listaClausulasClassificadas);
        }

        public async ValueTask<Result<ClassificacaoClausulaDto, Error>> ClassificarClausula(ClausulaDto clausula, CancellationToken cancellationToken = default)
        {
            if (clausula is null)
            {
                return Result.Failure<ClassificacaoClausulaDto, Error>(Errors.General.Business("Nenhuma cláusula for informada para ser classificada."));
            }

            var possiveisClassificacoes = await _estruturaClausulaRepository.ObterRelacaoClassificacaoAsync();

            if (possiveisClassificacoes is null)
            {
                return Result.Failure<ClassificacaoClausulaDto, Error>(Errors.General.Business("Nenhuma possível classificação foi encontrada."));
            }

            var possiveisClassificacoesJsonString = string.Join(", ", possiveisClassificacoes);
            possiveisClassificacoesJsonString = $"[{possiveisClassificacoesJsonString}]";

            var pergunta = ClassificacaoClausulaFactory.CriarPerguntaIA(clausula, possiveisClassificacoesJsonString);

            if (pergunta.IsFailure)
            {
                return Result.Failure<ClassificacaoClausulaDto, Error>(pergunta.Error);
            }

            var respostaIA = await _aIService.RealizarPergunta(pergunta.Value, cancellationToken);

            if (respostaIA.IsFailure)
            {
                return Result.Failure<ClassificacaoClausulaDto, Error>(respostaIA.Error);
            }

            var classificacaoIA = ClassificacaoClausulaFactory.ClassificarClausula(clausula, respostaIA.Value);

            if (classificacaoIA.IsFailure)
            {
                return Result.Failure<ClassificacaoClausulaDto, Error>(classificacaoIA.Error);
            }

            return Result.Success<ClassificacaoClausulaDto, Error>(classificacaoIA.Value);
        }
    }
}
