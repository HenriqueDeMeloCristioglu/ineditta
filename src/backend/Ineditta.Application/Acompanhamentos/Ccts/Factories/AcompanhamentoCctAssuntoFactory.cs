using Ineditta.Application.Acompanhamentos.CctsAssuntos.Entities;
using Ineditta.Application.EstruturasClausulas.Gerais.Repositories;

namespace Ineditta.Application.Acompanhamentos.Ccts.Factories
{
    public class AcompanhamentoCctAssuntoFactory
    {
        private readonly IEstruturaClausulaRepository _estruturaClausulaRepository;

        public AcompanhamentoCctAssuntoFactory(IEstruturaClausulaRepository estruturaClausulaRepository)
        {
            _estruturaClausulaRepository = estruturaClausulaRepository;
        }

        public async Task<IEnumerable<AcompanhamentoCctAssunto>> Criar(IEnumerable<int>? estruturasIds)
        {
            List<AcompanhamentoCctAssunto> assuntos = new();

            if (estruturasIds is null || !estruturasIds.Any())
            {
                return assuntos;
            }

            var estruturasClausulas = await _estruturaClausulaRepository.ObterTodasPorIds(estruturasIds);

            if (estruturasClausulas is null || !estruturasClausulas.Any())
            {
                return assuntos;
            }

            foreach (var estruturaClausula in estruturasClausulas)
            {
                var acompanhamentoCctAssunto = AcompanhamentoCctAssunto.Criar(estruturaClausula);

                if (acompanhamentoCctAssunto.IsFailure)
                {
                    continue;
                }

                assuntos.Add(acompanhamentoCctAssunto.Value);
            }

            return assuntos;
        }
    }
}
