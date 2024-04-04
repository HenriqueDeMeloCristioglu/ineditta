using Ineditta.Application.Documentos.Sindicais.Dtos;

namespace Ineditta.Application.Documentos.Estabelecimentos.Repositories
{
    public interface IDocumentoEstabelecimentoCruzamentosRepository
    {
        ValueTask<IEnumerable<Estabelecimento>?> ObterEstabelecimentosDocumentoPorCruzamento(
            IEnumerable<int>? cnaesIds,
            IEnumerable<int>? abrangenciaIds,
            IEnumerable<int>? sindicatosPatronaisIds,
            IEnumerable<int>? sindicatosLaboraisIds,
            string mesDataBaseDoc
        );

        ValueTask<IEnumerable<Estabelecimento>?> ObterEstabelecimentosDocumentoComercialPorCruzamento(
            IEnumerable<int>? cnaesIds,
            IEnumerable<int>? abrangenciaIds,
            IEnumerable<int>? sindicatosPatronaisIds,
            IEnumerable<int>? sindicatosLaboraisIds,
            string emailUsuario
        );

        ValueTask<IEnumerable<Estabelecimento>?> ObterEstabelecimentosDocumentoPorIds(IEnumerable<int> estabelecimentosIds);
    }
}
