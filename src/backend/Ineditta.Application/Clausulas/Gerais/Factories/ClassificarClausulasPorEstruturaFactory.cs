using CSharpFunctionalExtensions;

using Ineditta.Application.Clausulas.Gerais.Dtos;
using Ineditta.Application.Clausulas.Gerais.Dtos.ClassificarClausulasPorEstruturaFactory;

namespace Ineditta.Application.Clausulas.Gerais.Factories
{
    public static class ClassificarClausulasPorEstruturaFactory
    {
        public static Result<List<List<ClassificarClausulasPorEstruturaResponseDto>>> Criar(IEnumerable<ObterClausulasPorEmpresaDocumentoIdDto> clausulaGerals)
        {
            List<List<ClassificarClausulasPorEstruturaResponseDto>> classificacoes = new();
            List<ClassificarClausulasPorEstruturaResponseDto> clausulas = new();

            int classificacaoAtual = 0;
            foreach (var clausula in clausulaGerals)
            {
                if (classificacaoAtual != clausula.EstruturaClausulaId)
                {
                    if (clausulas.Any())
                    {
                        classificacoes.Add(clausulas);
                        clausulas = new List<ClassificarClausulasPorEstruturaResponseDto>();
                    }

                    classificacaoAtual = clausula.EstruturaClausulaId;
                }

                clausulas.Add(new ClassificarClausulasPorEstruturaResponseDto
                {
                    Id = clausula.Id,
                    Texto = clausula.Texto ?? string.Empty,
                    InstrucaoIa = clausula.InstrucaoIa,
                    MaximoPalavrasIa = clausula.MaximoPalavrasIa
                });
            }
            classificacoes.Add(clausulas);

            return classificacoes;
        }
    }
}
