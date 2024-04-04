using CSharpFunctionalExtensions;
using Ineditta.Application.InformacoesAdicionais.Cliente.Entities;
using Ineditta.Application.InformacoesAdicionais.Cliente.UseCases;

namespace Ineditta.Application.InformacoesAdicionais.Cliente.Factories
{
    public static class InformacaoAdicionalFactory
    {
        public static Result<IEnumerable<InformacaoAdicional>> Criar(IEnumerable<InformacaoAdicionalRequest> objs)
        {
            List<InformacaoAdicional> informacoesAdicionais = new();

            foreach (var info in objs)
            {
                var informacaoAdicional = InformacaoAdicional.Criar(info.Id, info.Valor);

                if (informacaoAdicional.IsFailure)
                {
                    return Result.Failure<IEnumerable<InformacaoAdicional>>(informacaoAdicional.Error);
                }

                informacoesAdicionais.Add(informacaoAdicional.Value);
            }

            return Result.Success<IEnumerable<InformacaoAdicional>>(informacoesAdicionais);
        }
    }
}
