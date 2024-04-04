using CSharpFunctionalExtensions;
using Ineditta.Application.InformacoesAdicionais.Cliente.Entities;
using Ineditta.Application.InformacoesAdicionais.Cliente.UseCases;

namespace Ineditta.Application.InformacoesAdicionais.Cliente.Factories
{
    public static class ObservacaoAdicionalFactory
    {
        public static Result<IEnumerable<ObservacaoAdicional>> Criar(IEnumerable<ObservacaoAdicionalRequest>? objs)
        {
            List<ObservacaoAdicional> observacoesAdicionais = new();

            if (objs != null)
            {
                foreach (var info in objs)
                {
                    var informacaoAdicional = ObservacaoAdicional.Criar(info.Id, info.Valor, (TipoObservacaoAdicional)info.Tipo);

                    if (informacaoAdicional.IsFailure)
                    {
                        return Result.Failure<IEnumerable<ObservacaoAdicional>>(informacaoAdicional.Error);
                    }

                    observacoesAdicionais.Add(informacaoAdicional.Value);
                }
            }

            return Result.Success<IEnumerable<ObservacaoAdicional>>(observacoesAdicionais);
        }
    }
}
