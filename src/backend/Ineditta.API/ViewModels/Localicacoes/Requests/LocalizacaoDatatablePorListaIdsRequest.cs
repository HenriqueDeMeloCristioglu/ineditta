using Ineditta.BuildingBlocks.Core.Web.API.Datatables;

namespace Ineditta.API.ViewModels.Localizacoes
{
    public class LocalizacaoDatatablePorListaIdsRequest : DataTableRequest
    {
        public IEnumerable<int> AbrangenciasSelecionadas { get; set; } = null!;
    }
}
