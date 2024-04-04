using Ineditta.API.ViewModels.ClausulasGerais.ViewModels;

namespace Ineditta.API.ViewModels.DocumentosSindicais.ViewModels.Exel
{
    public class TabelaClausulasInformacoesAdicionaisViewModel
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = null!;
        public IEnumerable<QuebraLinha> QuebrasLinhas { get; set; } = null!;
    }

    public class QuebraLinha
    {
        public IEnumerable<string> Cabecalho { get; set; } = null!;
        public IEnumerable<Linha> Linhas { get; set; } = null!;
    }

    public class Linha
    {
        public IEnumerable<InformacaoAdicionalSisap> InformacoesAdicionais { get; set; } = null!;
    }
}
