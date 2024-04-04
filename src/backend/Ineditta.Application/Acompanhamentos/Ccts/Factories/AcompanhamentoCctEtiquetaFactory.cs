using Ineditta.Application.Acompanhamentos.CctsEtiquetas.Entities;
using Ineditta.Application.Acompanhamentos.CctsEtiquetasOpcoes.Repositories;

namespace Ineditta.Application.Acompanhamentos.Ccts.Factories
{
    public class AcompanhamentoCctEtiquetaFactory
    {
        private readonly IAcompanhamentoCctEtiquetaOpcaoRepository _acompanhamentoCctEtiquetaOpcaoRepository;

        public AcompanhamentoCctEtiquetaFactory(IAcompanhamentoCctEtiquetaOpcaoRepository acompanhamentoCctEtiquetaOpcaoRepository)
        {
            _acompanhamentoCctEtiquetaOpcaoRepository = acompanhamentoCctEtiquetaOpcaoRepository;
        }

        public async Task<IEnumerable<AcompanhamentoCctEtiqueta>> Criar(IEnumerable<int>? etiquetasOpcoesIds)
        {
            List<AcompanhamentoCctEtiqueta> etiquetas = new();

            if (etiquetasOpcoesIds is null || !etiquetasOpcoesIds.Any())
            {
                return etiquetas;
            }

            foreach (var etiquetasOpcaoId in etiquetasOpcoesIds)
            {
                var acompanhamentoCctEtiquetaOpcao = await _acompanhamentoCctEtiquetaOpcaoRepository.ObterPorId(etiquetasOpcaoId);

                if (acompanhamentoCctEtiquetaOpcao is null)
                {
                    continue;
                }

                var acompanhamentoCctEtiqueta = AcompanhamentoCctEtiqueta.Criar(acompanhamentoCctEtiquetaOpcao);

                if (acompanhamentoCctEtiqueta.IsFailure)
                {
                    continue;
                }

                etiquetas.Add(acompanhamentoCctEtiqueta.Value);
            }

            return etiquetas;
        }
    }
}
