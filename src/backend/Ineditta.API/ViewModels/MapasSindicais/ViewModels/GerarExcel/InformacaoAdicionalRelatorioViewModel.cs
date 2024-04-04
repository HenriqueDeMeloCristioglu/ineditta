using Ineditta.Application.ClientesUnidades.Entities;
using Ineditta.Repository.Clausulas.Views.InformacoesAdicionais;

namespace Ineditta.API.ViewModels.MapasSindicais.ViewModels.GerarExcel
{
    public class InformacaoAdicionalRelatorioViewModel
    {
        public ClausulaGeralInformacaoAdicionalVw Vw { get; set; } = null!;
        public IEnumerable<ClienteUnidade> Estabelecimentos { get; set; } = null!;
    }
}
